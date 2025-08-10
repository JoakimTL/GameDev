using Civlike.World.State;
using Civlike.World.TectonicGeneration.Landscape;
using Engine;

namespace Civlike.World.TectonicGeneration;

public sealed class TectonicGlobeGenerator : IGlobeGenerator {
	public int Subdivisions => 7;

	public double Radius => 6378000;

	private List<IGlobeGenerationProcessingStep> _processingSteps;

	public TectonicGlobeGenerator( TectonicGenerationParameters parameters ) {
		_processingSteps = [];

		Engine.Structures.TypeDigraph<IGlobeGenerationProcessingStep> typeDigraph = new();

		foreach (Type? step in TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( TectonicGlobeGenerationProcessingStepBase ) ) )) {
			typeDigraph.Add( step );
		}

		IReadOnlyList<Type> stepsInOrder = typeDigraph.GetTypes();

		foreach (Type step in stepsInOrder) {
			IGlobeGenerationProcessingStep stepInstance = step.CreateInstance( [ parameters ] ) as IGlobeGenerationProcessingStep ?? throw new Exception( $"Failed to create instance of {step}" );
			_processingSteps.Add( stepInstance );
		}
	}

	public void GenerateInitialGlobeState( Globe globe ) {
		foreach (IGlobeGenerationProcessingStep step in _processingSteps) {
			step.Process( globe );
		}
	}
}
