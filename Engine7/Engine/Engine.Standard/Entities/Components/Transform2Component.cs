using Engine.Module.Entities.Container;
using Engine.Transforms.Models;
using Engine.Transforms;

namespace Engine.Standard.Entities.Components;

public sealed class Transform2Component : ComponentBase {
	public Transform2<double> Transform { get; }

	public Transform2Component() {
		this.Transform = new();
		this.Transform.OnMatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => this.InvokeComponentChanged();
}

public sealed class Transform3Component : ComponentBase {
	public Transform3<double> Transform { get; }

	public Transform3Component() {
		this.Transform = new();
		this.Transform.OnMatrixChanged += OnMatrixChanged;
	}

	private void OnMatrixChanged( IMatrixProvider<double> provider ) => this.InvokeComponentChanged();
}
