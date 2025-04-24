//using Engine.Module.Entities.Container;
//using Engine.Module.Render.Entities;
//using Engine.Standard.Entities.Components;
//using Engine.Transforms.Models;

//namespace Engine.Standard.Render.Entities.Behaviours;

//public sealed class Transform2Behaviour : SynchronizedRenderBehaviourBase<Transform2Archetype> {

//	private readonly Transform2<double> _highPrecisionTransform;
//	private readonly Transform2<float> _transform;

//	private TransformData<Vector2<double>, double, Vector2<double>> _incomingHighPrecisionTransformData;
//	private TransformData<Vector2<float>, float, Vector2<float>> _incomingTransformData;

//	public TransformReadonly<float, Vector2<float>, float, Vector2<float>> Transform { get; }
//	public TransformReadonly<double, Vector2<double>, double, Vector2<double>> HighPrecisionTransform { get; }

//	public Transform2Behaviour() {
//		this._transform = new();
//		this._highPrecisionTransform = new();
//		this.Transform = this._transform.Readonly;
//		this.HighPrecisionTransform = this._highPrecisionTransform.Readonly;
//	}

//	protected override void OnRenderEntitySet() {
//		base.OnRenderEntitySet();
//		Transform2Component t2c = Archetype.Transform2Component;
//		_incomingHighPrecisionTransformData = t2c.Transform.Data;
//		_incomingTransformData = new(
//			_incomingHighPrecisionTransformData.Translation.CastSaturating<double, float>(),
//			float.CreateSaturating( _incomingHighPrecisionTransformData.Rotation ),
//			_incomingHighPrecisionTransformData.Scale.CastSaturating<double, float>() );
//	}

//	protected override void OnUpdate( double time, double deltaTime ) {
//	}

//	protected override bool PrepareSynchronization( ComponentBase component ) {
//		if (component is Transform2Component t2c) {
//			_incomingHighPrecisionTransformData = t2c.Transform.Data;
//			_incomingTransformData = new(
//				_incomingHighPrecisionTransformData.Translation.CastSaturating<double, float>(),
//				float.CreateSaturating( _incomingHighPrecisionTransformData.Rotation ),
//				_incomingHighPrecisionTransformData.Scale.CastSaturating<double, float>() );
//			return true;
//		}
//		return false;
//	}

//	protected override void Synchronize() {
//		_highPrecisionTransform.Translation = _incomingHighPrecisionTransformData.Translation;
//		_highPrecisionTransform.Rotation = _incomingHighPrecisionTransformData.Rotation;
//		_highPrecisionTransform.Scale = _incomingHighPrecisionTransformData.Scale;
//		_transform.Translation = _incomingTransformData.Translation;
//		_transform.Rotation = _incomingTransformData.Rotation;
//		_transform.Scale = _incomingTransformData.Scale;
//	}
//}
