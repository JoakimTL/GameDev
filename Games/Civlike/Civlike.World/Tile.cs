namespace Civlike.World;

public sealed class Tile {

	private readonly Dictionary<Type, object> _states;

	internal Tile( Globe globe, ReadOnlyFace face ) {
		this.Globe = globe;
		this.Face = face;
		this._states = [];
	}

	public event Action<TileStateBase>? StateChanged;
	public Globe Globe { get; }
	public ReadOnlyFace Face { get; }

	public int Id => this.Face.Id;
	public IEnumerable<Tile> Neighbours => this.Face.Neighbours.Select( n => this.Globe.Tiles[ n.Id ] );

	public TTileState GetState<TTileState>() where TTileState : class, new() {
		if (!this._states.TryGetValue( typeof( TTileState ), out object? state ))
			this._states[ typeof( TTileState ) ] = state = new TTileState();
		return state as TTileState ?? throw new InvalidDataException( "State type does not match." );
	}

	internal void InvokeStateChanged( TileStateBase tileStateBase ) {
		this.StateChanged?.Invoke( tileStateBase );
	}
}

public abstract class TileStateBase {

	public Tile Tile { get; private set; } = null!;

	internal void SetTile( Tile tile ) => this.Tile = tile;

	public void InvokeStateChanged() => Tile.InvokeStateChanged( this );

}