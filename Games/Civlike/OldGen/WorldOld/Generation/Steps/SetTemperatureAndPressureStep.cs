//TODO: Have tiles be outside the ECS systems. Incorporate them into the ECS system (and thus rendering) as tile collection components: Lists of tiles. These lists are gained from the octree in the globe class.

using Engine;

namespace OldGen.WorldOld.Generation.Steps;

public sealed class SetTemperatureAndPressureStep : ITerrainGenerationProcessingStep {
	public string ProcessingStepMessage => "Setting temperature and pressure";

	public void Process( WorldGenerationParameters parameters, GlobeModel globe ) {
		const float P0 = 101325.0f;   // sea-level standard pressure, Pa
		const float g = 9.80665f;    // gravity, m/s²
		const float M = 0.0289644f;    // molar mass of dry air, kg/mol
		const float R0 = 8.3144598f;    // universal gas constant, J/(mol·K)

		float equatorTemperature = 40f;    // °C at 0° latitude
		float poleTemperature = -25f;   // °C at ±90° latitude
		float elevationTemperatureLapseRate = -6.5f; // °C/km
		float obliquityRad = (float) parameters.ObliquityDegrees * float.Pi / 180f; // radians
		float revolutionsPerOrbit = (float) parameters.RevolutionsPerOrbit;
		float seasonalAmp = 15f;

		float basePressure = 101325f; //Standard pressure at sea level in Pa
		float pressurePerMeter = 12f; //Pressure delta per meter elevation change in Pa
		float pressurePerKelvin = -100f; //Pressure delta per Kelvin temperature change in Pa

		ParallelProcessing.ForFaces( globe, 1, ( start, end, taskId ) => {
			for (int i = start; i < end; i++) {
				Face face = globe.Faces[ i ];
				Vector3<float> center = (face.Blueprint.VectorA + face.Blueprint.VectorB + face.Blueprint.VectorC) / 3;

				float latitude = MathF.Asin( center.Y );

				float normLat = float.Abs( latitude ) / (float.Pi / 2);

				float tempCelsius = float.Lerp( equatorTemperature, poleTemperature, normLat );

				float elevationKm = face.State.PressureHeight / 1000f;

				float seasonalTemp = 0;
				for (float d = 0; d < revolutionsPerOrbit; d++) {
					float dayAngle = 2 * float.Pi * d / revolutionsPerOrbit;
					Vector3<float> sunDir = new Vector3<float>(
						float.Cos( dayAngle ),
						float.Sin( dayAngle ) * float.Sin( obliquityRad ),
						float.Sin( dayAngle ) * float.Cos( obliquityRad )
					).Normalize<Vector3<float>, float>();
					float insolation = center.Dot( sunDir );
					seasonalTemp += seasonalTemp * insolation * seasonalAmp;
				}

				float temperature = tempCelsius + elevationKm * elevationTemperatureLapseRate + seasonalTemp;

				face.State.SetTemperature( Temperature.FromCelsius( float.Lerp( equatorTemperature, poleTemperature, normLat ) ) );

				face.State.SetStaticPressure( P0 * float.Exp( -(g * face.State.PressureHeight * M) / (R0 * face.State.Temperature.Kelvin) ) );
				face.State.SetBaseDynamicPressure( basePressure + face.State.PressureHeight * pressurePerMeter + face.State.Temperature.Kelvin * pressurePerKelvin );
				face.State.SetDynamicPressure( basePressure + face.State.PressureHeight * pressurePerMeter + face.State.Temperature.Kelvin * pressurePerKelvin );
			}
		} );
	}
}
