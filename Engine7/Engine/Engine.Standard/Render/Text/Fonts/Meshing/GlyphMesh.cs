﻿using Engine.Module.Render.Ogl.Scenes;

namespace Engine.Standard.Render.Text.Fonts.Meshing;

public sealed class GlyphMesh : DisposableIdentifiable, IMesh {
	private readonly VertexMesh<GlyphVertex> _mesh;

	public event Action? OnOffsetChanged;

	public Type VertexType => _mesh.VertexType;
	public uint ElementCount => _mesh.ElementCount;
	public uint ElementOffset => _mesh.ElementOffset;
	public uint VertexOffset => _mesh.VertexOffset;

	public IGlyph Glyph { get; }

	public GlyphMesh( IGlyph glyph, VertexMesh<GlyphVertex> mesh ) {
		this.Glyph = glyph;
		this._mesh = mesh;
		this._mesh.OnOffsetChanged += OnMeshChanged;
		Nickname = mesh.Nickname;
	}

	private void OnMeshChanged() => OnOffsetChanged?.Invoke();

	protected override bool InternalDispose() {
		_mesh.OnOffsetChanged -= OnMeshChanged;
		_mesh.Dispose();
		return true;
	}
}
