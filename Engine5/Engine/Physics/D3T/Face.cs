using Engine.Datatypes;
using System.Numerics;

namespace Engine.Physics.D3T;

//public interface ICollisionShape<V> where V : System.Numerics.

public class Face {
	public Box<Vector3> A, B, C;

	public override string ToString() {
		return $"[{A.ID},{B.ID},{C.ID}] {A.Value}, {B.Value}, {C.Value}";
	}
}
