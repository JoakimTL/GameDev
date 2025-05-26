using System.Globalization;
using System.Numerics;

namespace Engine;
public static class NumberExtensions {
	internal static string ToFormattedString<T>( this T number, bool isFirstNumber = false )
		where T :
			unmanaged, INumber<T> {
		if (T.IsNaN( number ))
			return "NaN";
		int sign = T.Sign( number );
		string numberString = number.ToString( "#,##0.###", CultureInfo.InvariantCulture )[ (sign >= 0 ? 0 : 1).. ];
		return isFirstNumber ? $"{(sign < 0 ? "-" : "")}{numberString}" : $"{(sign >= 0 ? "+" : "-")} {numberString}";
	}

	/// <typeparam name="T">The type which the frequency is supplied.</typeparam>
	/// <param name="frequency">The frequency of the oscilation in Hz.</param>
	/// <returns>Returns the oscilation period of input <paramref name="frequency"/> in milliseconds. If the period is lower than 1ms it will return 1ms.</returns>
	/// <exception cref="Exception">If the <paramref name="frequency"/> is less than <see cref="IFloatingPointIeee754{T}.Epsilon"/>.</exception>"
	public static uint ToPeriodMs<T>( this T frequency, uint lowestPossiblePeriodMs = 0, uint highestPossiblePeriodMs = uint.MaxValue )
		where T :
			unmanaged, IFloatingPointIeee754<T>
		=> frequency > T.Epsilon
			? uint.Clamp( uint.CreateSaturating( T.CreateSaturating( 1000 ) / frequency ), lowestPossiblePeriodMs, highestPossiblePeriodMs )
			: throw new Exception( $"Frequency must be greater than {T.Epsilon}." );

	/// <typeparam name="T">The type which the frequency is supplied.</typeparam>
	/// <param name="frequency">The frequency of the oscilation in Hz.</param>
	/// <param name="maxFrequency">The mininum allowed <paramref name="frequency"/>. Frequencies below this number are clamped up.</param>
	/// <param name="minFrequency">The maximum allowed <paramref name="frequency"/>. Frequencies above this number are clamped down.</param>
	/// <returns>Returns the oscilation period of input <paramref name="frequency"/> in milliseconds clamped between the <paramref name="minFrequency"/> and <paramref name="maxFrequency"/>. If the period is lower than 1ms it will return 1ms.</returns>
	/// <exception cref="ArgumentException">If the <paramref name="maxFrequency"/> is less than <paramref name="minFrequency"/>.</exception>""
	public static uint ToPeriodMs<T>( this T frequency, T minFrequency, T maxFrequency )
		where T :
			unmanaged, IFloatingPointIeee754<T>
		=> maxFrequency >= minFrequency
			? uint.Max( uint.CreateSaturating( T.CreateSaturating( 1000 ) / T.Clamp( frequency, minFrequency, maxFrequency ) ), 1 )
			: throw new ArgumentException( "Max frequency must be greater than or equal to min frequency." );

	/// <typeparam name="T">The output frequenct type</typeparam>
	/// <param name="periodMs">The period of oscilation in milliseconds</param>
	/// <returns>The frequency of the input oscilation period in Hz.</returns>
	/// <exception cref="ArgumentException">If the <paramref name="periodMs"/> is 0 ms.</exception>
	public static T ToFrequency<T>( this uint periodMs )
		where T :
			unmanaged, IFloatingPointIeee754<T>
		=> periodMs > 0
			? T.CreateSaturating( T.CreateSaturating( 1000 ) / T.CreateSaturating( periodMs ) )
			: throw new ArgumentException( "Period must be greater than zero." );
}
