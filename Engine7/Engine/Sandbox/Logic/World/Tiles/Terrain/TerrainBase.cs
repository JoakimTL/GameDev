namespace Sandbox.Logic.World.Tiles.Terrain;

public abstract class TerrainBase {

	private static readonly Dictionary<int, TerrainBase> _terrainTypes;

	static TerrainBase() {
		_terrainTypes = [];
		IEnumerable<Type> terrainTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.BaseType == typeof( TerrainBase ) );

		foreach (Type? terrainType in terrainTypes) {
			TerrainBase instance = terrainType.CreateInstance( null ) as TerrainBase ?? throw new InvalidOperationException( "Failed to construct terrain instance. Must have a parameterless constructor." );
			if (instance is IInitializable initializable)
				initializable.Initialize();
			if (_terrainTypes.TryGetValue( instance.Id, out TerrainBase? existing ))
				throw new InvalidOperationException( $"Terrain type with id {instance.Code} already exists: {existing.Name}." );
			_terrainTypes.Add( instance.Id, instance );
		}
	}

	public string Code { get; }
	public int Id { get; }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="code">Must be 4 characters. Should be filled with UTF-8 characters.</param>
	/// <param name="name"></param>
	/// <exception cref="ArgumentException"></exception>
	protected TerrainBase( string code, string name ) {
		if (string.IsNullOrWhiteSpace( code ))
			throw new ArgumentException( "ID code cannot be null or empty.", nameof( code ) );
		if (code.Length != 4)
			throw new ArgumentException( "ID code must be 4 characters long.", nameof( code ) );
		Code = code;
		unsafe {
			byte* codeBytes = stackalloc byte[ 4 ];
			System.Text.Encoding.UTF8.GetBytes( code, new Span<byte>( codeBytes, 4 ) );
			this.Id = *(int*) codeBytes;
		}
		this.Name = name;
	}

	public string Name { get; }
}

public sealed class DeepWater() : TerrainBase( "00DW", "Deep Water" ) {

}
public sealed class ShallowWater() : TerrainBase( "00SW", "Shallow Water" ) {

}
public sealed class CoastalWater() : TerrainBase( "00CW", "Coastal Water" ) {

}
public sealed class FreshWater() : TerrainBase( "00FW", "Fresh Water" ) {

}
public sealed class RockyCoast() : TerrainBase( "00RC", "Rocky Coast" ) {

}
public sealed class SandyCoast() : TerrainBase( "00SC", "Sandy Coast" ) {
}