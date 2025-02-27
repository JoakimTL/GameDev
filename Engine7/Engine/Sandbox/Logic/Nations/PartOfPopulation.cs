namespace Sandbox.Logic.Nations;

public sealed class PartOfPopulation {

	public event Action<PartOfPopulation>? CountChanged;

	public PartOfPopulation( PartOfPopulationKey key, int count ) {
		this.Key = key;
		this.Count = count;
	}

	public PartOfPopulationKey Key { get; }
	public int Count { get; private set; }

	public void AddPeople( int count ) => ChangeCount( count );

	public void RemovePeople( int count ) => ChangeCount( -count );

	private void ChangeCount( int count ) {
		if (count == 0)
			return;
		int newCount = Count + count;
		if (newCount < 0)
			throw new InvalidOperationException( "Not enough people to remove." );
		this.Count += count;
		CountChanged?.Invoke( this );
	}
}
