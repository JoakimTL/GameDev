using Engine.Datatypes.Buffers;

namespace Engine.Networking.Modules.TransferLayer.Tunnels;

public class DataReceiver : Identifiable {

	public event Action<byte[]>? MessageComplete;

	private uint _currentExpectedMessageSize;
	private readonly ProgressiveByteBuffer _currentMessage;

	public DataReceiver() {
		_currentMessage = new();
		_currentExpectedMessageSize = 0;
	}

	public void ReceivedData( byte[] buffer, uint bytesFilled ) {
		unsafe {
			byte* sizeDataPtr = stackalloc byte[ 4 ];
			_currentMessage.Add( buffer, bytesFilled );
			while ( _currentMessage.CaretPosition >= _currentExpectedMessageSize ) {
				if ( _currentMessage.CaretPosition < 4 )
					return;
				if ( _currentExpectedMessageSize < 4 )
					_currentExpectedMessageSize = _currentMessage.Read<uint>( 0 );
				if ( _currentMessage.CaretPosition >= _currentExpectedMessageSize ) {
					//The message is complete!
					byte[] message = _currentMessage.Flush( 4 );
					if ( message.Length > 0 )
						MessageComplete?.Invoke( message );
					_currentExpectedMessageSize = 0;
				}
			}
		}
	}

}