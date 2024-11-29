using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Ogl.Scenes;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Entities.Render;

public sealed class RenderEntity : DisposableIdentifiable, IUpdateable {
	private readonly Entity _entity;
	private readonly SceneInstanceProvider _sceneInstanceProvider;

	private readonly List<SceneInstanceBase> _sceneInstances;
	private readonly Dictionary<Type, RenderBehaviourBase> _behaviours;

	internal RenderEntity( Entity entity, SceneInstanceProvider sceneInstanceProvider ) {
		this._entity = entity;
		this._sceneInstanceProvider = sceneInstanceProvider;
		this._behaviours = [];
		_sceneInstances = [];
	}

	public T RequestSceneInstance<T>(string sceneName, uint layer ) where T : SceneInstanceBase, new() {
		T instance = this._sceneInstanceProvider.RequestSceneInstance<T>( sceneName, layer );
		_sceneInstances.Add( instance );
		return instance;
	}

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
		return true;
	}
}
