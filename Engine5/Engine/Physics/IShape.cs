using System.Numerics;

namespace Engine.Physics;
public interface IShape<V>
{

    V GetFurthest(V direction);

    float Mass { get; }
    Matrix4x4 InertiaTensor { get; }

}