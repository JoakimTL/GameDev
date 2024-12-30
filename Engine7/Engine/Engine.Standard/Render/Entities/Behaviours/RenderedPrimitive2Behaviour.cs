using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Entities.Components.Rendering;
using Engine.Standard.Render.Entities.Behaviours.Shaders;
using Engine.Standard.Render.Meshing;
using Engine.Standard.Render.Meshing.Services;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class RenderedPrimitive2Behaviour : SynchronizedRenderBehaviourBase<RenderedPrimitive2Archetype> {

	private Primitive2? _incomingPrimitive;
	private Primitive2? _currentPrimitive;

	private IMesh? _primitiveMesh;
	private PrimitiveMesh2Provider _primitiveMeshProvider = null!;
	private SceneInstance<Entity2SceneData> _sceneInstance = null!;

	protected override void OnRenderEntitySet() {
		_primitiveMeshProvider = this.RenderEntity.ServiceAccess.Get<PrimitiveMesh2Provider>();
		_sceneInstance = this.RenderEntity.RequestSceneInstance<SceneInstance<Entity2SceneData>>( "test", 0 );
		_sceneInstance.SetVertexArrayObject( this.RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SceneData>() );
		_sceneInstance.SetShaderBundle( this.RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<Primitive2ShaderBundle>() );
		_incomingPrimitive = Archetype.RenderedPrimitive2Component.Primitive;
		ForceSynchronization();
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (RenderEntity.TryGetBehaviour( out Transform2Behaviour? t2b ))
			_sceneInstance.Write( new Entity2SceneData( t2b.Transform.Matrix ) );
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is RenderedPrimitive2Component r2c) {
			this._incomingPrimitive = r2c.Primitive;
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		if (this._incomingPrimitive != this._currentPrimitive) {
			this._currentPrimitive = this._incomingPrimitive;
			_primitiveMesh = _currentPrimitive.HasValue ? _primitiveMeshProvider.Get( _currentPrimitive.Value ) : null;
			_sceneInstance.SetMesh( _primitiveMesh );
		}
	}
}
