using Engine.Rendering.Standard;

namespace Engine.Modularity.ECS.Organization;

public class BatchSceneObject<V, SD> : ClosedSceneObject<V, SD> where V : unmanaged where SD : unmanaged {

	private readonly bool[] _slots;
	private uint _lowestAvailable;
	private int _highestTaken;

	public BatchSceneObject( BufferedMesh mesh, ShaderBundle shader, uint batchSize ) {
		SetMesh( mesh );
		SetShaders( shader );
		SetSceneData( new SceneInstanceData<SD>( batchSize, 0 ) );
		this._slots = new bool[ batchSize ];
		this._lowestAvailable = 0;
		this._highestTaken = -1;
	}

	public void SetSceneData( uint i, SD data ) => this.SceneData!.SetInstance( i, data );

	public bool Available => this._lowestAvailable < this._slots.Length;
	public uint TakeSlot() {
		if ( this._lowestAvailable >= this._slots.Length )
			throw new InvalidOperationException( "No available slots!" );
		uint slot = this._lowestAvailable;
		this._slots[ slot ] = true;
		for ( ; this._lowestAvailable < this._slots.Length; this._lowestAvailable++ )
			if ( !this._slots[ this._lowestAvailable ] )
				break;
		if ( slot > this._highestTaken ) {
			this._highestTaken = (int) slot;
			this.SceneData!.SetActiveInstances( (uint) ( this._highestTaken + 1 ) );
		}
		return slot;
	}

	public void ReleaseSlot( uint slot ) {
		this._slots[ slot ] = false;
		if ( slot < this._lowestAvailable )
			this._lowestAvailable = slot;
		if ( slot == this._highestTaken ) {
			for ( ; this._highestTaken >= 0; ) {
				if ( this._slots[ this._highestTaken ] || this._highestTaken == -1 )
					break;
				if ( this._highestTaken >= 0 )
					this._highestTaken--;
			}
			this.SceneData!.SetActiveInstances( (uint)( this._highestTaken + 1) );
		}
		this.SceneData!.SetInstance( slot, default );
	}

	public override void Bind() {

	}
}