using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts.Meshing;

namespace Engine.Standard.Render.Text.Typesetting;

public sealed class GlyphInstance : SceneInstanceCollection<GlyphVertex, Entity2SceneData>.InstanceBase {
	internal void SetGlyphMesh( GlyphMesh? mesh ) => base.SetMesh( mesh );
	internal bool SetInstanceData( Entity2SceneData data ) => base.Write( data );
}