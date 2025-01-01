using Engine.Standard.Render;

namespace Engine.Standard.Entities.Components.Rendering;

public sealed class Vertex3RenderedMesh : IRenderedMesh {
	public IReadOnlyList<Vertex3> Vertices { get; }
	public IReadOnlyList<uint> Indices { get; }

	public Vertex3RenderedMesh( IEnumerable<Vertex3> vertices, IEnumerable<uint> indices ) {
		this.Vertices = vertices.ToList().AsReadOnly();
		this.Indices = indices.ToList().AsReadOnly();
	}
	public Vertex3RenderedMesh( ReadOnlySpan<Vertex3> vertices, ReadOnlySpan<uint> indices ) {
		this.Vertices = vertices.ToArray().AsReadOnly();
		this.Indices = indices.ToArray().AsReadOnly();
	}
}
