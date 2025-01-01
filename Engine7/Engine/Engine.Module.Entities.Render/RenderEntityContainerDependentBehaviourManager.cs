using Engine.Module.Entities.Container;
using System.Collections.Concurrent;

namespace Engine.Module.Render.Entities;

internal class RenderEntityContainerDependentBehaviourManager( RenderEntityContainer renderEntityContainer ) : IUpdateable {

	private readonly RenderEntityContainer _renderEntityContainer = renderEntityContainer;
	private readonly ConcurrentQueue<ArchetypeBase> _archetypesAdded = [];
	private readonly ConcurrentQueue<ArchetypeBase> _archetypesRemoved = [];

	//Called on game logic thread
	internal void OnArchetypeAdded( ArchetypeBase archetype ) => this._archetypesAdded.Enqueue( archetype );

	//Called on game logic thread
	internal void OnArchetypeRemoved( ArchetypeBase archetype ) => this._archetypesRemoved.Enqueue( archetype );

	private void ProcessAddedArchetype( ArchetypeBase archetype ) {
		if (!this._renderEntityContainer.TryGetRenderEntity( archetype.Entity.EntityId, out RenderEntity? renderEntity ))
			return;
		renderEntity.AddDependenciesOnArchetype( archetype );
	}

	private void ProcessRemovedArchetype( ArchetypeBase archetype ) {
		if (!this._renderEntityContainer.TryGetRenderEntity( archetype.Entity.EntityId, out RenderEntity? renderEntity ))
			return;
		renderEntity.RemoveAllDependentsOnArchetype( archetype );
	}

	public void Update( double time, double deltaTime ) {
		while (this._archetypesAdded.TryDequeue( out ArchetypeBase? archetype ))
			ProcessAddedArchetype( archetype );
		while (this._archetypesRemoved.TryDequeue( out ArchetypeBase? archetype ))
			ProcessRemovedArchetype( archetype );
	}
}