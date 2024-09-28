
using Engine;
using Engine.Modules.Rendering;
using System.Linq;
using UserTest;
using UserTest.Pathfinding;

//ReadOnlySpan<Vector2<float>> points = [ (0, 0), (1, 0), (0, 1), (3, 3), (-1, 2), (-3, 3), (-6, -6), (7, 6), 8, -1 ];

Span<Vector2<float>> points = stackalloc Vector2<float>[ 32 ];

Random r = new Random( 2 );

for (int i = 0; i < points.Length; i++) {
	points[ i ] = new Vector2<float>( r.NextSingle() * 2 - 1, r.NextSingle() * 2 - 1 );
}

Span<uint> order = stackalloc uint[ points.Length ];


Vector2<float> center = points.Average<Vector2<float>, float>(); 
Span<float> determinants = stackalloc float[ points.Length * points.Length ];

for (int i = 0; i < points.Length; i++) {
	for (int j = 0; j < points.Length; j++) {
		determinants[ i * points.Length + j ] = points[ i ].DeterminantWithOrigin( center, points[ j ] ) * ((points[ i ] - center).Normalize<Vector2<float>, float>().Dot( (points[ j ] - center).Normalize<Vector2<float>, float>() ) + 1);
	}
}

uint count = points.OrderClockwise( order );

Span<Vector2<float>> vectors = stackalloc Vector2<float>[ (int) count ];

for (int i = 0; i < count; i++) {
	vectors[ i ] = points[ (int) order[ i ] ];
}

Span<uint> remaining = stackalloc uint[ points.Length ];
uint edgeVertexCount = vectors.FormOuterEdges( remaining, true );

Span<Vector2<float>> edgeVertices = stackalloc Vector2<float>[ (int) edgeVertexCount ];
for (int i = 0; i < edgeVertexCount; i++) {
	edgeVertices[ i ] = vectors[ (int) remaining[ i ] ];
}

Span<Vector2<float>> normalizedVectors = stackalloc Vector2<float>[ (int) count ];

for (int i = 0; i < count; i++) {
	normalizedVectors[ i ] = vectors[ i ].MagnitudeSquared() > 0 ? vectors[ i ].Normalize<Vector2<float>, float>() : vectors[ i ];
}

Span<Vector2<float>> diffs = stackalloc Vector2<float>[ (int) count ];

for (int i = 0; i < count; i++) {
	diffs[ i ] = (normalizedVectors[ (i + 1) % (int) count ] - normalizedVectors[ i ]).Normalize<Vector2<float>, float>();
}

Span<float> angles = stackalloc float[ (int) count ];

for (int i = 0; i < count; i++) {
	angles[ i ] = MathF.Atan2( normalizedVectors[ i ].Y, normalizedVectors[ i ].X ) + MathF.PI;
}

Span<float> angleDiffs = stackalloc float[ (int) count ];

for (int i = 0; i < count; i++) {
	angleDiffs[ i ] = MathF.Atan2( diffs[ i ].Y, diffs[ i ].X );
}

for (int i = 0; i < count; i++) {
	Console.Write( $"Point {i}: " );
	for (int j = 0; j < count; j++) {
		Console.Write( $"{determinants[ i * points.Length + j ]:0.00}, " );
	}
	Console.WriteLine();
}

Console.WriteLine($"Original  (Sorted order: {string.Join(", ", order.ToArray() )})");
int size = 50;
for (int y = -size; y <= size; y++) {
	for (int x = -size; x <= size; x++) {
		int vertIndex = -1;
		for (int i = 0; i < count; i++) {
			int xI = (int) MathF.Round( points[ i ].X * size, 0, MidpointRounding.ToZero );
			int yI = (int) MathF.Round( points[ i ].Y * size, 0, MidpointRounding.ToZero );
			if (xI == x && yI == y) {
				vertIndex = i;
				break;
			}
		}
		{
			int xI = (int) MathF.Round( center.X * size, 0, MidpointRounding.ToZero );
			int yI = (int) MathF.Round( center.Y * size, 0, MidpointRounding.ToZero );
			if (xI == x && yI == y) {
				vertIndex = -2;
			}
		}
		Console.Write( vertIndex != -1 ? vertIndex == -2 ? "ce" : vertIndex.ToString( "00" ) : "--" );
	}
	Console.WriteLine();
}
Console.WriteLine();
Console.WriteLine( "Sorted" );
for (int y = -size; y <= size; y++) {
	for (int x = -size; x <= size; x++) {
		int vertIndex = -1;
		for (int i = 0; i < count; i++) {
			int xI = (int) MathF.Round( vectors[ i ].X * size, 0, MidpointRounding.ToZero );
			int yI = (int) MathF.Round( vectors[ i ].Y * size, 0, MidpointRounding.ToZero );
			if (xI == x && yI == y) {
				vertIndex = i;
				break;
			}
		}
		{
			int xI = (int) MathF.Round( center.X * size, 0, MidpointRounding.ToZero );
			int yI = (int) MathF.Round( center.Y * size, 0, MidpointRounding.ToZero );
			if (xI == x && yI == y) {
				vertIndex = -2;
			}
		}
		Console.Write( vertIndex != -1 ? vertIndex == -2 ? "ce" : vertIndex.ToString( "00" ) : "--" );
	}
	Console.WriteLine();
}
Console.WriteLine( $"Edges {edgeVertexCount}" );
for (int y = -size; y <= size; y++) {
	for (int x = -size; x <= size; x++) {
		int vertIndex = -1;
		for (int i = 0; i < edgeVertexCount; i++) {
			int xI = (int) MathF.Round( edgeVertices[ i ].X * size, 0, MidpointRounding.ToZero );
			int yI = (int) MathF.Round( edgeVertices[ i ].Y * size, 0, MidpointRounding.ToZero );
			if (xI == x && yI == y) {
				vertIndex = i;
				break;
			}
		}
		{
			int xI = (int) MathF.Round( center.X * size, 0, MidpointRounding.ToZero );
			int yI = (int) MathF.Round( center.Y * size, 0, MidpointRounding.ToZero );
			if (xI == x && yI == y) {
				vertIndex = -2;
			}
		}
		Console.Write( vertIndex != -1 ? vertIndex == -2 ? "ce" : vertIndex.ToString( "00" ) : "--" );
	}
	Console.WriteLine();
}

Console.WriteLine( count );
//Startup.StartModule<DefaultRenderModule>();
