namespace OldGen.World;

public readonly struct Pressure( float pascal ) {
	public readonly float Pascal = pascal;

	public float Bar => this.Pascal / 100000f;
	public float Atmosphere => this.Pascal / 101325f;
	public override int GetHashCode() => this.Pascal.GetHashCode();
	public override bool Equals( object? obj ) => obj is Pressure pressure && pressure == this;
	public override string ToString() => $"{this.Bar:N2} bar";

	public static Pressure FromBar( float bar ) => new( bar * 100000f );
	public static Pressure FromAtmosphere( float atmosphere ) => new( atmosphere * 101325f );

	public static implicit operator Pressure( float pascal ) => new( pascal );
	public static implicit operator Pressure( double pascal ) => new( (float) pascal );
	public static implicit operator float( Pressure pressure ) => pressure.Pascal;

	public static bool operator ==( Pressure left, Pressure right ) => left.Pascal == right.Pascal;
	public static bool operator !=( Pressure left, Pressure right ) => !(left == right);
}