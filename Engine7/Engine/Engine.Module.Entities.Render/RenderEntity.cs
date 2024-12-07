﻿using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Ogl.OOP.Shaders;
using Engine.Module.Render.Ogl.OOP.VertexArrays;
using Engine.Module.Render.Ogl.Scenes;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Entities.Render;

public sealed class RenderEntity : DisposableIdentifiable, IUpdateable {
	private readonly Entity _entity;
	private readonly List<IDisposable> _disposables;
	private readonly Dictionary<Type, RenderBehaviourBase> _behaviours;

	public RenderEntityServiceAccess ServiceAccess { get; }

	internal RenderEntity( Entity entity, RenderEntityServiceAccess serviceAccess ) {
		this._entity = entity;
		this.ServiceAccess = serviceAccess;
		this._behaviours = [];
		this._disposables = [];
	}

	public T RequestSceneInstance<T>( string sceneName, uint layer ) where T : SceneInstanceBase, new() {
		T instance = this.ServiceAccess.SceneInstanceProvider.RequestSceneInstance<T>( sceneName, layer );
		this._disposables.Add( instance );
		return instance;
	}

	public SceneInstanceCollection<TVertexData, TInstanceData> RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( string sceneName, uint layer ) 
		where TVertexData : unmanaged
		where TInstanceData : unmanaged
		where TShaderBundle : ShaderBundleBase {
		SceneInstanceCollection<TVertexData, TInstanceData> collection = this.ServiceAccess.SceneInstanceProvider.RequestSceneInstanceCollection<TVertexData, TInstanceData, TShaderBundle>( sceneName, layer );
		this._disposables.Add( collection );
		return collection;
	}

	//public T? RequestShaderBundle<T>() where T : ShaderBundleBase
	//	=> this._serviceAccess.ShaderBundleProvider.GetShaderBundle<T>() as T;

	//public OglVertexArrayObjectBase? RequestCompositeVertexArray<TVertexData, TSceneInstanceData>() where TVertexData : unmanaged where TSceneInstanceData : unmanaged
	//	=> this._serviceAccess.CompositeVertexArrayProvider.GetVertexArray<TVertexData, TSceneInstanceData>();

	//public VertexMesh<TVertex> RequestNewEmptyMesh<TVertex>( uint vertexCount, uint elementCount ) where TVertex : unmanaged
	//	=> this._serviceAccess.MeshProvider.CreateEmptyMesh<TVertex>( vertexCount, elementCount );

	//public VertexMesh<TVertex> RequestNewMesh<TVertex>( Span<TVertex> vertices, Span<uint> elements ) where TVertex : unmanaged
	//	=> this._serviceAccess.MeshProvider.CreateMesh( vertices, elements );

	//TODO: public void ListenToEvents<T>( Action<T> action ) => this._entity.ListenToEvents( action );

	public void SendMessageToEntity( object message ) => this._entity.AddMessage( message );

	public bool AddBehaviour( RenderBehaviourBase behaviour ) {
		if (!this._behaviours.TryAdd( behaviour.GetType(), behaviour ))
			return this.LogWarningThenReturn( $"Behaviour of type {behaviour.GetType().Name} already exists.", false );
		return true;
	}

	public void RemoveBehaviour( Type behaviourType ) {
		if (!this._behaviours.Remove( behaviourType ))
			this.LogWarning( $"Couldn't find behaviour of type {behaviourType.Name}." );
	}

	public bool TryGetBehaviour<T>( [NotNullWhen( true )] out T? behaviour ) where T : RenderBehaviourBase
		=> (behaviour = null) is null && this._behaviours.TryGetValue( typeof( T ), out RenderBehaviourBase? baseBehaviour ) && (behaviour = baseBehaviour as T) is not null;

	public void Update( double time, double deltaTime ) {
		foreach (RenderBehaviourBase behaviour in this._behaviours.Values)
			behaviour.Update( time, deltaTime );
	}

	protected override bool InternalDispose() {
		foreach (RenderBehaviourBase behaviour in this._behaviours.Values)
			behaviour.Dispose();
		this._behaviours.Clear();
		foreach (IDisposable disposable in this._disposables)
			disposable.Dispose();
		this._disposables.Clear();
		return true;
	}
}
