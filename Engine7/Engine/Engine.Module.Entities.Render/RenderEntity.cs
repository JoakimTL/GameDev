using Engine.Logging;
using Engine.Module.Entities.Container;
using System.Diagnostics.CodeAnalysis;

namespace Engine.Module.Entities.Render;
public sealed class Scene : DisposableIdentifiable {
	protected override bool InternalDispose() {
		throw new NotImplementedException();
	}
}

public sealed class RenderEntity : DisposableIdentifiable, IUpdateable {
	private readonly Entity _entity;

	private readonly Dictionary<Type, RenderBehaviourBase> _behaviours;

	internal RenderEntity( Entity entity ) {
		this._entity = entity;
		this._behaviours = [];

	}

	public void SendMessageToEntity( object message ) => _entity.AddMessage( message );

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
