using Engine.Module.Render.Ogl.Scenes;
using Engine.Standard.Render.Text.Fonts.Meshing;

namespace Engine.Standard.Render.Text.Typesetting;

public sealed class GlyphInstance : SceneInstanceCollection<GlyphVertex, Entity2SceneData>.InstanceBase {
	private Entity2SceneData _lastInstanceData;

	internal void SetGlyphMesh( GlyphMesh? mesh ) => base.SetMesh( mesh );
	internal bool SetInstanceData( Entity2SceneData data ) {
		_lastInstanceData = data;
		return UpdateInstanceData();
	}
	internal bool UpdateInstanceData() => base.Write( _lastInstanceData );

	internal new void SetAllocated( bool allocated ) => base.SetAllocated( allocated );
}