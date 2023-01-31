namespace Engine.Physics;

public readonly struct Face<V> where V : struct
{
    public readonly V A;
    public readonly V B;
    public readonly V C;

    public Edge<V> AB => new(A, B);
    public Edge<V> BC => new(B, C);
    public Edge<V> CA => new(C, A);

    public Face(V a, V b, V c)
    {
        A = a;
        B = b;
        C = c;
    }
}
