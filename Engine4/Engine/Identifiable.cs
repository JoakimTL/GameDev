namespace Engine;

public abstract class Identifiable {
	/// <summary>
	/// Unique Id
	/// </summary>
	public ulong ID { get; }
	/// <summary>
	/// Type name
	/// </summary>
	public string TypeName { get; }
	public Guid Guid { get; private set; }
	/// <summary>
	/// Personalized name.
	/// </summary>
	public string Name { get; private set; }
	protected virtual string UniqueNameTag => string.Empty;
	public string FullName => $"{this.TypeName}/{this.Name}:{this.ID}{( !string.IsNullOrEmpty( this.UniqueNameTag ) ? $"[{this.UniqueNameTag}]" : string.Empty )}";

	public bool HasName => this._nameSet;

	private bool _nameSet;

	public Identifiable() {
		this.ID = UID64.Next;
		Type type = GetType();
		this.TypeName = type.ReflectedType?.Name ?? type.Name;
		this.Name = string.Empty;
		this._nameSet = false;
	}

	public Identifiable( string name ) : this() {
		this.Name = name;
		this._nameSet = true;
	}

	public Identifiable( string name, Guid guid ) : this( name ) {
		this.Guid = guid;
	}

	/// <summary>
	/// Sets the name of the object. Can only be set once.
	/// </summary>
	/// <param name="name">The name given.</param>
	public void SetName( string name ) {
		if ( string.IsNullOrEmpty( name ) || this._nameSet )
			return;
		this.Name = name;
		this._nameSet = true;
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

	protected void SetGuid( Guid newGuid ) => this.Guid = newGuid;
}
