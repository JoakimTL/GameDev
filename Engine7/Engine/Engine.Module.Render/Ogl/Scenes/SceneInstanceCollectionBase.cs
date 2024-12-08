using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Used to organize a set of related, but different scene instances. They may be related in the sense of glyphs in a sentence, or tiles on a map grid.
/// The scene instances are required to use the same VertexArrayObject and ShaderBundle. All scene instances will be found on the same render layer.
/// </summary>
public sealed class SceneInstanceCollection<TVertexData, TInstanceData>( Scene scene, uint renderLayer, OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle ) : DisposableIdentifiable
	where TVertexData : unmanaged
	where TInstanceData : unmanaged {

	public OglVertexArrayObjectBase VertexArrayObject { get; } = vertexArrayObject;
	public ShaderBundleBase ShaderBundle { get; } = shaderBundle;
	public uint RenderLayer { get; } = renderLayer;

	private readonly List<InstanceBase> _sceneInstances = [];
	private readonly Scene _scene = scene;

	public T Create<T>() where T : InstanceBase, new() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		T instance = _scene.CreateInstance<T>( RenderLayer );
		instance.SetVertexArrayObject( this.VertexArrayObject );
		instance.SetShaderBundle( this.ShaderBundle );
		this._sceneInstances.Add( instance );
		instance.OnDisposed += OnInstanceDisposed;
		return instance;
	}

	private void OnInstanceDisposed( IListenableDisposable disposable ) {
		if (disposable is InstanceBase sceneInstance)
			this._sceneInstances.Remove( sceneInstance );
	}

	public void Clear() {
		ObjectDisposedException.ThrowIf( this.Disposed, this );
		foreach (InstanceBase sceneInstance in this._sceneInstances)
			sceneInstance.Dispose();
		this._sceneInstances.Clear();
	}

	protected override bool InternalDispose() {
		Clear();
		return true;
	}

	public abstract class InstanceBase() : SceneInstanceBase( typeof( TInstanceData ) ) {
		protected override void Initialize() { }
		internal new void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) => base.SetVertexArrayObject( vertexArrayObject );
		internal new void SetShaderBundle( ShaderBundleBase? shaderBundle ) => base.SetShaderBundle( shaderBundle );
		internal new void SetLayer( uint layer ) => base.SetLayer( layer );
	}
}
