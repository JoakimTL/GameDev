using Engine.Datatypes;
using System.Numerics;

namespace Engine.Physics.D3T;

public class Tetrahedron {
	public Box<Vector3> A, B, C, P;

	public Vector3 Centroid => ( A.Value + B.Value + C.Value + P.Value ) / 4;

	public Tetrahedron( Box<Vector3> a, Box<Vector3> b, Box<Vector3> c, Box<Vector3> p ) {
		A = a;
		B = b;
		C = c;
		P = p;
	}

	//V = (1/6) * |(p1 - p4) * ((p2 - p4) × (p3 - p4))|
	public float Volume => 1f / 6 * MathF.Abs( Vector3.Dot( A.Value - P.Value, Vector3.Cross( B.Value - A.Value, C.Value - A.Value ) ) );

	/*
		∑[V_T * (r^2 * I_3 - r * r^T)]
	 */

	public Matrix4x4 GetInertiaTensor( Vector3 centerOfMass ) {
		var P1 = A.Value - centerOfMass;
		var P2 = B.Value - centerOfMass;
		var P3 = C.Value - centerOfMass;
		var P4 = P.Value - centerOfMass;
		var P11 = P1 * P1;
		var P22 = P2 * P2;
		var P33 = P3 * P3;
		var P44 = P4 * P4;
		var P12 = P1 * P2;
		var P13 = P1 * P3;
		var P14 = P1 * P4;
		var P23 = P2 * P3;
		var P24 = P2 * P4;
		var P34 = P3 * P4;

		//	1 | 0 | 0 | 0
		//	1 | 1 | 0 | 0
		//	1 | 1 | 1 | 0
		//	1 | 1 | 1 | 1

		var det = 6 * Volume;
		var xx = det / 60 * (
			P11.Y +
			P12.Y + P22.Y +
			P13.Y + P23.Y + P33.Y +
			P14.Y + P24.Y + P34.Y + P44.Y +
			P11.Z +
			P12.Z + P22.Z +
			P13.Z + P23.Z + P33.Z +
			P14.Z + P24.Z + P34.Z + P44.Z
		);
		var yy = det / 60 * (
			P11.X +
			P12.X + P22.X +
			P13.X + P23.X + P33.X +
			P14.X + P24.X + P34.X + P44.X +
			P11.Z +
			P12.Z + P22.Z +
			P13.Z + P23.Z + P33.Z +
			P14.Z + P24.Z + P34.Z + P44.Z
		);
		var zz = det / 60 * (
			P11.X +
			P12.X + P22.X +
			P13.X + P23.X + P33.X +
			P14.X + P24.X + P34.X + P44.X +
			P11.Y +
			P12.Y + P22.Y +
			P13.Y + P23.Y + P33.Y +
			P14.Y + P24.Y + P34.Y + P44.Y
		);
		var yz = -det / 120 * (
			(( (2 * P1.Y) + P2.Y + P3.Y + P4.Y ) * P1.Z) +
			(( P1.Y + (2 * P2.Y) + P3.Y + P4.Y ) * P2.Z) +
			(( P1.Y + P2.Y + (2 * P3.Y) + P4.Y ) * P3.Z) +
			(( P1.Y + P2.Y + P3.Y + (2 * P4.Y) ) * P4.Z) );
		var xz = -det / 120 * (
			(( (2 * P1.X) + P2.X + P3.X + P4.X ) * P1.Z) +
			(( P1.X + (2 * P2.X) + P3.X + P4.X ) * P2.Z) +
			(( P1.X + P2.X + (2 * P3.X) + P4.X ) * P3.Z) +
			(( P1.X + P2.X + P3.X + (2 * P4.X) ) * P4.Z) );
		var xy = -det / 120 * (
			(( (2 * P1.X) + P2.X + P3.X + P4.X ) * P1.Y) +
			(( P1.X + (2 * P2.X) + P3.X + P4.X ) * P2.Y) +
			(( P1.X + P2.X + (2 * P3.X) + P4.X ) * P3.Y) +
			(( P1.X + P2.X + P3.X + (2 * P4.X) ) * P4.Y) );
		//xz and xy might need to swap positions in this matrix
		return new Matrix4x4(
			xx, xz, xy, 0,
			xz, yy, yz, 0,
			xy, yz, zz, 0,
			0, 0, 0, 0
		);
	}
}
