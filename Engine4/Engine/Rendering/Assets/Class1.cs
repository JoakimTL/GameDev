using Engine.Rendering.Standard;

namespace Engine.Rendering.Assets;

[AttributeUsage( AttributeTargets.Class, AllowMultiple = false )]
public class AssetDirectoryAttribute : Attribute {
	public string BaseDirectory { get; }

	public AssetDirectoryAttribute( string baseDirectory ) {
		this.BaseDirectory = baseDirectory;
	}
}

//[AssetDirectory( "assets/shaders" )]
//public class ShaderAsset : AssetRef<ShaderBundle> {
//	public ShaderAsset( string path ) : base( path ) {

//	}

//	protected override ShaderBundle ResolveAsset() => throw new NotImplementedException();
//}
[AssetDirectory( "assets/textures" )]
public class TextureAsset : AssetRef<Texture> {
	public TextureAsset( string path ) : base( path ) {

	}

	protected override Texture ResolveAsset() => throw new NotImplementedException();
}
