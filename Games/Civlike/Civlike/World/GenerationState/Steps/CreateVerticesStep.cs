using Engine;
using Engine.Generation.Meshing;

namespace Civlike.World.GenerationState.Steps;

[Engine.Processing.Do<IGenerationStep>.After<CreateIcosphereStep>]
public sealed class CreateVerticesStep : GlobeGenerationProcessingStepBase<GeneratingGlobeBase, GlobeGeneratorParameterBase> {
	public override string StepDisplayName => "Creating vertices";

	public override void Process( GeneratingGlobeBase globe, GlobeGeneratorParameterBase parameters ) {
		if (globe.Icosphere is null)
			throw new InvalidOperationException( "Icosphere is null." );
		Icosphere sphere = globe.Icosphere;

		Vertex[] vertices = new Vertex[ sphere.Vertices.Count ];
		for (int i = 0; i < sphere.Vertices.Count; i++) {
			Vector3<float> vector = sphere.Vertices[ i ];
			vertices[ i ] = new Vertex( i, new PackedNormal( vector ) );
		}
		globe.SetVertices( vertices );
	}
}
