using Engine.Datatypes;
using System.Numerics;

namespace Engine.Physics.D3T;

//public interface ICollisionShape<V> where V : System.Numerics.

public class Face
{
    public Box<Vector3> A, B, C;

    public override string ToString() => $"[{A.Uid},{B.Uid},{C.Uid}] {A.Value}, {B.Value}, {C.Value}";
}
