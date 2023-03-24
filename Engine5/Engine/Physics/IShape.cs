using System.Numerics;

namespace Engine.Physics;
public interface IShape
{
    event Action<IShape>? PhysicalPropertiesChanged;
    double Mass { get; }
    Matrix4x4 InertiaTensor { get; }
}

public interface IShape<V> : IShape
{
    V GetFurthest(V direction);
}