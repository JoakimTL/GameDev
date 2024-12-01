using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Standard.Entities.Components;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class TextRenderingArchetype : ArchetypeBase {
	public RenderedTextComponent RenderedTextComponent { get; set; } = null!;
	public Transform2Component Transform2Component { get; set; } = null!;
}

public sealed class Transform2Archetype : ArchetypeBase {
	public Transform2Component Transform2Component { get; set; } = null!;
}

public sealed class Transform2Behaviour : SynchronizedRenderBehaviourBase<Transform2Archetype> {

	private Matrix4x4<float> _unsynchronizedMatrix;
	public Matrix4x4<float> SynchronizedMatrix { get; private set; }

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is Transform2Component t2c) {
			_unsynchronizedMatrix = t2c.Transform.Matrix.CastSaturating<double, float>();
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		SynchronizedMatrix = _unsynchronizedMatrix;
	}
}

public sealed class TextRenderingBehaviour : DependentRenderBehaviourBase<TextRenderingArchetype> {
	private Transform2Behaviour? _subscribedTransformBehaviour;

	public override void Update( double time, double deltaTime ) {
		if (_subscribedTransformBehaviour is null) {
			if (!RenderEntity.TryGetBehaviour( out _subscribedTransformBehaviour ))
				return;
			_subscribedTransformBehaviour.OnSynchronized += OnTransformSynchronized;
		}
	}

	private void OnTransformSynchronized( RenderBehaviourBase @base ) {

	}

	protected override bool InternalDispose() {
		_subscribedTransformBehaviour?.Dispose();
		return true;
	}
}
