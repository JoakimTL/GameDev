﻿//using Engine.GlobalServices;
//using Engine.Rendering.Contexts.Objects;
//using Engine.Rendering.Contexts.Objects.Assets;
//using Engine.Structure.Interfaces;
//using System.Collections.Concurrent;

//namespace Engine.Rendering.Contexts.Services;

//public abstract class LoadedAssetService<T> : Identifiable, IContextService, IUpdateable where T : LoadedAssetBase
//{

//	public string BaseDirectory { get; }

//	private readonly ReferenceCountingService _referenceCountingService;

//	private readonly ConcurrentDictionary<string, T> _assets;
//	private readonly ConcurrentQueue<T> _newAssets;
//	private readonly ConcurrentQueue<T> _dereferencedAssets;
//	private readonly HashSet<T> _oldAssets;
//	private readonly List<T> _removingAssets;

//	protected LoadedAssetService(string baseDirectory, ReferenceCountingService referenceCountingService)
//	{
//		BaseDirectory = baseDirectory;
//		_referenceCountingService = referenceCountingService;
//		_referenceCountingService.Unreferenced += Unreferenced;
//		_assets = new();
//		_newAssets = new();
//		_dereferencedAssets = new();
//		_oldAssets = new();
//		_removingAssets = new();
//	}

//	protected abstract void LoadAsset(T asset);
//	protected abstract T CreateAsset(string path);

//	private void Unreferenced(object obj)
//	{
//		if (obj is T asset)
//			_dereferencedAssets.Enqueue(asset);
//	}

//	public void Update(float time, float deltaTime)
//	{
//		while (_newAssets.TryDequeue(out var asset))
//			LoadAsset(asset);

//		while (_dereferencedAssets.TryDequeue(out var asset))
//			if (_oldAssets.Add(asset))
//				asset.UnloadingTime = time + 60;
//		_removingAssets.AddRange(_oldAssets.Where(p => p.UnloadingTime > time));
//		for (int i = 0; i < _removingAssets.Count; i++)
//		{
//			var asset = _removingAssets[i];
//			asset.Dispose();
//			_assets.TryRemove(asset.Path, out _);
//			_oldAssets.Remove(asset);
//		}
//		_removingAssets.Clear();
//	}

//	public ReferenceContainer<T>? Get(string assetPath)
//	{
//		string path = $"{BaseDirectory}{assetPath}";
//		if (!File.Exists(path))
//		{
//			this.LogWarning($"File {path} not found!");
//			return null;
//		}
//		if (!_assets.TryGetValue(path, out var asset))
//		{
//			asset = CreateAsset(path);
//			if (_assets.TryAdd(path, asset))
//			{
//				_newAssets.Enqueue(asset);
//			}
//			else
//			{
//				asset = _assets[path];
//			}
//		}
//		if (asset is null)
//			return null;
//		var ret = new ReferenceContainer<T>(asset);
//		_referenceCountingService.Reference(ret, asset);
//		return ret;
//	}
//}


//public sealed class TextureAssetService : LoadedAssetService<TextureAsset>
//{
//	private readonly TextureService _textureService;
//	private readonly TextureIndexingService _textureIndexingService;

//	public TextureAssetService(TextureService textureService, TextureIndexingService textureIndexingService, ReferenceCountingService referenceCountingService)
//		: base("assets/textures/", referenceCountingService)
//	{
//		_textureService = textureService;
//		_textureIndexingService = textureIndexingService;
//	}

//	protected override void LoadAsset(TextureAsset asset)
//	{
//		Texture? texture = _textureService.Get(asset.Path);
//		if (texture is null)
//		{
//			this.LogWarning($"Unable to load texture for {asset}!");
//			return;
//		}

//		asset.SetTexture(texture);
//		_textureIndexingService.SetIndex(asset);
//	}

//	protected override TextureAsset CreateAsset(string path) => new(path);
//}
