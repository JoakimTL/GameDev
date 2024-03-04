using System.Diagnostics;

namespace Engine.OpenGL.OOP;

public abstract class OglObjectBase : IDisposable {
	public bool Disposed { get; private set; } = false;
	public string Nickname { get; set; } = "";
	protected abstract string DisplayName { get; }

	~OglObjectBase() {
		if (!Disposed)
			Debug.Fail( $"OpenGL object \"{DisplayName}\" was not disposed before destruction!" );
	}

	public void Dispose() {
		if (Disposed)
			return;
		if (InternalDispose())
			Disposed = true;
		GC.SuppressFinalize( this );
	}

	/// <returns>True if the object was fully disposed. False if there are still undisposed parts.</returns>
	protected abstract bool InternalDispose();

	public override string ToString() => DisplayName;
}
