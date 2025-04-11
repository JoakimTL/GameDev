//namespace Sandbox.Logic.Nations;

//public sealed class PartOfPopulation {

//	public event Action<PartOfPopulation>? CountChanged;

//	public PartOfPopulation( PartOfPopulationKey key, long count ) {
//		this.Key = key;
//		this.Count = count;
//	}

//	public PartOfPopulationKey Key { get; }
//	public long Count { get; private set; }

//	public void AddPeople( long count ) => ChangeCount( count );

//	public void RemovePeople( long count ) => ChangeCount( -count );

//	private void ChangeCount( long count ) {
//		if (count == 0)
//			return;
//		long newCount = Count + count;
//		if (newCount < 0)
//			throw new InvalidOperationException( "Not enough people to remove." );
//		this.Count += count;
//		CountChanged?.Invoke( this );
//	}
//}
