using Engine.Datatypes.Buffers;

namespace Engine.Networking.Module.TransferLayer.Tunnels;

public class DataReceiver : Identifiable {

    public event Action<byte[]>? MessageComplete;

    private uint _currentExpectedMessageSize;
    private readonly ProgressiveByteBuffer _currentMessage;

    public DataReceiver() {
        this._currentMessage = new();
        this._currentExpectedMessageSize = 0;
    }

    public void ReceivedData( byte[] buffer, uint bytesFilled ) {
        unsafe {
            byte* sizeDataPtr = stackalloc byte[ 4 ];
            this._currentMessage.Add( buffer, bytesFilled );
            while ( this._currentMessage.CaretPosition >= this._currentExpectedMessageSize ) {
                if ( this._currentMessage.CaretPosition < 4 )
                    return;
                if ( this._currentExpectedMessageSize < 4 )
                    this._currentExpectedMessageSize = _currentMessage.Read<uint>( 0 );
                if ( this._currentMessage.CaretPosition >= this._currentExpectedMessageSize ) {
                    //The message is complete!
                    byte[] message = this._currentMessage.Flush( 4 );
                    if ( message.Length > 0 )
                        MessageComplete?.Invoke( message );
                    this._currentExpectedMessageSize = 0;
                }
            }
        }
    }

}