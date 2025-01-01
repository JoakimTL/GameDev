using Engine.Module.Entities.Container;
using Engine.Transforms.Models;
using Engine.Transforms;
using Engine.Logging;

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
	public Vector4<double> NormalColor { get; private set; } = 1;
	public Vector4<double> HoverColor { get; private set; } = 0.75;
	public Vector4<double> PressedColor { get; private set; } = 0.5;

	public void SetColors( Vector4<double> normal, Vector4<double> hover, Vector4<double> pressed ) {
		NormalColor = normal;
		HoverColor = hover;
		PressedColor = pressed;
		InvokeComponentChanged();
	}
}

public sealed class Collider2Component : ComponentBase {

	private readonly List<Vector2<double>> _colliderVertices = [];

	public IReadOnlyList<Vector2<double>> ColliderVertices => _colliderVertices;

	public void SetBaseVertices( ReadOnlySpan<Vector2<double>> vertices ) {
		_colliderVertices.Clear();
		_colliderVertices.AddRange( vertices );
		InvokeComponentChanged();
	}

	public void SetBaseVertices( IEnumerable<Vector2<double>> vertices ) {
		_colliderVertices.Clear();
		_colliderVertices.AddRange( vertices );
		InvokeComponentChanged();
	}
}

public sealed class UiElementArchetype : ArchetypeBase {
	public UiComponent UiComponent { get; set; } = null!;
	public Transform2Component Transform2Component { get; set; } = null!;
	public Collider2Component Collider2Component { get; set; } = null!;
}

public sealed class ButtonComponent : ComponentBase, IMessageReadingComponent {
	public void ReadMessage( object message ) {
		if (message is UiElementClicked clicked) {
			this.LogLine( $"Button clicked with button {clicked.Button} at time {clicked.Time}." );
		}
	}
}
