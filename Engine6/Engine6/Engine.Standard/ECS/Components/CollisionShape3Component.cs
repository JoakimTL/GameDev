using Engine.Data;
using Engine.Math.NewFolder;
using Engine.Modules.ECS;
using Engine.Modules.Physics;

namespace Engine.Standard.ECS.Components;

public sealed class CollisionShape3Component : ComponentBase {

	public CollisionShapeTemplate3? Template { get; private set; }
	public CollisionShape3? Shape { get; private set; }
	private IMatrixProvider? _matrixProvider;

	public CollisionShape3Component() {
		this.Template = null;
		this.Shape = null;
		this._matrixProvider = null;
		EntitySet += OnEntitySet;
		ComponentChanged += OnComponentChanged;
	}

	private void OnComponentChanged( ComponentBase component ) {
		if (this.Template is null) {
			this.Shape = null;
			return;
		}
		this.Shape ??= new( this.Template );
		this.Shape.SetMatrixProvider( this._matrixProvider );
	}

	private void OnEntitySet( ComponentBase component ) {
		this.Entity.ComponentAdded += OnComponentAdded;
		this.Entity.ComponentRemoved += OnComponentRemoved;
		if (this.Entity.TryGetComponent( out Transform3Component? tc ))
			SetMatrixProvider( tc.Transform );
	}

	private void OnComponentAdded( Entity entity, ComponentBase component ) {
		if (component is Transform3Component tc)
			SetMatrixProvider( tc.Transform );
	}

	private void OnComponentRemoved( Entity entity, ComponentBase component ) {
		if (component is Transform3Component)
			SetMatrixProvider( null );
	}

	public void SetTemplate( CollisionShapeTemplate3 template ) {
		this.Template = template;
		TriggerChanged();
	}

	private void SetMatrixProvider( IMatrixProvider? matrixProvider ) {
		this._matrixProvider = matrixProvider;
		TriggerChanged();
	}

	internal bool TryGetAABB( out AABB3<double> aabb ) {
		throw new NotImplementedException();
	}
}
