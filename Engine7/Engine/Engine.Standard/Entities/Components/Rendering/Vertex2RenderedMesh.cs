//using Engine.Standard.Render;

//namespace Engine.Standard.Entities.Components.Rendering;

//public sealed class Vertex2RenderedMesh : IRenderedMesh {
//	public IReadOnlyList<Vertex3> Vertices { get; }
//	public IReadOnlyList<uint> Indices { get; }

//	public Vertex2RenderedMesh( IEnumerable<Vertex3> vertices, IEnumerable<uint> indices ) {
//		this.Vertices = vertices.ToList().AsReadOnly();
//		this.Indices = indices.ToList().AsReadOnly();
//	}

//	public Vertex2RenderedMesh( ReadOnlySpan<Vertex3> vertices, ReadOnlySpan<uint> indices ) {
//		this.Vertices = vertices.ToArray().AsReadOnly();
//		this.Indices = indices.ToArray().AsReadOnly();
//	}
//}