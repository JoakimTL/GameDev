namespace Civlike.World.TectonicGeneration.Parameters.Old;

public sealed class SoilDepthGenerationConstants {
	/// <summary>
	/// Reference maximum soil depth on flat, wet terrain.<br/>
	/// Defined in meters.
	/// </summary>
	public double MaxSoilDepth { get; set; } = 2;
	/// <summary>
	/// Controls exponential decay of soil depth with terrain gradient<br/>
	/// Defined as unitless constant.
	/// </summary>
	public double SlopeDecayConstant { get; set; } = 3;
	/// <summary>
	/// Amplitude of fractal noise perturbation for local variation<br/>
	/// Defined as unitless constant.
	/// </summary>
	public double SoilNoiseAmplitude { get; set; } = 0.1;
	/// <summary>
	/// Base soil porosity, representing the fraction of soil volume that is pore space<br/>
	/// Defined as unitless constant, typically around 0.4 for mineral soils.
	/// </summary>
	public double SoilPorosityBase { get; set; } = 0.4;
	/// <summary>
	/// Amplitude of fractal noise perturbation for soil porosity<br/>
	/// Defined as unitless constant.
	/// </summary>
	public double SoilPorosityNoiseAmplitude { get; set; } = 0.1;
}
