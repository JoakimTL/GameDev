using Engine.Module.Render.Ogl.Scenes;

namespace Civlike.Client.Render.World.Lines;

public sealed class Line3Instance : SceneInstanceCollection<LineVertex, Line3SceneData>.InstanceBase {
	public new void SetMesh( IMesh mesh ) => base.SetMesh( mesh );
	public new bool Write<T>( T data ) where T : unmanaged => base.Write( data );
	public new void SetAllocated( bool allocated ) => base.SetAllocated( allocated );
}
