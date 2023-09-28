using Engine.Data;
using Engine.Modules.Physics;

namespace Engine.Modules.Entity.Components;

public sealed class CollisionShape2Component : ComponentBase {

	public CollisionShapeTemplate2? Template { get; private set; }
	public CollisionShape2? Shape { get; private set; }
	private IMatrixProvider? _matrixProvider;

	public CollisionShape2Component() {
		this.Template = null;
		this.Shape = null;
		this._matrixProvider = null;
		EntitySet += OnEntitySet;
		ComponentChanged += OnComponentChanged;
	}

	private void OnComponentChanged( ComponentBase component ) {
		if ( this.Template is null ) {
			this.Shape = null;
			return;
		}
		this.Shape ??= new( this.Template );
		this.Shape.SetMatrixProvider( this._matrixProvider );
	}

	private void OnEntitySet( ComponentBase component ) {
		this.Entity.ComponentAdded += OnComponentAdded;
		this.Entity.ComponentRemoved += OnComponentRemoved;
		if ( this.Entity.TryGetComponent( out Transform2Component? tc ) )
			SetMatrixProvider( tc.Transform );
	}

	private void OnComponentAdded( Entity entity, ComponentBase component ) {
		if ( component is Transform2Component tc )
			SetMatrixProvider( tc.Transform );
	}

	private void OnComponentRemoved( Entity entity, ComponentBase component ) {
		if ( component is Transform2Component )
			SetMatrixProvider( null );
	}

	public void SetTemplate( CollisionShapeTemplate2 template ) {
		this.Template = template;
		TriggerChanged();
	}

	private void SetMatrixProvider( IMatrixProvider? matrixProvider ) {
		this._matrixProvider = matrixProvider;
		TriggerChanged();
	}
}