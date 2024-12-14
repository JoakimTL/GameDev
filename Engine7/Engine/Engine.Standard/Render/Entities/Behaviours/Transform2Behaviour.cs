using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Standard.Entities.Components;
using Engine.Transforms.Models;

namespace Engine.Standard.Render.Entities.Behaviours;

public sealed class Transform2Behaviour : SynchronizedRenderBehaviourBase<Transform2Archetype> {

	private readonly Transform2<float> _transform;

	private Vector2<float> _incomingTranslation;
	private float _incomingRotation;
	private Vector2<float> _incomingScale;

	public TransformReadonly<float, Vector2<float>, float, Vector2<float>> Transform { get; }
	public Transform2Behaviour() {
		this._transform = new();
		this.Transform = this._transform.Readonly;
	}

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
		Transform2Component t2c = Archetype.Transform2Component;
		this._incomingTranslation = t2c.Transform.Translation.CastSaturating<double, float>();
		this._incomingRotation = float.CreateSaturating( t2c.Transform.Rotation );
		this._incomingScale = t2c.Transform.Scale.CastSaturating<double, float>();
	}

	protected override void OnUpdate( double time, double deltaTime ) {
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is Transform2Component t2c) {
			this._incomingTranslation = t2c.Transform.Translation.CastSaturating<double, float>();
			this._incomingRotation = float.CreateSaturating( t2c.Transform.Rotation );
			this._incomingScale = t2c.Transform.Scale.CastSaturating<double, float>();
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		_transform.Translation = _incomingTranslation;
		_transform.Rotation = _incomingRotation;
		_transform.Scale = _incomingScale;
	}
}
