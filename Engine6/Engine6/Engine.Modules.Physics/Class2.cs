using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Modules.Physics;
internal class Class2 {
}

public readonly struct Line2 {
	public readonly Vector2 A;
	public readonly Vector2 B;

	public Line2( Vector2 a, Vector2 b ) {
		A = a;
		B = b;
	}

	public static Vector2? GetIntersectionPoint(Line2 l, Line2 r ) {
		Vector2 l_ab = l.B - l.A;
		Vector2 r_ab = r.B - r.A;

		return null;


	}
}

public readonly struct Triangle2 {
	public readonly Vector2 A;
	public readonly Vector2 B;
	public readonly Vector2 C;

	public Triangle2( Vector2 a, Vector2 b, Vector2 c ) {
		A = a;
		B = b;
		C = c;
	}
}

public readonly struct Line3 {
	public readonly Vector3 A;
	public readonly Vector3 B;

	public Line3( Vector3 a, Vector3 b ) {
		A = a;
		B = b;
	}
}
public readonly struct Triangle3 {
	public readonly Vector3 A;
	public readonly Vector3 B;
	public readonly Vector3 C;

	public Triangle3( Vector3 a, Vector3 b, Vector3 c ) {
		A = a;
		B = b;
		C = c;
	}
}

public readonly struct Tertahedron {

	public readonly Vector3 A;
	public readonly Vector3 B;
	public readonly Vector3 C;
	public readonly Vector3 D;

	public Tertahedron( Vector3 a, Vector3 b, Vector3 c, Vector3 d ) {
		A = a;
		B = b;
		C = c;
		D = d;
	}
}

public readonly struct IndexTriangle {

	public readonly int A;
	public readonly int B;
	public readonly int C;

	public IndexTriangle( int a, int b, int c ) {
		A = a;
		B = b;
		C = c;
	}
}

public readonly struct IndexTertahedron {

	public readonly int A;
	public readonly int B;
	public readonly int C;
	public readonly int D;

	public IndexTertahedron( int a, int b, int c, int d ) {
		A = a;
		B = b;
		C = c;
		D = d;
	}
}