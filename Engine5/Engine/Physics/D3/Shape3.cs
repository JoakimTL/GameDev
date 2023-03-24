using System.Numerics;

namespace Engine.Physics.D3;
public class Shape3 : IShape
{
    public double Mass => throw new NotImplementedException();

    public Matrix4x4 InertiaTensor => throw new NotImplementedException();

    public event Action<IShape>? PhysicalPropertiesChanged;
}

/// <summary>
/// A collection of tetrahedrons that fills the volume of a convex shape.
/// </summary>
public class ConvexShape3 : IShape
{
    public ConvexShape3(double mass)
    {

    }



    public double Mass => throw new NotImplementedException();

    public Matrix4x4 InertiaTensor => throw new NotImplementedException();

    public event Action<IShape>? PhysicalPropertiesChanged;
}

//Shape -> A combination of convex hulls in specific locations
//Hull -> A collection of vertices, this is handled as if they are the vertices of a convex shape.
