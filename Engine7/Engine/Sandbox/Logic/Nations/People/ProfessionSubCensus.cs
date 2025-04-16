using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations.People;

public sealed class ProfessionSubCensus {
	private readonly Dictionary<ProfessionKey, uint> _populationByProfession = [];

	public (ProfessionKey, uint) GetPeople( ProfessionTypeBase profession, ushort educationLevel ) {
		ProfessionKey key = new( educationLevel, profession );
		return (key, _populationByProfession.TryGetValue( key, out var population ) ? population : 0);
	}

	public void AddPeople( ProfessionKey key, uint count ) {
		_populationByProfession.TryGetValue( key, out uint population );
		population += count;
		_populationByProfession[ key ] = population;
	}

	public void RemovePeople( ProfessionKey key, uint count ) {
		_populationByProfession.TryGetValue( key, out uint population );
		if (population < count)
			throw new ArgumentOutOfRangeException( nameof( count ), "Cannot remove more people than exist." );
		population -= count;
		if (population == 0)
			_populationByProfession.Remove( key );
		else
			_populationByProfession[ key ] = population;
	}

	public void TransferPeople( ProfessionKey from, ProfessionKey to, uint count ) {
		RemovePeople( from, count );
		AddPeople( to, count );
	}

	public void AddPeople( ProfessionTypeBase profession, ushort educationLevel, uint count ) => AddPeople( new ProfessionKey( educationLevel, profession ), count );

	public void RemovePeople( ProfessionTypeBase profession, ushort educationLevel, uint count ) => RemovePeople( new ProfessionKey( educationLevel, profession ), count );

}