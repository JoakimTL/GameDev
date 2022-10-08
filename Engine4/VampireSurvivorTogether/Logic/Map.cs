using Engine.Data.Datatypes;
using Engine.Modularity.ECS;

namespace VampireSurvivorTogether.Logic;
public class Map {

	public Dictionary<Vector2i, MapArea> _areas;

}

public class MapArea {

	public const int SideLength = 8;

	public readonly Vector2i Coordinates;
	private readonly HashSet<Entity> _entities;

	public MapArea( Vector2i coordinates ) {
		this.Coordinates = coordinates;
		this._entities = new();
	}

	public void AddEntity( Entity entity ) {
		this._entities.Add( entity );
	}

	public void RemoveEntity( Entity entity ) {
		this._entities.Remove( entity );
	}

	public void Update(List<Entity> relocatedEntities) {

	}

}