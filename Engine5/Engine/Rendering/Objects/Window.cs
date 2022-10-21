using Engine.Datatypes;
using Engine.Rendering.OGL;
using Engine.Structure.Interfaces;
using GLFW;
using OpenGL;
using System.Numerics;

namespace Engine.Rendering.Objects;

public sealed class Window : Identifiable, IUpdateable, IDisposable
{

    public readonly WindowPtr Pointer;
    private readonly Context _context;
    private readonly Viewport _viewport;

    public Vector2i Size { get; private set; }
    public Vector2 AspectRatioVector { get; private set; }
    public float AspectRatio { get; private set; }
    public bool Focused { get; private set; }

    public delegate void WindowEventHandler(Window window);
    public event WindowEventHandler? Resized;
    public event WindowEventHandler? Closing;
    public bool Closed { get; private set; }

    public Window(WindowPtr pointer)
    {
        Pointer = pointer;
        Closed = false;
        _viewport = new();
        _context = new(this);
        _context.Bind();
        Gl.BindAPI();
        //ContextUtilities.SwapInterval( settings?.vsyncLevel ?? 0 );
        Log.Line("Using OpenGL version: " + Gl.GetString(StringName.Version), Log.Level.NORMAL, ConsoleColor.Blue);
        Gl.GetInteger(GetPName.MaxVertexAttribBindings, out uint maxAttribBindings);
        Gl.GetInteger(GetPName.MaxVertexAttribs, out uint maxAttribs);
        Gl.GetInteger(GetPName.MaxVertexAttribRelativeOffset, out uint maxAttribOffset);
        Log.Line($"Max vertex spec: Bindings: {maxAttribBindings}, Attributes: {maxAttribs}, Offset: {maxAttribOffset}B", Log.Level.NORMAL, ConsoleColor.Blue);
        Gl.GetInteger(GetPName.MaxUniformBlockSize, out uint maxBlockSize);
        Gl.GetInteger(GetPName.MaxUniformBufferBindings, out uint maxBindings);
        Gl.GetInteger(GetPName.MaxUniformLocations, out uint maxLocations);
        Log.Line($"Max uniform spec: Bindings: {maxBindings}, Locations: {maxLocations}, Block size: {maxBlockSize}B", Log.Level.NORMAL, ConsoleColor.Blue);
        Gl.GetInteger(GetPName.MaxShaderStorageBufferBindings, out maxBindings);
        Log.Line($"Max shader storage spec: Bindings: {maxBindings}", Log.Level.NORMAL, ConsoleColor.Blue);
        //TODO: move debug callback to GL event handler
        //Event handler should be a render service, as it's not a context specific service.
        //this._debugCallback = GLDebugHandler;
        //Gl.DebugMessageCallback( this._debugCallback, IntPtr.Zero );
    }

    public void Bind()
    {
        //TODO
        //Framebuffer.Unbind( FramebufferTarget.DrawFramebuffer );
        _viewport.Set(0, Size);
    }

    private void OnFocused(bool value) => Focused = value;

    public void Update(float time, float deltaTime)
    {
        if (Closed)
        {
            this.LogError("Window has been closed!");
            return;
        }
        if (WindowUtilities.ShouldWindowClose(Pointer))
        {
            WindowUtilities.SetTitle(Pointer, "Closing!");
            Dispose();
            Closed = true;
            Closing?.Invoke(this);
            return;
        }

        _context.Bind();
        Glfw.GetWindowSize(Pointer, out int w, out int h);
        if (Size.X != w || Size.Y != h)
        {
            Size = (w, h);
            UpdateAspectRatio();
            Resized?.Invoke(this);
        }

        _context.Update(time, deltaTime);
        WindowUtilities.SetTitle(Pointer, (1f / deltaTime).ToString("N2"));
        WindowUtilities.SwapBuffer(Pointer);
    }

    private void UpdateAspectRatio()
    {
        AspectRatio = (float)Size.X / Size.Y;
        float aspectRatioX = Size.X > Size.Y ? (float)Size.X / Size.Y : 1;
        float aspectRatioY = Size.Y > Size.X ? (float)Size.Y / Size.X : 1;
        AspectRatioVector = new Vector2(aspectRatioX, aspectRatioY);
    }

    public void SetSize(Vector2i newSize)
    {
        if (Vector2i.NegativeOrZero(newSize))
        {
            this.LogError("Tried to set window size to zero or a negative number.");
            return;
        }
        if (Size == newSize)
            return;
        WindowUtilities.SetSize(Pointer, newSize.X, newSize.Y);
    }

    public void SetTitle(string newTitle)
    {
        if (string.IsNullOrEmpty(newTitle))
        {
            this.LogError("Tried to set window title to an invalid value.");
            return;
        }
        WindowUtilities.SetTitle(Pointer, newTitle);
    }

    public Vector2i GetPixelCoord(Vector2 ndc) => Vector2i.Floor((ndc + Vector2.One) * 0.5f * Size);
    public static implicit operator WindowPtr(Window window) => window.Pointer;

    public void Dispose()
    {
        _context.Dispose();
        WindowUtilities.DestroyWindow(Pointer);
    }
}
