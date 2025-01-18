namespace Sandbox.Logic.World.Tiles;

public readonly struct TriangleIndices( int a, int b, int c ) {
	public int A { get; } = a;
	public int B { get; } = b;
	public int C { get; } = c;
}
