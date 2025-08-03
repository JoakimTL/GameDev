namespace OldGen.World.TectonicGeneration.Parameters;

//public sealed class EndMemberThermalProperties {
//	public ThermalProperties DrySoil { get; } = new( 1600, 1200, 0.5 );
//	public ThermalProperties PoreWater { get; } = new( 100, 4186, 0.6 );
//	public ThermalProperties Rock { get; } = new( 2700, 800, 2.5 );
//	public ThermalProperties Seawater { get; } = new( 1025, 3993, 0.6 );
//	public ThermalProperties Water { get; }
//}

public sealed class EndMemberProperties {
	/// <summary>
	/// Air properties:<br/>
	/// Density: 1.293 kg/m³<br/>
	/// Thermal capacity: 1005 J/(kg·K)<br/>
	/// Thermal conductivity: 0.023 W/(m·K)<br/>
	/// Emissivity: 0.2<br/> 
	/// Albedo: 0<br/>
	/// Minimum Roughness: 0m<br/>
	/// Maximum Roughness: 0m<br/>
	/// </summary>
	public PhysicalProperties Air { get; } = new( 1.293, 1005, 0.023, 0.2, 0, 0, 0 );
	/// <summary>
	/// Dry soil properties:<br/>
	/// Density: 1600 kg/m³<br/>
	/// Thermal capacity: 1200 J/(kg·K)<br/>
	/// Thermal conductivity: 0.5 W/(m·K)<br/>
	/// Emissivity: 0.92<br/>
	/// Albedo: 0.2<br/>
	/// Minimum Roughness: 0.001m<br/>
	/// Maximum Roughness: 0.05m<br/>
	/// </summary>
	public PhysicalProperties DrySoil { get; } = new( 1600, 1200, 0.5, 0.92, 0.2, 0.001, 0.05 );
	/// <summary>
	/// Water properties:<br/>
	/// Density: 1000 kg/m³<br/>
	/// Thermal capacity: 4200 J/(kg·K)<br/>
	/// Thermal conductivity: 0.54 W/(m·K)<br/>
	/// Emissivity: 0.98<br/>
	/// Albedo: 0.06<br/>
	/// Minimum Roughness: 0.001m<br/>
	/// Maximum Roughness: 2m<br/>
	/// </summary>
	public PhysicalProperties Water { get; } = new( 1000, 4200, 0.54, 0.98, 0.06, 0.001, 2 );
	/// <summary>
	/// Vegetation properties:<br/>
	/// Density: 400 kg/m³<br/>
	/// Thermal capacity: 1300 J/(kg·K)<br/>
	/// Thermal conductivity: 0.15 W/(m·K)<br/>
	/// Emissivity: 0.98<br/>
	/// Albedo: 0.15<br/>
	/// Minimum Roughness: 0.5m<br/>
	/// Maximum Roughness: 2.5m<br/>
	/// </summary>
	public PhysicalProperties Vegetation { get; } = new( 400, 1300, 0.15, 0.98, 0.15, 0.5, 2.5 );
	/// <summary>
	/// Snow properties:<br/>
	/// Density: 500 kg/m³<br/>
	/// Thermal capacity: 4200 J/(kg·K)<br/>
	/// Thermal conductivity: 0.1 W/(m·K)<br/>
	/// Emissivity: 0.98<br/>
	/// Albedo: 0.9<br/>
	/// Minimum Roughness: 0.001m<br/>
	/// Maximum Roughness: 0.01m<br/>
	/// </summary>
	public PhysicalProperties Snow { get; } = new( 500, 4200, 0.1, 0.98, 0.9, 0.001, 0.01 );
	/// <summary>
	/// Ice properties:<br/>
	/// Density: 916.8 kg/m³<br/>
	/// Thermal capacity: 4200 J/(kg·K)<br/>
	/// Thermal conductivity: 2.22 W/(m·K)<br/>
	/// Emissivity: 0.98<br/>
	/// Albedo: 0.5<br/>
	/// Minimum Roughness: 0.0001m<br/>
	/// Maximum Roughness: 0.001m<br/>
	/// </summary>
	public PhysicalProperties Ice { get; } = new( 916.8, 4200, 2.22, 0.98, 0.5, 0.0001, 0.001 );
}