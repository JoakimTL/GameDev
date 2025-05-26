using Civlike.World.GenerationState;
using Engine;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

public sealed class AtmosphericDynamicsStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
		float omega = 2f * float.Pi / (float) globe.PlanetaryConstants.RotationPeriod; // Angular velocity in rad/s
		float drag = (float) globe.AtmosphericDynamicsParameters.FrictionCoefficient; // Wind drag coefficient
		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				Vector3<float> center = face.Center;
				TectonicFaceState state = face.State;

				Vector3<float> gradP = Vector3<float>.Zero;
				foreach (NeighbouringFace nbr in face.Neighbours) {
					Face<TectonicFaceState> nbrFace = nbr.Face as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Neighbour face at index {nbr.Face.Id} is not of type TectonicFaceState." );
					TectonicFaceState nbrState = nbrFace.State;
					gradP += (nbrState.Pressure - state.Pressure) * nbr.NormalizedDirection * (float) globe.ApproximateTileLength;
				}
				gradP /= (float) globe.TileArea;

				float f = float.CopySign( float.Max( float.Abs( 2f * omega * face.Center.Y ), 1e-2f ), face.Center.Y );

				float rho = state.Pressure / ((float) globe.UniversalConstants.SpecificGasConstant * state.AirTemperature.Kelvin);

				Vector3<float> vGeost = center.Cross( gradP ) * (1f / (rho * f));
				vGeost -= vGeost.Dot( center ) * center;

				state.Wind = (1f - drag) * state.Wind + drag * vGeost;
			}
		} );
	}
}
