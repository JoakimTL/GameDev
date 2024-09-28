using Engine.Data.Transforms;
using System.Numerics;

namespace Engine.Standard.ECS.Components;

public interface ITransformComponent<TScalar, TTranslation, TRotation, TScale>
	where TScalar :
		unmanaged, INumber<TScalar>
	where TTranslation :
		unmanaged
	where TRotation :
		unmanaged
	where TScale :
		unmanaged {
	TransformInterface<TScalar, TTranslation, TRotation, TScale> Transform { get; }
}