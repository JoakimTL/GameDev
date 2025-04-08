using Sandbox.Logic.Setup;

namespace Sandbox.Logic.Nations;

public sealed class Census : IUpdateable {

	private readonly Dictionary<PartOfPopulationKey, PartOfPopulation> _population;
	private readonly Queue<PartOfPopulationKey> _deleteQueue;

	public Census() {
		_population = [];
		_deleteQueue = new();
	}

	public PartOfPopulation Get( ProfessionTypeBase? profession, Sex sex, byte educationLevel, ushort ageYears ) => Get( new( profession, sex, educationLevel, ageYears ) );

	public PartOfPopulation Get( PartOfPopulationKey key ) {
		if (!_population.TryGetValue( key, out PartOfPopulation? pop )) {
			_population.Add( key, pop = new( key, 0 ) );
			pop.CountChanged += PopCountChanged;
		}
		return pop;
	}

	public IReadOnlyList<PartOfPopulationKey> GetKeys( Func<PartOfPopulationKey, bool> predicate ) => _population.Keys.Where( predicate ).ToList();

	private void PopCountChanged( PartOfPopulation population ) {
		if (population.Count == 0)
			_deleteQueue.Enqueue( population.Key );
	}

	public void TransferPeople( PartOfPopulationKey from, PartOfPopulationKey to, int count ) {
		PartOfPopulation fromPop = Get( from );
		PartOfPopulation toPop = Get( to );
		if (fromPop.Count < count)
			throw new InvalidOperationException( "Not enough people to transfer." );
		fromPop.RemovePeople( count );
		toPop.AddPeople( count );
	}

	public void Update( double time, double deltaTime ) {
		while (_deleteQueue.TryDequeue( out PartOfPopulationKey key )) {
			if (!_population.Remove( key, out PartOfPopulation? pop ))
				continue;
			pop.CountChanged -= PopCountChanged;
		}
	}
}
