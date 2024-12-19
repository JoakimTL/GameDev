using Engine.Module.Entities.Container;
using Engine.Standard.Render;

namespace Engine.Standard.Entities.Components.Rendering;

public sealed class RenderedVertex3ShapeComponent : ComponentBase {
	private readonly List<Vertex3> _vertices = [];
	private readonly List<uint> _indices = [];

	public IReadOnlyList<Vertex3> Vertices => this._vertices;
	public IReadOnlyList<uint> Indices => this._indices;

	public void Set( IEnumerable<Vertex3> vertices, IEnumerable<uint> indices ) {
		this._vertices.Clear();
		this._indices.Clear();
		this._vertices.AddRange( vertices );
		this._indices.AddRange( indices );
		InvokeComponentChanged();
	}
}
