namespace Engine.Rendering.Standard.Scenes;

public class LayeredScene : Scene {
	protected override int SortMethod( ISceneObject x, ISceneObject y ) {
		if ( x.HasTransparency != y.HasTransparency )
			return x.HasTransparency ? 1 : -1;
		if ( x.Layer != y.Layer )
			return x.Layer > y.Layer ? 1 : -1;
		if ( x.SortingIndex != y.SortingIndex )
			return x.SortingIndex > y.SortingIndex ? 1 : -1;
		return x.ID > y.ID ? 1 : -1;
	}
}
