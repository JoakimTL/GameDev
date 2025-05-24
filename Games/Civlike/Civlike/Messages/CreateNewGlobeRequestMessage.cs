using Civlike.World.GameplayState;
using Civlike.World.GenerationState;

namespace Civlike.Messages;

//public sealed class CreateWorldMessage {
//	public double SurfaceArea { get; }
//	public uint SubdivisionCount { get; }

//	public CreateWorldMessage( double surfaceArea, uint subdivisionCount ) {
//		if (subdivisionCount < 7)
//			throw new ArgumentOutOfRangeException( nameof( subdivisionCount ), "Subdivision count must be at least 7." );
//		if (subdivisionCount > 12)
//			throw new ArgumentOutOfRangeException( nameof( subdivisionCount ), "Subdivision count must be at most 12." );
//		if (surfaceArea <= 38000000)
//			throw new ArgumentOutOfRangeException( nameof( surfaceArea ), "Surface area must be greater than 38000000." );
//		SurfaceArea = surfaceArea;
//		SubdivisionCount = subdivisionCount;
//	}
//}

public sealed record CreateNewGlobeRequestMessage<TGlobeType, TParameter>( TParameter Parameters, Type[]? StepsToIgnore = null ) where TGlobeType : GeneratingGlobeBase, new() where TParameter : GlobeGeneratorParameterBase;
