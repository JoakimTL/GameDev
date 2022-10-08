using Engine.Rendering.Standard.Meshing;
using Engine.Rendering.Standard.UI.Standard.Shaders;
using Engine.Rendering.Standard.VertexArrayObjects.Layouts;

namespace Engine.Rendering.Standard.UI.Standard;

public class ColoredMeshDisplay : RenderedUIElement<ColoredMeshData, UIShaderBundle, Vertex2, Entity2SceneData> {
	public ColoredMeshDisplay( VertexMesh<Vertex2> mesh ) {
		this.SceneObject.SetMesh( mesh );
	}

	public void SetMesh( VertexMesh<Vertex2> mesh ) => this.SceneObject.SetMesh( mesh );
	protected override Entity2SceneData GetSceneData() => new() { ModelMatrix = this.Transform.Matrix, Color = this._renderData.Color };
}
