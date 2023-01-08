using Engine.Datatypes;
using System.Numerics;

namespace Engine.Physics.D3T;

public class Edge {
	public Box<Vector3> A, B;

	public Edge( Box<Vector3> a, Box<Vector3> b ) {
		A = a;
		B = b;
	}

	public override bool Equals( object? obj ) {
		if ( obj is not Edge e )
			return false;

		return e.A == A && e.B == B || e.B == A && e.A == B;
	}

	public override int GetHashCode() {
		return A.GetHashCode() + B.GetHashCode();
	}
	public override string ToString() {
		return $"[{A.ID},{B.ID}] {A.Value}, {B.Value}";
	}
}
