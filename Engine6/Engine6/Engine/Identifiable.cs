using System.Diagnostics;

namespace Engine;

public class Identifiable {

	/// <summary>
	/// Unique Id
	/// </summary>
	public ulong Uid { get; }
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
	public string IdentifiableName { get; private set; }
	protected virtual string ExtraInformation => string.Empty;
	public string FullName => $"{this.TypeName}/{this.IdentifiableName}:{ExtraInformation}:uid{this.Uid}";

	public Identifiable() {
		this.Uid = Uid64.Next;
		this.IdentifiableName = string.Empty;
	}

	public Identifiable( string name ) : this() {
		this.IdentifiableName = name;
	}

	/// <summary>
	/// Sets the name of the object. Can only be set once.
	/// </summary>
	/// <param name="name">The name given.</param>
	protected void SetIdentifiableName( string name ) {
		if ( string.IsNullOrEmpty( name ) || !string.IsNullOrEmpty( this.IdentifiableName ) )
			return;
		this.IdentifiableName = name;
	}

	public override string ToString() => this.FullName;

	public override int GetHashCode() => this.Uid.GetHashCode();

	public override bool Equals( object? obj ) {
		if ( obj is Identifiable i )
			return i.Uid == this.Uid;
		return false;
	}

	public static bool operator ==( Identifiable? l, Identifiable? r ) {
		if ( l is null )
			return r is null;
		return l.Equals( r );
	}

	public static bool operator !=( Identifiable? l, Identifiable? r ) => !( l == r );

}

public abstract class DisposableIdentifiable : Identifiable, IDisposable {

	public bool Disposed { get; private set; } = false;

	/// <summary>
	/// Invoked after diposal is complete.
	/// </summary>
	public event Action? OnDisposed;

	~DisposableIdentifiable() {
		if (!Disposed)
			Debug.Fail( $"Object \"{this.FullName}\" was not disposed before destruction!" );
	}

	public void Dispose() {
		if (Disposed)
			return;
		if (InternalDispose()) {
			Disposed = true;
			OnDisposed?.Invoke();
		}
		GC.SuppressFinalize( this );
	}

	/// <returns>True if the object was fully disposed. False if there are still undisposed parts.</returns>
	protected abstract bool InternalDispose();
}