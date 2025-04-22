using Engine.Module.Render.Ogl.Scenes;

namespace Civs.Render.World.Lines;

public sealed class Line3Instance : SceneInstanceCollection<LineVertex, Line3SceneData>.InstanceBase {
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
	public new void SetActive( bool active ) => base.SetActive( active );
}
