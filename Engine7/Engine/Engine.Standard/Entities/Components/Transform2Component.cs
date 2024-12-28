using Engine.Module.Entities.Container;
using Engine.Transforms.Models;
using Engine.Transforms;
using Engine.Physics;

namespace Engine.Standard.Entities.Components;

public sealed class Transform2Component : ComponentBase {
	public Transform2<double> Transform { get; }

	public Transform2Component() {
		this.Transform = new();
		this.Transform.OnMatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => this.InvokeComponentChanged();
}


public sealed class UiComponent : ComponentBase {

	//Reads input messages

	


}

public sealed class Collider2Component : ComponentBase {

	public Collider2Shape ColliderShape { get; } = new();

	public void SetBaseVertices( ReadOnlySpan<Vector2<double>> vertices ) {
		ColliderShape.SetBaseVertices( vertices );
		InvokeComponentChanged();
	}

	public void SetBaseVertices( IEnumerable<Vector2<double>> vertices ) {
		ColliderShape.SetBaseVertices( vertices );
		InvokeComponentChanged();
	}

	internal void SetTransform( IMatrixProvider<double>? transform ) {
		ColliderShape.SetTransform( transform );
		InvokeComponentChanged();
	}
}

public sealed class UiElementArchetype : ArchetypeBase {
	public UiComponent UiComponent { get; set; } = null!;
	public Transform2Component Transform2Component { get; set; } = null!;
	public Collider2Component Collider2Component { get; set; } = null!;

	protected override void OnEntitySet( Entity e ) {
		Collider2Component.SetTransform( Transform2Component.Transform );
	}

	protected override void OnArchetypeRemoved() {
		Collider2Component.SetTransform( null );
	}
}

public sealed class Collider2Shape : ConvexShapeBase<Vector2<double>, double> {
	public Collider2Shape() : base( Array.Empty<Vector2<double>>().AsSpan() ) { }

	internal new void SetBaseVertices( ReadOnlySpan<Vector2<double>> vertices ) => base.SetBaseVertices( vertices );
	internal new void SetBaseVertices( IEnumerable<Vector2<double>> vertices ) => base.SetBaseVertices( vertices );
	internal new void SetTransform( IMatrixProvider<double>? transform ) => base.SetTransform( transform );
}