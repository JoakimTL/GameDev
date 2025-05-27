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
		var adp = globe.AtmosphericDynamicsParameters;

		float Ω = 2f * float.Pi / (float) globe.PlanetaryConstants.RotationPeriod; // Angular velocity in rad/s
		float dt = (float) secondsToSimulate; // Time step in seconds

		float linearDragCoefficient = (float) adp.LinearFrictionCoefficient; // Linear drag coefficient
		float quadraticDragCoefficient = (float) adp.QuadraticFrictionCoefficient; // Quadratic drag coefficient
		float minimumQuadraticDragCoefficient = (float) adp.MinimumQuadraticFrictionCoefficient; // Minimum quadratic drag coefficient

		float tileLength = (float) globe.TileLength; // Length of a tile in meters
		float circleArea = tileLength * tileLength * float.Pi; // Area of a tile in m²
		Vector3<float> upAxis = Vector3<float>.UnitY;

		ParallelProcessing.Range( globe.TectonicFaces.Count, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				//for (int i = 0; i < globe.TectonicFaces.Count; i++) {
				Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				Vector3<float> n = face.CenterNormalized;
				float latitude = face.LatitudeRads;

				Vector3<float> hadleyWind = GetHadleyComponent( n, upAxis, face, adp );

				Vector3<float> pressureGradient = GetPressureGradient( face, state, tileLength );
				Vector3<float> pressureWind = -adp.PressureGradientCoefficient * pressureGradient;

				//Vector3<float> orographicLift = GetOrographicLifting( n, face, tileLength, adp );

				//Vector3<float> baseWind = hadleyWind + pressureWind /*+ orographicLift*/;

				var baseWind = hadleyWind + pressureWind;

				Vector3<float> coriolis = ApplyCoriolis( baseWind, upAxis, state.CoriolisFactor );

				Vector3<float> windWithCoriolis = baseWind + coriolis;

				var windVector = ApplyDrag( windWithCoriolis, linearDragCoefficient, quadraticDragCoefficient, minimumQuadraticDragCoefficient );

				state.Wind = windVector;
				state.TangentialWind = windVector - windVector.Dot( n ) * n;
			}
		} );
	}

	private Vector3<float> GetPressureGradient( Face<TectonicFaceState> face, TectonicFaceState state, float tileLength ) {
		Vector3<float> pressureGradient = Vector3<float>.Zero;
		foreach (NeighbouringFace nbr in face.Neighbours) {
			Face<TectonicFaceState> nbrFace = nbr.Face as Face<TectonicFaceState> ?? throw new InvalidCastException( $"Neighbour face at index {nbr.Face.Id} is not of type TectonicFaceState." );
			TectonicFaceState nbrState = nbrFace.State;
			pressureGradient += (nbrState.Pressure - state.Pressure) * nbr.NormalizedDirection / tileLength;
		}
		return pressureGradient;
	}

	private Vector3<float> GetHadleyComponent( Vector3<float> normal, Vector3<float> upAxis, Face<TectonicFaceState> face, AtmosphericDynamicsParameters adp ) {
		float φ0 = adp.HadleyCellLatitudeWidth; // in radians
		float A = adp.HadleyStrength; // m/s
		var lat = face.LatitudeRads; // in radians
		float sin = float.Sin( lat / φ0 * float.Pi );
		float cos = float.Cos( lat / φ0 * float.Pi );

		//var weakeningFactor = MathF.Exp( -(lat * lat) / (φ0 * φ0) );
		Vector3<float> sideways = upAxis.Cross( normal );

		Vector3<float> hadleyWind = A * (normal * cos + sideways * sin);
		return hadleyWind;
	}

	//private Vector3<float> GetOrographicLifting( Vector3<float> normal, Face<TectonicFaceState> face, float tileLength, AtmosphericDynamicsParameters adp ) {
	//	float liftingStrength = adp.OrographicLiftingStrength;

	//	Vector3<float> orographicLift = Vector3<float>.Zero;
	//	foreach (NeighbouringFace nbr in face.Neighbours) {
	//		Face<TectonicFaceState> nbrFace = nbr.Face as Face<TectonicFaceState>;
	//		float dh = nbrFace.State.Elevation - face.State.Elevation;
	//		if (dh > 0) {
	//			Vector3<float> gradient = dh * nbr.NormalizedDirection / tileLength;
	//			orographicLift += gradient;
	//		}
	//	}

	//	return liftingStrength * orographicLift.Project( normal );
	//}

	private Vector3<float> ApplyCoriolis( Vector3<float> velocity, Vector3<float> upAxis, float f ) {
		return upAxis.Cross( velocity ) / float.CopySign( float.Max( float.Abs( f ), 1e-6f ), f );
	}

	private Vector3<float> ApplyDrag( Vector3<float> velocity, float linCoefficient, float quadCoefficient, float minQuadCoefficient ) {
		float speed = velocity.Magnitude<Vector3<float>, float>();
		float decay = 1f / (1f + linCoefficient + quadCoefficient * speed);
		return velocity * decay;
	}
}

/*Vector3<float> n = face.CenterNormalized;
				Vector3<float> e = upAxis.Cross( n );
				Vector3<float> t = n.Cross( e );

				Vector3<float> pressureGradient = GetPressureGradient( face, state, tileLength );
				float rho = state.Pressure / ((float) globe.UniversalConstants.SpecificGasConstant * state.AirTemperature.Kelvin);

				Vector3<float> G3 = -(1f / rho) * pressureGradient;
				Vector2<float> G = (G3.Dot( e ), G3.Dot( t ));

				float fCoriolis = 2f * Ω * n.Y;

				Matrix2x2<float> Alin = new(
					linearDragCoefficient, -fCoriolis,
					fCoriolis, linearDragCoefficient
				);

				if (!Alin.TryGetInverse( out Matrix2x2<float> invAlin ))
					throw new InvalidOperationException( "Failed to invert matrix Alin." );

				var uG = invAlin * G;
				float speedG = uG.Magnitude<Vector2<float>, float>();

				float C_effective = float.Max( quadraticDragCoefficient * speedG, minimumQuadraticDragCoefficient );

				Matrix2x2<float> Aeff = new(
					C_effective, -fCoriolis,
					fCoriolis, C_effective
				);

				if (!Aeff.TryGetInverse( out Matrix2x2<float> invAeff ))
					throw new InvalidOperationException( "Failed to invert matrix Aeff." );

				Vector2<float> u2 = invAeff * G;

				Vector3<float> windVector = u2.X * e + u2.Y * t;

				state.Wind = windVector;
				state.TangentialWind = windVector - windVector.Dot( n ) * n;

/*
 * Face<TectonicFaceState> face = globe.TectonicFaces[ i ];
				TectonicFaceState state = face.State;

				Vector3<float> n = face.Center;
				Vector3<float> e = upAxis.Cross( n );
				Vector3<float> t = n.Cross( e );

				Vector3<float> pressureGradient = GetPressureGradient( face, state, tileLength );
				float rho = state.Pressure / ((float) globe.UniversalConstants.SpecificGasConstant * state.AirTemperature.Kelvin);

				Vector3<float> G3 = -(1f / rho) * pressureGradient;
				float Gmag = G3.Magnitude<Vector3<float>, float>();
				//Vector2<float> G = (G3.Dot( e ), G3.Dot( t ));

				float fCoriolis = 2f * Ω * n.Y; // Coriolis factor

				float disc = fCoriolis * fCoriolis + 4 * quadraticDragCoefficient * Gmag;
				float uMag = (-fCoriolis + float.Sqrt( disc )) / (2 * quadraticDragCoefficient);

				var dir = Gmag > 1e-8f ? G3 / Gmag : Vector3<float>.Zero;

				//float alpha = linearDragCoefficient;
				//Matrix2x2<float> A = new(
				//	alpha, -fCoriolis,
				//	fCoriolis, alpha
				//);

				//if (!A.TryGetInverse( out Matrix2x2<float> Ainv ))
				//	throw new InvalidOperationException( "Failed to invert matrix A." );

				//Vector2<float> u2 = Ainv * G; // Wind vector in the tangent plane

				//Vector3<float> windVector = u2.X * e + u2.Y * t;//Need to solve this.

				var windVector = uMag * dir; // Wind vector in 3D space

				state.Wind = windVector;
				state.TangentialWind = windVector - windVector.Dot( n ) * n;
 */