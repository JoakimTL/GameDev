namespace Engine.Physics;

public readonly struct Edge<V> where V : struct
{
    public readonly V A;
    public readonly V B;

    public Edge(V a, V b)
    {
        A = a;
        B = b;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Edge<V> edge)
            return false;
        return (A.Equals(edge.A) && B.Equals(edge.B)) || (A.Equals(edge.B) && B.Equals(edge.A));
    }

    public static bool operator ==(Edge<V> left, Edge<V> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Edge<V> left, Edge<V> right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A.GetHashCode(), B.GetHashCode());
    }
}
