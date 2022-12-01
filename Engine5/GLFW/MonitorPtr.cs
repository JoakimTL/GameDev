namespace GLFW;

/// <summary>
///     Wrapper around a pointer to monitor.
/// </summary>
/// <seealso cref="MonitorPtr" />
[System.Runtime.InteropServices.StructLayout( System.Runtime.InteropServices.LayoutKind.Sequential )]
public struct MonitorPtr : IEquatable<MonitorPtr> {
	/// <summary>
	///     Represents a <c>null</c> value for a <see cref="MonitorPtr" /> object.
	/// </summary>
	public static readonly MonitorPtr None;

	/// <summary>
	///     Internal pointer.
	/// </summary>
	private readonly nint _handle;

	/// <summary>
	///     Determines whether the specified <see cref="MonitorPtr" />, is equal to this instance.
	/// </summary>
	/// <param name="other">The <see cref="MonitorPtr" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public bool Equals( MonitorPtr other ) => this._handle.Equals( other._handle );

	/// <summary>
	///     Determines whether the specified <see cref="object" />, is equal to this instance.
	/// </summary>
	/// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals( object? obj ) {
		if ( obj is MonitorPtr monitor )
			return Equals( monitor );
		return false;
	}

	/// <summary>
	///     Returns a hash code for this instance.
	/// </summary>
	/// <returns>
	///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	/// </returns>
	public override int GetHashCode() => HashCode.Combine( this._handle );

	/// <summary>
	///     Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator ==( MonitorPtr left, MonitorPtr right ) => left.Equals( right );

	/// <summary>
	///     Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	///     The result of the operator.
	/// </returns>
	public static bool operator !=( MonitorPtr left, MonitorPtr right ) => !left.Equals( right );

	/// <summary>
	///     Returns a <see cref="string" /> that represents this instance.
	/// </summary>
	/// <returns>
	///     A <see cref="string" /> that represents this instance.
	/// </returns>
	public override string ToString() => this._handle.ToString();

	/// <summary>
	///     Gets the position, in screen coordinates of the valid work are for the monitor.
	/// </summary>
	/// <seealso cref="Glfw.GetMonitorWorkArea" />

	public System.Drawing.Rectangle WorkArea {
		get {
			Glfw.GetMonitorWorkArea( this._handle, out int x, out int y, out int width, out int height );
			return new System.Drawing.Rectangle( x, y, width, height );
		}
	}

	/// <summary>
	///     Gets the content scale of this monitor.
	///     <para>The content scale is the ratio between the current DPI and the platform's default DPI.</para>
	/// </summary>
	/// <seealso cref="Glfw.GetMonitorContentScale" />

	public System.Numerics.Vector2 ContentScale {
		get {
			Glfw.GetMonitorContentScale( this._handle, out float x, out float y );
			return new System.Numerics.Vector2( x, y );
		}
	}

	/// <summary>
	///     Gets or sets a user-defined pointer to associate with the window.
	/// </summary>
	/// <seealso cref="Glfw.GetMonitorUserPointer" />
	/// <seealso cref="Glfw.SetMonitorUserPointer" />
	public nint UserPointer {
		get => Glfw.GetMonitorUserPointer( this._handle );
		set => Glfw.SetMonitorUserPointer( this._handle, value );
	}
}
