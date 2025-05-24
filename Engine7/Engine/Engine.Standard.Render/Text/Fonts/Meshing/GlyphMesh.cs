using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Standard.Render.Text.Fonts.Meshing;

public sealed class GlyphMesh : DisposableIdentifiable, IMesh {
	private readonly VertexMesh<GlyphVertex> _mesh;

	public event Action? OnOffsetChanged;

	public Type VertexType => this._mesh.VertexType;
	public uint ElementCount => this._mesh.ElementCount;
	public uint ElementOffset => this._mesh.ElementOffset;
	public uint VertexOffset => this._mesh.VertexOffset;

	public DefinedGlyph GlyphDefinition { get; }

	public GlyphMesh( DefinedGlyph glyphDefinition, VertexMesh<GlyphVertex> mesh ) {
		this.GlyphDefinition = glyphDefinition;
		this._mesh = mesh;
		this._mesh.OnOffsetChanged += OnMeshChanged;
		this.Nickname = mesh.Nickname;
	}

	private void OnMeshChanged() => OnOffsetChanged?.Invoke();

	protected override bool InternalDispose() {
		this._mesh.OnOffsetChanged -= OnMeshChanged;
		this._mesh.Dispose();
		return true;
	}
}
