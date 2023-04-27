using Engine.Datatypes.Vectors;
using Engine.Rendering.OGL;
using OpenGL;

namespace Engine.Rendering.Contexts.Objects;
public abstract class FrameBuffer : Identifiable, IDisposable
{
    private uint _framebufferId;
    private readonly Dictionary<int, bool> _activeAttachments;
    private readonly List<uint> _renderBuffers;
    private readonly List<Texture> _textures;
    private readonly Viewport _viewport;

    public Proportions Resolution { get; private set; }
    public Vector2i Size => Resolution?.Size ?? 1;

    public FrameBuffer(Viewport viewport, Proportions proportions)
    {
        if (proportions is null)
            throw new ArgumentNullException(nameof(proportions));
        _framebufferId = Gl.CreateFramebuffer();
        _activeAttachments = new Dictionary<int, bool>();
        _renderBuffers = new List<uint>();
        _textures = new List<Texture>();
        _viewport = viewport;
        Resolution = proportions;
        Resolution.Resized += ProportionsChanged;
        ProportionsChanged();
    }

#if DEBUG
    ~FrameBuffer()
    {
        System.Diagnostics.Debug.Fail($"{this} was not disposed!");
    }
#endif

    public Texture CreateTexture(TextureTarget target, InternalFormat internalFormat, params (TextureParameterName, int)[] parameters)
    {
        Texture t = new($"FBO#{IdentifiableName}:{internalFormat}", target, Size, internalFormat, null, 0, parameters);
        _textures.Add(t);
        this.LogLine($"Created new texture [{t}]!", Log.Level.LOW, ConsoleColor.Cyan);
        return t;
    }

    public uint CreateRenderbuffer(InternalFormat format, int samples)
    {
        uint buffer = Gl.CreateRenderbuffer();
        if (samples <= 0)
        {
            Gl.NamedRenderbufferStorage(buffer, format, Size.X, Size.Y);
        }
        else
        {
            Gl.NamedRenderbufferStorageMultisample(buffer, samples, format, Size.X, Size.Y);
        }
        return buffer;
    }

    private void ProportionsChanged()
    {
        if (_framebufferId == 0)
            return;
        Wipe();
        Generate();
        Validate();
    }

    public void SetProportions(Proportions proportions)
    {
        if (Resolution is not null)
        {
            Resolution.Resized -= ProportionsChanged;
            Resolution.Dispose();
        }
        if (proportions is null)
            return;
        Resolution = proportions;
        Resolution.Resized += ProportionsChanged;
        ProportionsChanged();
    }

    public void BlitToFrameBuffer(FrameBuffer destination, ClearBufferMask mask, BlitFramebufferFilter filter)
    {
        Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _framebufferId);
        Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, destination._framebufferId);
        Gl.BlitFramebuffer(
            0, 0, Size.X, Size.Y,
            0, 0, destination.Size.X, destination.Size.Y,
            mask, filter);
        Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
    }

    public void BlitToScreen(Window window, ClearBufferMask mask, BlitFramebufferFilter filter)
    {
        Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _framebufferId);
        Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        Gl.BlitFramebuffer(
            0, 0, Size.X, Size.Y,
            0, 0, window.Size.X, window.Size.Y,
            mask, filter);
        Gl.DrawBuffer(DrawBufferMode.Back);
        Gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
    }

    public abstract void Clear();
    protected abstract void Generate();

    private void Wipe()
    {
        foreach (KeyValuePair<int, bool> kvp in _activeAttachments)
        {
            if (kvp.Value)
            {
                DetachBuffer((FramebufferAttachment)kvp.Key);
            }
            else
            {
                DetachTexture((FramebufferAttachment)kvp.Key);
            }
        }
        if (_renderBuffers.Count > 0)
        {
            Gl.DeleteRenderbuffers(_renderBuffers.ToArray());
            _renderBuffers.Clear();
        }

        for (int i = 0; i < _textures.Count; i++)
            _textures[i].Dispose();
        _textures.Clear();
    }

    protected void Validate()
    {
        FramebufferStatus status = Gl.CheckNamedFramebufferStatus(_framebufferId, FramebufferTarget.Framebuffer);
        if (status != FramebufferStatus.FramebufferComplete)
        {
            this.LogWarning($"Framebuffer validation failed: {status}");
            return;
        }
        this.LogLine("Framebuffer validated!", Log.Level.HIGH, color: ConsoleColor.Green);
    }

    #region Attachment
    /// <summary>
    /// Attaches a texture to the framebuffer at the specified attachment point.
    /// </summary>
    /// <param name="attachment">The attachment point</param>
    /// <param name="tex">The texture id</param>
    /// <param name="level">The texture level, if the texture has several layers you must choose which layer to write to.</param>
    public void AttachTexture(FramebufferAttachment attachment, Texture tex, int level = 0)
    {
        if (tex is null)
            return;
        if (tex.TextureID == 0)
            return;
        Gl.NamedFramebufferTexture(_framebufferId, attachment, tex.TextureID, level);
        _activeAttachments[(int)attachment] = false;
        this.LogLine($"Attached texture [{tex}] to [{attachment}]!", Log.Level.NORMAL);
    }

    /// <summary>
    /// Detaches a texture from the framebuffer at the specified attachment point.
    /// </summary>
    /// <param name="attachment">The attachment point</param>
    public void DetachTexture(FramebufferAttachment attachment)
    {
        Gl.NamedFramebufferTexture(_framebufferId, attachment, 0, 0);
        _activeAttachments.Remove((int)attachment);
    }

    /// <summary>
    /// Attaches a buffer to the framebuffer at the specified attachment point.
    /// </summary>
    /// <param name="attachment">The attachment point</param>
    /// <param name="buf">The buffer id</param>
    public void AttachBuffer(FramebufferAttachment attachment, uint buf)
    {
        if (buf == 0)
            return;
        Gl.NamedFramebufferRenderbuffer(_framebufferId, attachment, RenderbufferTarget.Renderbuffer, buf);
        _activeAttachments[(int)attachment] = true;
    }

    /// <summary>
    /// Detaches a buffer from the framebuffer at the specified attachment point.
    /// </summary>
    /// <param name="attachment">The attachment point</param>
    public void DetachBuffer(FramebufferAttachment attachment)
    {
        Gl.NamedFramebufferRenderbuffer(_framebufferId, attachment, RenderbufferTarget.Renderbuffer, 0);
        _activeAttachments.Remove((int)attachment);
    }

    public void EnableCurrentColorAttachments()
    {
        List<int> attachments = new();
        int i = 0;
        foreach (int attachment in _activeAttachments.Keys)
            if (attachment != (int)FramebufferAttachment.DepthAttachment && attachment != (int)FramebufferAttachment.DepthStencilAttachment)
            {
                attachments.Add(attachment);
                i++;
            }
        attachments.Sort();
        Gl.NamedFramebufferDrawBuffers(_framebufferId, i, attachments.ToArray());
    }

    protected void Clear(OpenGL.Buffer bufferType, int buffer, uint[] values) => Gl.ClearNamedFramebuffer(_framebufferId, bufferType, buffer, values);

    protected void Clear(OpenGL.Buffer bufferType, int buffer, int[] values) => Gl.ClearNamedFramebuffer(_framebufferId, bufferType, buffer, values);

    protected void Clear(OpenGL.Buffer bufferType, int buffer, float[] values) => Gl.ClearNamedFramebuffer(_framebufferId, bufferType, buffer, values);

    protected void ClearDepthStencil(OpenGL.Buffer bufferType, int buffer, float depth, int stencil) => Gl.ClearNamedFramebuffer(_framebufferId, bufferType, buffer, depth, stencil);
    #endregion

    public void Bind()
    {
        Gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebufferId);
        _viewport.Set(0, Size);
    }

    internal static void Unbind(FramebufferTarget target) => Gl.BindFramebuffer(target, 0);

    public void Dispose()
    {
        Wipe();
        Gl.DeleteFramebuffers(_framebufferId);
        _framebufferId = 0;
        GC.SuppressFinalize(this);
    }

    public abstract class Proportions : Identifiable, IDisposable
    {
        public abstract Vector2i Size { get; }
        public abstract event Action Resized;

        public abstract void Dispose();
    }

    public sealed class ProportionalProportions : Proportions
    {

        public override Vector2i Size => Vector2i.Max(Vector2i.Ceiling(_proportions.Size * _scale), 1);
        public override event Action? Resized;

        private readonly Proportions _proportions;
        private readonly float _scale;

        public ProportionalProportions(Proportions proportions, float scale)
        {
            _proportions = proportions;
            _scale = scale;
            _proportions.Resized += Resized;
        }

        public override void Dispose()
        {
            _proportions.Resized -= Resized;
        }
    }

    public sealed class WindowProportions : Proportions
    {

        public override Vector2i Size => Vector2i.Max(Vector2i.Ceiling(_window.Size * _scale), 1);
        public override event Action? Resized;

        private readonly Window _window;
        private readonly float _scale;

        public WindowProportions(Window window, float scale)
        {
            _window = window;
            _scale = scale;
            window.Resized += WindowResized;
        }

        private void WindowResized(Window window) => Resized?.Invoke();

        public override void Dispose()
        {
            _window.Resized -= WindowResized;
        }
    }

    public sealed class StaticProportions : Proportions
    {

        private Vector2i _size;
        public override Vector2i Size => _size;
        public override event Action? Resized;

        public StaticProportions(Vector2i initialSize)
        {
            if (Vector2i.NegativeOrZero(initialSize) || initialSize.X == 0 || initialSize.Y == 0)
            {
                this.LogWarning("Size must be greater than zero.");
                throw new ArgumentException(nameof(initialSize));
            }
            _size = initialSize;
        }

        public void Resize(Vector2i newSize)
        {
            if (Vector2i.NegativeOrZero(newSize) || newSize.X == 0 || newSize.Y == 0)
            {
                this.LogWarning("Size must be greater than zero.");
                return;
            }
            _size = newSize;
            Resized?.Invoke();
        }

        public override void Dispose() { }
    }
}
