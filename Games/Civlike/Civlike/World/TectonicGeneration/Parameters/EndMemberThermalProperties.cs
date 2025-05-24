namespace Civlike.World.TectonicGeneration.Parameters;

public sealed class EndMemberThermalProperties {
	public ThermalProperties DrySoil { get; } = new( 1600, 1200, 0.5 );
	public ThermalProperties PoreWater { get; } = new( 100, 4186, 0.6 );
	public ThermalProperties Rock { get; } = new( 2700, 800, 2.5 );
	public ThermalProperties Seawater { get; } = new( 1025, 3993, 0.6 );
}
