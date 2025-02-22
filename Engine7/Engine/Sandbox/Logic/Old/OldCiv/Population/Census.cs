using Sandbox.Logic.Old.OldCiv.Research;
using Sandbox.Logic.World.Tiles;
using Sandbox.Logic.World.Tiles.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Old.OldCiv.Population;
public sealed class Census : IUpdateable {
	private readonly PopulationCenter _populationCenter;

	private ulong _totalPopulation;

	public Census( PopulationCenter populationCenter ) {
		this._populationCenter = populationCenter;
	}

	public void Update( double time, double deltaTime ) {
		_totalPopulation = 0;
		foreach (Tile tile in _populationCenter.Tiles) {
			//_totalPopulation += tile.DataModel.GetData<TilePopulationData>().Population;
		}
	}
}

public abstract class ProfessionBase( string name, params TechnologyFieldBase[] techFields ) {
	public string Name { get; } = name;
	public IReadOnlyList<TechnologyFieldBase> TechFields { get; } = techFields.ToList().AsReadOnly();
}

public static class ProfessionList {

}


public sealed class PopulationCenter {

	public string Name { get; set; }
	private readonly List<Tile> _tiles;
	private readonly Census _census;
	private readonly TechnologyResearcher _researcher;

	public PopulationCenter( PopulationCenter? derivedFrom, Tile originTile, string name ) {
		Name = name;
		_tiles = [ originTile ];
		_census = new( this );
		_researcher = new( derivedFrom?._researcher.ResearchedTechnologyTypes.ToHashSet(), _census );
	}

	public IReadOnlyList<Tile> Tiles => _tiles;

	public void AddTile( Tile tile ) {
		_tiles.Add( tile );
	}

	public void RemoveTile( Tile tile ) {
		_tiles.Remove( tile );
	}

	public void MoveTile( Tile tile, PopulationCenter otherPopulationCenter ) {
		RemoveTile( tile );
		otherPopulationCenter.AddTile( tile );
	}

	public void Update( double time, double deltaTime ) {
		_census.Update( time, deltaTime );
		_researcher.Update( time, deltaTime );
	}
}