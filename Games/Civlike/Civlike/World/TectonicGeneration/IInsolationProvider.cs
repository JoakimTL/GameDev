using Civlike.World.GenerationState;

namespace Civlike.World.TectonicGeneration;

public interface IInsolationProvider {
	void Preprocess( double daysSimulated );
	/// <summary>
	/// Calculates the daily mean insolation for a given face based on its latitude and the current solar declination.
	/// </summary>
	/// <returns>W/m²</returns>
	float GetDailyMeanInsolation( FaceBase face );
	float GetDailyMeanInsolation( float latRad );
}