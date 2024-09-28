using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data;
public sealed class Box<T>( T value ) {
	public T Value { get; set; } = value;

	public static implicit operator T( Box<T> box ) => box.Value;
	public static implicit operator Box<T>( T value ) => new( value );
	public override string ToString() => $"Box[{Value?.ToString()}]";
	public override bool Equals( object? obj ) => (obj is Box<T> box && this == box) || (obj is T t && Equals( Value, t ));
	public override int GetHashCode() => HashCode.Combine( Value );
	public static bool operator ==( Box<T> left, Box<T> right ) => Equals( left.Value, right.Value );
	public static bool operator !=( Box<T> left, Box<T> right ) => !(left == right);
}
