using Engine.Structure.Uid;

namespace Engine;
public class Identifiable {

    /// <summary>
    /// Unique Id
    /// </summary>
    public ulong Uid { get; }
    /// <summary>
    /// Type name
    /// </summary>
    public string TypeName
    {
        get
        {
            Type type = GetType();
            return type.ReflectedType?.Name ?? type.Name;
        }
    }
    /// <summary>
    /// Personalized name.
    /// </summary>
    public string IdentifiableName { get; private set; }
    public string FullName => $"{TypeName}/{IdentifiableName}:uid{Uid}{(!string.IsNullOrEmpty(UniqueNameTag) ? $"({UniqueNameTag})" : string.Empty)}";
    protected virtual string UniqueNameTag => string.Empty;

    public Identifiable()
    {
        Uid = UID64.Next;
        IdentifiableName = string.Empty;
    }

    public Identifiable(string name) : this()
    {
        IdentifiableName = name;
    }

    /// <summary>
    /// Sets the name of the object. Can only be set once.
    /// </summary>
    /// <param name="name">The name given.</param>
    protected void SetIdentifiableName(string name)
    {
        if (string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(IdentifiableName))
            return;
        IdentifiableName = name;
    }

    public override string ToString() => FullName;

    public override int GetHashCode() => Uid.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is Identifiable i)
            return i.Uid == Uid;
        return false;
    }

    public static bool operator ==(Identifiable? l, Identifiable? r)
    {
        if (l is null)
            return r is null;
        return l.Equals(r);
    }

    public static bool operator !=(Identifiable? l, Identifiable? r) => !(l == r);

}
