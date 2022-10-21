using Engine.Structure.Uid;

namespace Engine;
public class Identifiable {

	/// <summary>
	/// Unique Id
	/// </summary>
	public ulong ID { get; }
	/// <summary>
	/// Type name
	/// </summary>
	public string TypeName {
		get {
			Type type = GetType();
			return type.ReflectedType?.Name ?? type.Name;
		}
	}
	/// <summary>
	/// Personalized name.
	/// </summary>
	public string Name { get; private set; }
	public string FullName => $"{this.TypeName}/{this.Name}:{this.ID}{( !string.IsNullOrEmpty( this.UniqueNameTag ) ? $"[{this.UniqueNameTag}]" : string.Empty )}";
	protected virtual string UniqueNameTag => string.Empty;

	public Identifiable() {
		this.ID = UID64.Next;
		this.Name = string.Empty;
	}

	public Identifiable( string name ) : this() {
		this.Name = name;
	}

	/// <summary>
	/// Sets the name of the object. Can only be set once.
	/// </summary>
	/// <param name="name">The name given.</param>
	protected void SetName( string name ) {
		if ( string.IsNullOrEmpty( name ) || !string.IsNullOrEmpty( this.Name ) )
			return;
		this.Name = name;
	}

	public override string ToString() => this.FullName;

	public override int GetHashCode() => this.ID.GetHashCode();

	public override bool Equals( object? obj ) {
		if ( obj is Identifiable i )
			return i.ID == this.ID;
		return false;
	}

	public static bool operator ==( Identifiable? l, Identifiable? r ) {
		if ( l is null )
			return r is null;
		return l.Equals( r );
	}

	public static bool operator !=( Identifiable? l, Identifiable? r ) => !( l == r );

}
