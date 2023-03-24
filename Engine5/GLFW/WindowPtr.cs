namespace GLFW;

/// <summary>
///     Wrapper around a GLFW window pointer.
/// </summary>
[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public struct WindowPtr : IEquatable<WindowPtr> {
	/// <summary>
	///     Describes a default/null instance.
	/// </summary>
	public static readonly WindowPtr None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint handle;

	/// <summary>
	///     Performs an implicit conversion from <see cref="WindowPtr" /> to <see cref="nint" />.
	/// </summary>
	/// <param name="window">The window.</param>
	/// <returns>
	///     The result of the conversion.
	/// </returns>
	public static implicit operator nint( WindowPtr window ) { return window.handle; }

	/// <summary>
	///     Performs an explicit conversion from <see cref="nint"/> to <see cref="WindowPtr"/>.
	/// </summary>
	/// <param name="handle">A pointer representing the window handle.</param>
	/// <returns>The result of the conversion.</returns>
	public static explicit operator WindowPtr( nint handle ) => new( handle );

	/// <summary>
	/// Creates a new instance of the <see cref="WindowPtr"/> struct.
	/// </summary>
	/// <param name="handle">A pointer representing the window handle.</param>
	public WindowPtr( nint handle ) {
		this.handle = handle;
	}

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => handle.ToString();

	/// <summary>
	///     Determines whether the specified <see cref="WindowPtr" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="WindowPtr" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="WindowPtr" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( WindowPtr other ) => handle.Equals( other.handle );

	/// <summary>
	///     Determines whether the specified <see cref="System.object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="System.object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is WindowPtr window )
			return Equals( window );
		return false;
	}

	/// <summary>
	///     Gets or sets the opacity of the window in the range of <c>0.0</c> and <c>1.0</c> inclusive.
	/// </summary>
	public float Opacity {
		get => Glfw.GetWindowOpacity( handle );
		set => Glfw.SetWindowOpacity( handle, Math.Min( 1.0f, Math.Max( 0.0f, value ) ) );
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => handle.GetHashCode();

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( WindowPtr left, WindowPtr right ) { return left.Equals( right ); }

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( WindowPtr left, WindowPtr right ) { return !left.Equals( right ); }
}
