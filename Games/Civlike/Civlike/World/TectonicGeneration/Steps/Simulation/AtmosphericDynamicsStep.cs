using Civlike.World.GameplayState;
using Civlike.World.GenerationState;
using Civlike.World.TectonicGeneration.Parameters;
using Engine;
using System;
using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace Civlike.World.TectonicGeneration.Steps.Simulation;

/*
 * Momentum balance terms
 * - Pressure-gradient force (from your height/pressure field)
 * - Coriolis acceleration (with local f=2Ωsin⁡φf=2Ωsinφ on each tile)
 * - Non-linear advection (u⋅∇uu⋅∇u) to cap runaway speeds and spawn eddies
 * - Quadratic surface drag (Fdrag=−Cd ∣u∣uFdrag​=−Cd​∣u∣u) rather than a simple linear sink
 * - Horizontal diffusion (ν∇2uν∇2u or biharmonic) to smooth grid‐scale noise
 * Tile geometry & projection
 * - Compute each triangle’s outward normal nn
 * - Solve in 3D but enforce “no-through-flow”: u⋅n=0u⋅n=0
 * - Project uu onto the local tangent plane:
 *   - utan=u−(u⋅n)n
 * Continuity / mass conservation
 * - For a single layer: enforce ∇⋅(H utan)=0∇⋅(Hutan​)=0 (or diagnose vertical velocity ww)
 * - If you hold layer depth HH fixed, you’re in a barotropic regime—vertical ww is purely a by-product
 * Boundary-layer closure
 * - Ekman‐layer or vertical‐mixing parameterization to turn and slow the lowest-level wind
 * - Yields realistic cross-isobar flow and reduces purely geostrophic extremes
 * Thermal / thickness forcing
 * - Prescribe or solve for horizontal gradients in layer thickness or geopotential to drive jets and trades
 * - Even a simple latitudinal profile of “effective” gHgH gives realistic belts
 * Numerical solution strategy
 * - Form your steady 2×2 (horizontal) momentum balance on each tile
 * - Solve algebraically (e.g. Newton–Raphson) for uu given local forcing and drag
 * - Optionally iterate with neighbor exchanges for advection and diffusion coupling
 * Parameter choices & scaling
 * - Drag coefficient CdCd​ depends on surface type (land vs. sea)
 * - Eddy viscosity νν tuned to your tile size (smaller tiles need smaller νν)
 * - Vertical mixing length ≃10–50 m for the lowest model level
 * Diagnostics & sanity checks
 * - Check geostrophic limit: u≈1f k^×∇pu≈f1​k^×∇p when drag ≪ Coriolis
 * - Verify maximum wind speeds are O(10 m/s) not O(100 m/s) on Earth-like setups
 * - Inspect convergence: ensure your “steady” solve actually reaches a residual ≪1 %
 * Use of vertical velocity
 * - Treat w=(u⋅n)w=(u⋅n) as a diagnostic only (e.g. for dust/cloud schemes)
 * - Do not re-inject ww into your 2D momentum unless you upgrade to a full 3D solver
 * Extension paths (optional)
 * - Add multi-layer or shallow-water with thickness evolution for baroclinic effects
 * - Couple to temperature/advection for seasonal or diurnal cycles
*/
public sealed class AtmosphericDynamicsStep : ISimulationStep {
	public void Process( TectonicGeneratingGlobe globe, TectonicGlobeParameters parameters, double daysSimulated, double secondsToSimulate ) {
		AtmosphericDynamicsParameters adp = globe.AtmosphericDynamicsParameters;

		float linearDragCoefficient = (float) adp.LinearFrictionCoefficient; // Linear drag coefficient
		float quadraticDragCoefficient = (float) adp.QuadraticFrictionCoefficient; // Quadratic drag coefficient
		float minimumQuadraticDragCoefficient = (float) adp.MinimumQuadraticFrictionCoefficient; // Minimum quadratic drag coefficient
		float coriolisStrength = (float) adp.CoriolisStrength; // Coriolis strength factor
		float pressureGradientCoefficient = -(float) adp.PressureGradientCoefficient; // Pressure gradient coefficient

		Vector3<float> upAxis = Vector3<float>.UnitY;

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				//for (int i = 0; i < globe.TectonicFaces.Count; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				Vector3<float> n = face.CenterNormalized;

				Vector3<float> pressureGradient = PhysicsHelpers.GetPressureGradient( face, state );
				Vector3<float> pressureWind = pressureGradientCoefficient * pressureGradient;

				Vector3<float> coriolis = PhysicsHelpers.ApplyCoriolis( pressureWind, upAxis, state.CoriolisFactor );

				Vector3<float> windBeforeDrag = pressureWind + coriolis + state.HadleyWinds;

				Vector3<float> windVector = PhysicsHelpers.ApplyDrag( windBeforeDrag, linearDragCoefficient, quadraticDragCoefficient );

				state.Wind = windVector;
				state.TangentialWind = windVector - windVector.Dot( n ) * n;
			}
		} );
	}
}