using System.Numerics;

namespace Engine;

public static class Vector2Extensions {
	public static Vector3<TScalar> ToCartesianFromPolar<TScalar>( in this Vector2<TScalar> polar, TScalar radius )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar> {
		TScalar sinY = TScalar.Sin( polar.Y );
		TScalar x = radius * sinY * TScalar.Cos( polar.X );
		TScalar y = radius * TScalar.Cos( polar.Y );
		TScalar z = radius * sinY * TScalar.Sin( polar.X );
		return new( x, y, z );
	}

	public static uint DiscardDuplicates<TScalar>( this ReadOnlySpan<Vector2<TScalar>> vertices, in Span<uint> remaining )
		where TScalar :
			unmanaged, INumber<TScalar> {
		int currentId = 0;
		for (int i = 0; i < vertices.Length; i++) {
			bool overlapping = false;
			Vector2<TScalar> a = vertices[ i ];
			for (int j = i + 1; j < vertices.Length; j++)
				if (vertices[ j ] == a) {
					overlapping = true;
					break;
				}
			if (!overlapping)
				remaining[ currentId++ ] = (uint) i;
		}
		return (uint) currentId;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TScalar"></typeparam>
	/// <param name="vertices"></param>
	/// <param name="order">The order needed to form a shape where travel between vertices is clockwise around the center of all the points</param>
	/// <returns>Number of indices in the order span.</returns>
	public static uint OrderClockwise<TScalar>( this ReadOnlySpan<Vector2<TScalar>> vertices, in Span<uint> order, bool hasNoDuplicates = false )
	where TScalar :
		unmanaged, IFloatingPointIeee754<TScalar> {
		if (order.Length != vertices.Length)
			return 0;
		if (vertices.Length < 3)
			return 0;

		uint workingVerticesCount = (uint) vertices.Length;
		Span<uint> remaining = stackalloc uint[ vertices.Length ];
		if (!hasNoDuplicates)
			workingVerticesCount = DiscardDuplicates( vertices, remaining );
		else
			for (int i = 0; i < vertices.Length; i++)
				remaining[ i ] = (uint) i;

		Span<Vector2<TScalar>> workingVertices = stackalloc Vector2<TScalar>[ (int) workingVerticesCount ];
		for (int i = 0; i < workingVerticesCount; i++)
			workingVertices[ i ] = vertices[ (int) remaining[ i ] ];

		Span<(Vector2<TScalar> vertex, int inputOrder)> sortedSpan = stackalloc (Vector2<TScalar>, int)[ workingVertices.Length ];
		for (int i = 0; i < workingVertices.Length; i++)
			sortedSpan[ i ] = (workingVertices[ i ], i);

		Vector2<TScalar> center = workingVertices.Average<Vector2<TScalar>, TScalar>();

		for (int i = 0; i < sortedSpan.Length - 1; i++) {
			TScalar highest = TScalar.Zero;
			int highestIndex = i;
			for (int j = i + 1; j < sortedSpan.Length; j++) {
				Vector2<TScalar> a = sortedSpan[ i ].vertex;
				Vector2<TScalar> b = sortedSpan[ j ].vertex;
				TScalar determinant = a.DeterminantWithOrigin( center, b ) > TScalar.Zero ? TScalar.One : -TScalar.One;
				if (!a.TryNormalizedDotWithOrigin( center, b, out TScalar dot ))
					continue;
				TScalar value = determinant + dot;
				if (value <= highest && highestIndex != i)
					continue;
				highestIndex = j;
				highest = value;
			}

			if (highestIndex == i)
				continue;
			(sortedSpan[ i + 1 ], sortedSpan[ highestIndex ]) = (sortedSpan[ highestIndex ], sortedSpan[ i + 1 ]);
		}

		for (int i = 0; i < workingVertices.Length; i++)
			order[ i ] = (uint) sortedSpan[ i ].inputOrder;

		return workingVerticesCount;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TScalar"></typeparam>
	/// <param name="vertices"></param>
	/// <param name="order">The order needed to form a shape where travel between vertices is clockwise around the center of all the points</param>
	/// <returns>Number of indices in the order span.</returns>
	public static uint OrderClockwise<TScalar>( this Span<Vector2<TScalar>> vertices, in Span<uint> order )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> OrderClockwise( (ReadOnlySpan<Vector2<TScalar>>) vertices, order );

	/// <summary>
	/// Returns the order of indices needed to form a shape consisting of vertices on the outer edges of the input vertices.
	/// </summary>
	/// <param name="vertices"></param>
	/// <param name="remaining">The remaining points</param>
	/// <returns>Number of indices in the remaining span.</returns>
	public static uint FormOuterEdges<TScalar>( this ReadOnlySpan<Vector2<TScalar>> vertices, in Span<uint> remaining, bool verticesInClockwiseOrder = false )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar> {

		uint workingVerticesCount = (uint) vertices.Length;
		Span<uint> clockwiseOrder = stackalloc uint[ vertices.Length ];
		if (!verticesInClockwiseOrder)
			workingVerticesCount = OrderClockwise( vertices, clockwiseOrder );
		else
			for (int i = 0; i < workingVerticesCount; i++)
				clockwiseOrder[ i ] = (uint) i;

		Span<Vector2<TScalar>> workingVertices = stackalloc Vector2<TScalar>[ (int) workingVerticesCount ];
		for (int i = 0; i < workingVerticesCount; i++)
			workingVertices[ i ] = vertices[ (int) clockwiseOrder[ i ] ];

		Vector2<TScalar> center = workingVertices.Average<Vector2<TScalar>, TScalar>();

		int currentKeptVertexIndex = 0;
		int startIndex = -1;
		{
			TScalar dist = TScalar.Zero;
			for (int i = 0; i < workingVertices.Length; i++) {
				TScalar currentDist = (workingVertices[ i ] - center).MagnitudeSquared();
				if (currentDist > dist) {
					dist = currentDist;
					startIndex = i;
				}
			}
		}
		int currentIndex = startIndex;

		remaining[ currentKeptVertexIndex++ ] = (uint) currentIndex;

		//Only retain the vertices on the outer edges of the shape.
		for (int iteration = 0; iteration < workingVertices.Length; iteration++) {
			int i = currentIndex;
			int i2 = (i + 1) % workingVertices.Length;

			Vector2<TScalar> a = workingVertices[ i ];
			Vector2<TScalar> b = workingVertices[ i2 ];

			for (int j = 2; j < workingVertices.Length; j++) {
				int i3 = (i + j) % workingVertices.Length;
				Vector2<TScalar> c = workingVertices[ i3 ];

				TScalar det = b.DeterminantWithOrigin( a, c );
				if (det < TScalar.Zero) {
					i2 = i3;
					b = c;
				}
				if (i3 == startIndex)
					break;
			}

			if (i2 == startIndex)
				break;
			iteration += i2 - i;
			currentIndex = i2;
			remaining[ currentKeptVertexIndex++ ] = (uint) currentIndex;
		}

		return (uint) currentKeptVertexIndex;
	}
	/// <summary>
	/// Returns the order of indices needed to form a shape consisting of vertices on the outer edges of the input vertices.
	/// </summary>
	/// <param name="vertices"></param>
	/// <param name="remaining">The remaining points</param>
	/// <returns>Number of indices in the remaining span.</returns>
	public static uint FormOuterEdges<TScalar>( this Span<Vector2<TScalar>> vertices, in Span<uint> remaining, bool verticesInClockwiseOrder = false )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar>
		=> FormOuterEdges( (ReadOnlySpan<Vector2<TScalar>>) vertices, remaining, verticesInClockwiseOrder );


	public static Vector2 ToNumerics( in this Vector2<float> vector )
		=> new( vector.X, vector.Y );

	public static Vector2 ToNumerics( in this Vector2<double> vector )
		=> new( (float) vector.X, (float) vector.Y );

	public static Vector2<TScalar> FromNumerics<TScalar>( in this Vector2 vector )
		where TScalar :
			unmanaged, INumber<TScalar>
		=> new( TScalar.CreateSaturating( vector.X ), TScalar.CreateSaturating( vector.Y ) );
	//
	/*
	 * (a - o) * X | (c - o) * X
	 * (b - p) * Y | (d - p) * Y
	 * 
	 * ((a - o) * X) * ((d - p) * Y) - ((b - p) * Y) * ((c - o) * X)
	 * 
	 * ((a - o) * (d - p)) * XY - ((b - p) * (c - o)) * YX
	 * ((a - o) * (d - p)) * XY + ((b - p) * (c - o)) * XY
	 * ((a - o) * (d - p) + (b - p) * (c - o)) * XY
	 * (ad - ap - od + op + bc - bo - pc + po) * XY
	 * (ad + bc - ap - od - bo - pc + 2op) * XY
	 * (ad + bc - o * (d + b) - p * (a + p) + 2op) * XY
	 * 
	 * (a - o) * X | (b - p) * Y
	 * (c - o) * X | (d - p) * Y
	 * 
	 * ((a - o) * X) * ((d - p) * Y) - ((c - o) * X) * ((b - p) * Y)
	 * 
	 * ((a - o) * (d - p)) * XY - ((c - o) * (b - p)) * XY
	 * ((ad - ap - od + op) - (cb - cp - ob + op)) * XY
	 * (ad - ap - od + op - cb + cp + ob - op) * XY 
	 * (ad - ap - od - cb + cp + ob) * XY
	 * (x(a) * y(b) - x(a) * y(o) - x(o) * y(b) - x(b) * y(a) + x(b) * y(o) + x(o) * y(a)) * XY
	 */
	//

	/// <summary>
	/// 
	/// </summary>
	/// <param name="uHat">The unit vector which forms the plane.</param>
	/// <returns></returns>
	public static Vector3<TScalar> RotateToPlane<TScalar>( this in Vector2<TScalar> q, in Vector3<TScalar> uHat )
		where TScalar :
			unmanaged, IFloatingPointIeee754<TScalar> {
		if (TScalar.One + uHat.Z <= TScalar.Epsilon)
			return new Vector3<TScalar>( -q.X, -q.Y, TScalar.Zero);
		TScalar ux = uHat.X, uy = uHat.Y, uz = uHat.Z;
		TScalar x = q.X, y = q.Y;
		TScalar c = uz;
		TScalar alpha = TScalar.One / (TScalar.One + c);

		return new Vector3<TScalar>(
			(c + alpha * uy * uy) * x - alpha * ux * uy * y,
			-(alpha * ux * uy) * x + (c + alpha * ux * ux) * y,
			-(ux * x + uy * y)
		);
	}
}
