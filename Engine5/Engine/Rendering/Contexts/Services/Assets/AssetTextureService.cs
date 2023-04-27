using Engine.Datatypes.Vectors;
using Engine.GlobalServices;
using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Interfaces;
using OpenGL;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Advanced;

namespace Engine.Rendering.Contexts.Services.Assets;

public class AssetTextureService : ExternalAssetReferenceService<string, Texture>, IUpdateable, IDisposable
{

    private readonly Dictionary<string, Texture> _textures;
    private readonly ConcurrentQueue<string> _updatedTextures;
    private readonly FileWatchingService _fileWatchingService;

    public AssetTextureService(FileWatchingService fileWatchingService) : base("assets\\textures\\", ".png")
    {
        _textures = new();
        _updatedTextures = new();
        _fileWatchingService = fileWatchingService;
    }

    /// <summary>
    /// A requested texture should also be <see cref="Discarded(string)"/> when no longer in use.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    protected override Texture? OnRequest(string path)
    {
        if (!_textures.TryGetValue(path, out var texture))
            if (LoadFile(path, out texture))
                _textures.Add(path, texture);
        if (texture is not null)
            AddReference(path);
        return texture;
    }

    protected override void OnDiscarded(string path)
    {
        if (!_textures.Remove(path, out Texture? t))
            return;
        if (!RemoveReference(path))
            return;
        _fileWatchingService.Untrack(path, FileChanged);
        t.Dispose();
    }

    private void FileChanged(string path) => _updatedTextures.Enqueue(path);

    public void Update(float time, float deltaTime)
    {
        unsafe
        {
            while (_updatedTextures.TryDequeue(out string? path))
                if (_textures.TryGetValue(path, out var texture))
                    if (LoadRawData(path, out uint[]? pixelData, out Vector2i res))
                        fixed (uint* src = pixelData)
                            texture.SetPixels(PixelFormat.Rgba, PixelType.UnsignedByte, new nint(src));
        }
    }

    private bool LoadFile(string filepath, [NotNullWhen(true)] out Texture? t, int samples = 0, TextureMagFilter filter = TextureMagFilter.Linear)
    {
        t = null;
        if (!File.Exists(filepath))
        {
            Log.Warning($"Failed to find texture {filepath}!");
            return false;
        }

        if (!LoadRawData(filepath, out uint[]? pixelData, out Vector2i res))
            return false;

        t = new(filepath, TextureTarget.Texture2d, res, InternalFormat.Rgba8, filepath, samples, (TextureParameterName.TextureMagFilter, (int)filter), (TextureParameterName.TextureMinFilter, (int)filter));
        _fileWatchingService.Track(filepath, FileChanged);

        unsafe
        {
            fixed (uint* src = pixelData)
                t.SetPixels(PixelFormat.Rgba, PixelType.UnsignedByte, new nint(src));
        }

        Log.Line($"[{filepath}] loaded as GL Texture [{t}]!", Log.Level.NORMAL);
        return true;

    }

    private bool LoadRawData(string filepath, [NotNullWhen(true)] out uint[]? pixelData, out Vector2i resolution)
    {
        pixelData = null;
        resolution = 0;
        if (!File.Exists(filepath))
        {
            Log.Warning($"Failed to find texture {filepath}!");
            return false;
        }

        using Image<Rgba32>? img = Image.Load<Rgba32>(filepath);
        resolution = (img.Width, img.Height);
        IMemoryGroup<Rgba32>? pixels = img.GetPixelMemoryGroup();
        if (pixels is null)
        {
            Log.Warning($"Failed to load texture {filepath}!");
            return false;
        }

        pixelData = new uint[pixels.TotalLength];

        unsafe
        {
            uint bytesCopied = 0;
            fixed (uint* dst = pixelData)
            {
                foreach (Memory<Rgba32> memory in pixels)
                {
                    using (System.Buffers.MemoryHandle memHandle = memory.Pin())
                    {
                        uint bytesToCopy = (uint)memory.Length * sizeof(uint);
                        Unsafe.CopyBlock(&dst[bytesCopied], memHandle.Pointer, bytesToCopy);
                        bytesCopied += bytesToCopy;
                    }
                }
            }
        }
        return true;

    }

    public void Dispose()
    {
        foreach (var t in _textures.Values)
            t.Dispose();
        _textures.Clear();
        GC.SuppressFinalize(this);
    }
}