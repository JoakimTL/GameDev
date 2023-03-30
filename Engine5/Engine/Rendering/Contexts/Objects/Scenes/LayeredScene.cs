namespace Engine.Rendering.Contexts.Objects.Scenes;

public abstract class LayeredScene : Scene {
	public LayeredScene( IEnumerable<string> shaderIndices ) : base( shaderIndices ) { }

	public override int SortMethod( SceneObjectUsage x, SceneObjectUsage y ) {
		if ( x.SceneObject.ShaderBundle?.Get( x.ShaderIndex )?.UsesTransparency != y.SceneObject.ShaderBundle?.Get( x.ShaderIndex )?.UsesTransparency )
			return ( x.SceneObject.ShaderBundle?.Get( x.ShaderIndex )?.UsesTransparency ?? false ) ? 1 : -1;
		if ( x.SceneObject.Layer != y.SceneObject.Layer )
			return x.SceneObject.Layer > y.SceneObject.Layer ? 1 : -1;
		if ( x.SceneObject.SortingIndex != y.SceneObject.SortingIndex )
			return x.SceneObject.SortingIndex > y.SceneObject.SortingIndex ? 1 : -1;
		return x.SceneObject.Uid > y.SceneObject.Uid ? 1 : -1;
	}
}
