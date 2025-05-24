namespace Civlike.World;

public readonly struct Temperature( float kelvin ) {
	public readonly float Kelvin = kelvin;

	public float Celsius => this.Kelvin - 273.15f;
	public float Fahrenheit => this.Celsius * 9 / 5 + 32;

	public override int GetHashCode() => this.Kelvin.GetHashCode();
	public override bool Equals( object? obj ) => obj is Temperature temperature && temperature == this;
	public override string ToString() => $"{this.Celsius:N2} °C";

	public static Temperature FromCelsius( float celsius ) => new( celsius + 273.15f );
	public static Temperature FromFahrenheit( float fahrenheit ) => new( (fahrenheit - 32) * 5 / 9 + 273.15f );

	public static implicit operator Temperature( float kelvin ) => new( kelvin );
	public static implicit operator Temperature( double kelvin ) => new( (float) kelvin );
	public static implicit operator float( Temperature temperature ) => temperature.Kelvin;
	public static bool operator ==( Temperature left, Temperature right ) => left.Kelvin == right.Kelvin;
	public static bool operator !=( Temperature left, Temperature right ) => !(left == right);
}
