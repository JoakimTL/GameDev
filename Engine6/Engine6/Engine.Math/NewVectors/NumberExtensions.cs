using System.Numerics;

namespace Engine.Math.NewVectors;
public static class NumberExtensions {
	public static char SignCharacter<T>( this T number )
		where T :
			unmanaged, INumber<T> 
		=> T.Sign( number ) >= 0 ? '+' : '-';
}
