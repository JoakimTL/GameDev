﻿using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;

namespace Engine.Module.Render.Ogl.Scenes;

/// <summary>
/// Used to organize a set of related, but different scene instances. They may be related in the sense of glyphs in a sentence, or tiles on a map grid.
/// The scene instances are required to use the same VertexArrayObject and ShaderBundle. All scene instances will be found on the same render layer.
/// </summary>
public sealed class SceneInstanceCollection<TVertexData, TInstanceData>( Scene scene, uint renderLayer, OglVertexArrayObjectBase vertexArrayObject, ShaderBundleBase shaderBundle ) : Identifiable, IRemovable
	where TVertexData : unmanaged
	where TInstanceData : unmanaged {

	public OglVertexArrayObjectBase VertexArrayObject { get; } = vertexArrayObject;
	public ShaderBundleBase ShaderBundle { get; } = shaderBundle;
	public uint RenderLayer { get; } = renderLayer;

	public bool Removed { get; private set; }

	private readonly RemovableList _removables = new();
	private readonly Scene _scene = scene;

	public event RemovalHandler? OnRemoved;

	public T Create<T>() where T : InstanceBase, new() {
		T instance = _scene.CreateInstance<T>( RenderLayer );
		instance.SetVertexArrayObject( this.VertexArrayObject );
		instance.SetShaderBundle( this.ShaderBundle );
		_removables.Add( instance );
		return instance;
	}


	public void Clear() {
		_removables.Clear( true );
	}

	public void Remove() {
		_removables.Clear( true );
		Removed = true;
		OnRemoved?.Invoke( this );
	}

	public abstract class InstanceBase() : SceneInstanceBase( typeof( TInstanceData ) ) {
		protected override void Initialize() { }
		internal new void SetVertexArrayObject( OglVertexArrayObjectBase? vertexArrayObject ) => base.SetVertexArrayObject( vertexArrayObject );
		internal new void SetShaderBundle( ShaderBundleBase? shaderBundle ) => base.SetShaderBundle( shaderBundle );
		internal new void SetLayer( uint layer ) => base.SetLayer( layer );
		internal new void Remove() => base.Remove();
	}
}
