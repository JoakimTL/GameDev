namespace Engine.Networking;

public class DataReceiver : Identifiable {

	public event Action<byte[]>? MessageComplete;

	private int _currentExpectedMessageSize;
	private readonly List<byte> _currentMessage;

	public DataReceiver() {
		this._currentMessage = new List<byte>();
		this._currentExpectedMessageSize = 0;
	}

	public void ReceivedData( byte[] buffer, int bytesFilled ) {
		this._currentMessage.AddRange( buffer.Take( bytesFilled ) );
		while ( this._currentMessage.Count >= this._currentExpectedMessageSize ) {
			if ( this._currentMessage.Count < 4 )
				return;
			if ( this._currentExpectedMessageSize < 4 )
				this._currentExpectedMessageSize = BitConverter.ToInt32( this._currentMessage.Take( 4 ).ToArray(), 0 );
			if ( this._currentMessage.Count >= this._currentExpectedMessageSize ) {
				//The message is complete!
				byte[] message = this._currentMessage.Take( new Range( 4, this._currentExpectedMessageSize ) ).ToArray();
				this._currentMessage.RemoveRange( 0, this._currentExpectedMessageSize );
				if ( message.Length > 0 ) {
					MessageComplete?.Invoke( message );
				}
				this._currentExpectedMessageSize = 0;
			}
		}
	}

}
