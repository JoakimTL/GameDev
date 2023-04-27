using Engine.Datatypes.Colors;
using Engine.Rendering.Contexts.Objects;
using Engine.Structure.Interfaces;
using OpenGL;
using System.Numerics;
using Engine.Rendering.Contexts.Objects.AssetManagerment;
using Engine.Structure.Attributes;

namespace Engine.Rendering.Contexts.Services.Assets;

[ProcessAfter<VertexBufferObjectService>(typeof(IInitializable))]
public sealed class AssetIndexedTextureService : AssetReferenceService<string>, IContextService, IInitializable, IUpdateable, IDisposable
{
    private readonly ShaderStorageBufferObjectService _shaderStorageBufferObjectService;
    private readonly AssetTextureService _assetTextureService;
    private readonly Dictionary<string, ushort> _assetIndexes;
    private readonly IndexedTexture?[] _textures;
    private readonly ulong[] _handles;
    private IndexedTexture _indexedDefaultMissingTexture = null!;
    private ShaderStorageBufferObject _ssbo = null!;
    private ushort _lowestOpenIndex;
    private bool _updated;

    public ShaderStorageBufferObject ShaderStorageBuffer => _ssbo;

    public AssetIndexedTextureService(ShaderStorageBufferObjectService shaderStorageBufferObjectService, AssetTextureService assetTextureService)
    {
        _shaderStorageBufferObjectService = shaderStorageBufferObjectService;
        _assetTextureService = assetTextureService;
        _textures = new IndexedTexture?[ushort.MaxValue + 1];
        _handles = new ulong[_textures.Length];
        _assetIndexes = new();
        _lowestOpenIndex = 1;
        _updated = true;
    }

    public void Initialize()
    {
        _ssbo = _shaderStorageBufferObjectService.Create("TextureAddresses", sizeof(ulong) * (ushort.MaxValue + 1), ShaderType.FragmentShader);
        var defaultMissingTexture = new Texture("DefaultMissingTexture", TextureTarget.Texture2d, 3, InternalFormat.Rgba8);
        var colors = new Color8x4[] {
            new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0,   1), new Vector4(0, 0, 1, 1),
            new Vector4(0, 0, 0, 1), new Vector4(0, 0, 0, .5f), new Vector4(0, 0, 0, 0),
            new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, .5f), new Vector4(1, 1, 1, 0),
        };
        unsafe
        {
            fixed (Color8x4* srcPtr = colors)
                defaultMissingTexture.SetPixels(PixelFormat.Rgba, PixelType.UnsignedByte, new nint(srcPtr));
        }
        _indexedDefaultMissingTexture = new(defaultMissingTexture, 0);
    }

    public IndexedTexture Request(string texturePath)
    {
        if (_assetIndexes.TryGetValue(texturePath, out ushort index))
        {
            AddReference(texturePath); 
            return _textures[index].NotNull();
        }
        var texture = _assetTextureService.Request(texturePath);
        if (texture is null)
            return _indexedDefaultMissingTexture;
        if (_textures[_lowestOpenIndex] is not null)
            return this.LogWarningThenReturn("No more free indexes for textures!", _indexedDefaultMissingTexture);
        index = _lowestOpenIndex;
        IndexedTexture indexedTexture = new(texture, index);
        _assetIndexes.Add(texturePath, index);
        AddReference(texturePath);
        _textures[index] = indexedTexture;
        _lowestOpenIndex = GetFirstOpenIndexFrom((ushort)(_lowestOpenIndex + 1));
        _updated = true;
        return indexedTexture;
    }

    public void Discarded(string texturePath)
    {
        if (!_assetIndexes.TryGetValue(texturePath, out ushort index))
            return;
        if (!RemoveReference(texturePath))
            return;
        var indexedTexture = _textures[index];
        if (indexedTexture is null) 
            return;
        if (!_assetIndexes.Remove(texturePath))
            return;
        _textures[index] = null;
        _assetTextureService.Discarded(texturePath);
        if (indexedTexture.Index < _lowestOpenIndex)
            _lowestOpenIndex = indexedTexture.Index;
        _updated = true;
    }

    public void Update(float time, float deltaTime)
    {
        if (!_updated)
            return;
        _updated = false;
        for (int i = 0; i < _textures.Length; i++)
        {
            ulong handle = _textures[i]?.Texture.GetHandle() ?? 0;
            if (handle == _indexedDefaultMissingTexture.Index)
                handle = _indexedDefaultMissingTexture.Texture.GetHandle();
            _handles[i] = handle;
        }
        unsafe
        {
            fixed (ulong* srcPtr = _handles)
                _ssbo.Write(srcPtr, (uint)(_handles.Length * sizeof(ulong)));
        }
    }

    private ushort GetFirstOpenIndexFrom(ushort start)
    {
        if (start == 0)
            start = 1;
        for (ushort i = start; i < _textures.Length; i++)
            if (_textures[i] is null)
                return i;
        return ushort.MaxValue;
    }

    public void Dispose()
    {
        _ssbo?.Dispose();
        _indexedDefaultMissingTexture?.Texture.Dispose();
    }
}
