using Sandbox.Logic.Resources;

namespace Sandbox.Logic.World.Tiles.Data;

public sealed class TileResourceData : TileDataBase, ITileData {
	private const string _dataCode = "0RES";
	public static int CodeId { get; } = _dataCode.ToIntCode();

	/// <summary>
	/// The current quantity of resources on the tile.
	/// </summary>
	private readonly Dictionary<int, float> _resourceQuantity = [];
	/// <summary>
	/// The rate at which resources are renewed on the tile. (Think inflow of water from a river, or the growth of plants.)
	/// </summary>
	private readonly Dictionary<int, float> _resourceRenewal = [];
	/// <summary>
	/// The capacity of the tile to hold resources. If there is no entry then the capacity is "infinite".
	/// </summary>
	private readonly Dictionary<int, float> _resourceMaxQuantity = [];

	public void Set( int resourceId, float abundance ) => _resourceQuantity[ resourceId ] = abundance;
	public void Set( string resourceIdentifier, float abundance ) => _resourceQuantity[ ResourceList.GetResource( resourceIdentifier ).Id ] = abundance;
	public void SetLimit( int resourceId, float resourceLimit ) => _resourceMaxQuantity[ resourceId ] = resourceLimit;
	public void SetLimit( string resourceIdentifier, float resourceLimit ) => _resourceMaxQuantity[ ResourceList.GetResource( resourceIdentifier ).Id ] = resourceLimit;
	public void SetRenewalRate( int resourceId, float rateOfRenewal ) => _resourceRenewal[ resourceId ] = rateOfRenewal;
	public void SetRenewalRate( string resourceIdentifier, float rateOfRenewal ) => _resourceRenewal[ ResourceList.GetResource( resourceIdentifier ).Id ] = rateOfRenewal;

	public float Get( int resourceId ) => _resourceQuantity.TryGetValue( resourceId, out float value ) ? value : 0;
	public float Get( string resourceIdentifier ) => _resourceQuantity.TryGetValue( ResourceList.GetResource( resourceIdentifier ).Id, out float value ) ? value : 0;
	public float GetLimit( int resourceId ) => _resourceMaxQuantity.TryGetValue( resourceId, out float value ) ? value : float.MaxValue;
	public float GetLimit( string resourceIdentifier ) => _resourceMaxQuantity.TryGetValue( ResourceList.GetResource( resourceIdentifier ).Id, out float value ) ? value : float.MaxValue;
	public float GetRenewalRate( int resourceId ) => _resourceRenewal.TryGetValue( resourceId, out float value ) ? value : 0;
	public float GetRenewalRate( string resourceIdentifier ) => _resourceRenewal.TryGetValue( ResourceList.GetResource( resourceIdentifier ).Id, out float value ) ? value : 0;

}
