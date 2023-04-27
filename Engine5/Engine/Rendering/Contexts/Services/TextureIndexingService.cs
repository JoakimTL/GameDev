//using Engine.Datatypes.Colors;
//using Engine.Rendering.Contexts.Objects;
//using Engine.Rendering.Contexts.Objects.Assets;
//using Engine.Structure.Attributes;
//using Engine.Structure.Interfaces;
//using OpenGL;
//using System.Numerics;

//namespace Engine.Rendering.Contexts.Services;

//[ProcessAfter<TextureAssetService, IUpdateable>]
//public sealed class TextureIndexingService : Identifiable, IContextService, IUpdateable, IInitializable, IDisposable
//{

//	private ushort _lowestOpenIndex;
//	private readonly TextureAsset?[] _assets;
//	private readonly ulong[] _handles;
//	private readonly VertexBufferObjectService _vertexBufferObjectService;
//	private readonly RenderBufferObjectService _renderBufferObjectService;
//	private readonly ShaderStorageBufferObjectService _shaderStorageBufferObjectService;
//	private bool _updated;
//	private Texture? _defaultMissingTexture;
//	private ShaderStorageBufferObject? _ssbo;

//	public TextureIndexingService(VertexBufferObjectService vertexBufferObjectService, RenderBufferObjectService renderBufferObjectService, ShaderStorageBufferObjectService shaderStorageBufferObjectService)
//	{
//		_lowestOpenIndex = 0;
//		_assets = new TextureAsset?[ushort.MaxValue + 1];
//		_handles = new ulong[_assets.Length];
//		_updated = true;
//		_vertexBufferObjectService = vertexBufferObjectService;
//		this._renderBufferObjectService = renderBufferObjectService;
//		_shaderStorageBufferObjectService = shaderStorageBufferObjectService;
//	}

//	public void Initialize()
//	{
//		_ssbo = _shaderStorageBufferObjectService.Create("TextureAddresses", sizeof(ulong) * (ushort.MaxValue + 1), ShaderType.VertexShader, ShaderType.FragmentShader );
//		_defaultMissingTexture = new Texture("DefaultMissingTexture", TextureTarget.Texture2d, 3, InternalFormat.Rgba8);
//		var colors = new Color8x4[] {
//			new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0,   1), new Vector4(0, 0, 1, 1),
//			new Vector4(0, 0, 0, 1), new Vector4(0, 0, 0, .5f), new Vector4(0, 0, 0, 0),
//			new Vector4(1, 1, 1, 1), new Vector4(1, 1, 1, .5f), new Vector4(1, 1, 1, 0),
//		};
//		unsafe
//		{
//			fixed (Color8x4* srcPtr = colors)
//				_defaultMissingTexture.SetPixels(PixelFormat.Rgba, PixelType.UnsignedByte, new nint(srcPtr));
//		}
//	}

//	public void Update(float time, float deltaTime)
//	{
//		if (!_updated || _ssbo is null || _defaultMissingTexture is null)
//			return;
//		_updated = false;
//		for (int i = 0; i < _assets.Length; i++)
//		{
//			ulong handle = _assets[i]?.GetHandle() ?? 0;
//			if (handle == 0)
//				handle = _defaultMissingTexture.GetHandle();
//			_handles[i] = handle;
//		}
//		unsafe
//		{
//			fixed (ulong* srcPtr = _handles)
//				_ssbo.Write(srcPtr, (uint)(_handles.Length * sizeof(ulong)));
//		}
//	}

//	internal void SetIndex(TextureAsset asset)
//	{
//		if (_assets[_lowestOpenIndex] is not null)
//		{
//			this.LogWarning("No more free indexes for textures!");
//			return;
//		}
//		asset.SetIndex(_lowestOpenIndex);
//		_assets[_lowestOpenIndex] = asset;
//		_updated = true;
//		asset.AssetDisposed += AssetDisposed;
//		_lowestOpenIndex = GetFirstOpenIndexFrom((ushort)(_lowestOpenIndex + 1));
//	}

//	private void AssetDisposed(LoadedAssetBase assetBase)
//	{
//		if (assetBase is not TextureAsset asset)
//			return;
//		_assets[asset.Index] = null;
//		_updated = true;
//		if (asset.Index < _lowestOpenIndex)
//			_lowestOpenIndex = asset.Index;
//	}

//	private ushort GetFirstOpenIndexFrom(ushort start)
//	{
//		for (ushort i = start; i < _assets.Length; i++)
//			if (_assets[i] is null)
//				return i;
//		return ushort.MaxValue;
//	}

//	public void Dispose()
//	{
//		_ssbo?.Dispose();
//		_defaultMissingTexture?.Dispose();
//	}
//}
