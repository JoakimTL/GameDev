namespace Engine;

public sealed class DisposedObjectException : Exception {

	public DisposedObjectException( DisposableIdentifiable di ) : base( $"{di} has already been disposed!" ) {

	}

}
