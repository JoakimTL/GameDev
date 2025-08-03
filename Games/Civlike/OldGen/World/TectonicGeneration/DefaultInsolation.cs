//namespace OldGen.World.TectonicGeneration;

//public static class DefaultInsolation {
//	public static float GetDailyMeanInsolation( InsolationProvider provider, Latitude latitude ) {
//		float sinLat = latitude.Sin;
//		float cosLat = latitude.Cos;
//		float tanLat = latitude.Tan;
//		float cosH0 = -tanLat * provider.TanDelta;
//		cosH0 = float.Clamp( cosH0, -1, 1 );
//		float h0 = MathF.Acos( cosH0 );
//		float dailyMean = provider.DailyMeanPreprocessConstant * (h0 * sinLat * provider.SinDelta + cosLat * provider.CosDelta * MathF.Sin( h0 ));
//		dailyMean = MathF.Max( dailyMean, 0 );
//		return dailyMean;
//	}
//}