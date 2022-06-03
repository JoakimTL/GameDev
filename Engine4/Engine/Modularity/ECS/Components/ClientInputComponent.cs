using System.Collections;
using System.Numerics;
using Engine.Data;
using Engine.Modularity.ECS.Networking;
using Engine.Rendering.InputHandling;
using GLFW;

namespace Engine.Modularity.ECS.Components;
[Identification( "4db39bf6-bb4b-4258-ad57-e0d032018480" )]
[Network( ComponentBroadcastSide.BOTH, System.Net.Sockets.ProtocolType.Udp )]
public class ClientInputComponent : UpdateableComponent {

	/// <summary>
	/// An arbitrary unit with some relation to the users mouse cursor position on screen, or the OGL virtual mouse. This is affected by sensitivity and other things, and can be used however wanted.
	/// </summary>
	public Vector2 MousePosition { get; set; }
	/// <summary>
	/// The position of the mouse scroll wheel.
	/// </summary>
	public float MouseScroll { get; set; }
	private readonly BitArray _buttons;
	private readonly BitArray _keys;

	public ClientInputComponent() {
		this._buttons = new( DataUtils.BitsInUnmanagedArray<byte>( MouseInputEventManager.MaxButtonIndex ) * 8 );
		this._keys = new( DataUtils.BitsInUnmanagedArray<byte>( KeyboardInputEventManager.MaxKeyIndex ) * 8 );
	}

	public void Set( MouseButton button, bool state ) {
		int buttonId = (int) button;
		if ( buttonId < 0 || buttonId >= this._buttons.Count )
			return;
		this._buttons.Set( buttonId, state );
	}

	public void Set( Keys key, bool state ) {
		int keyId = (int) key;
		if ( keyId < 0 || keyId >= this._keys.Count )
			return;
		this._keys.Set( keyId, state );
	}

	public bool IsButtonPressed( MouseButton button ) {
		int buttonIndex = (int) button;
		if ( buttonIndex < 0 || buttonIndex > this._buttons.Count )
			return false;
		return this._buttons[ buttonIndex ];
	}

	public bool IsKeyPressed( Keys key ) {
		int keyIndex = (int) key;
		if ( keyIndex < 0 || keyIndex > this._keys.Count )
			return false;
		return this._keys[ keyIndex ];
	}

	public override void SetFromSerializedData( byte[] data ) {
		byte[][]? segments = Segmentation.Parse( data );
		if ( segments is null || segments.Length != 4 )
			return;
		Vector2? mousePosition = DataUtils.ToUnmanaged<Vector2>( segments[ 0 ] );
		if ( mousePosition.HasValue )
			this.MousePosition = mousePosition.Value;
		float? mouseScroll = DataUtils.ToUnmanaged<float>( segments[ 1 ] );
		if ( mouseScroll.HasValue )
			this.MouseScroll = mouseScroll.Value;
		BitArray keys = new( segments[ 2 ] );
		this._keys.SetAll( false );
		this._keys.Or( keys );
		BitArray buttons = new( segments[ 3 ] );
		this._buttons.SetAll( false );
		this._buttons.Or( buttons );
	}

	protected override byte[]? GetSerializedData() {
		byte[] keyData = new byte[ DataUtils.BitsInUnmanagedArray<byte>( this._keys.Count ) ];
		byte[] buttonData = new byte[ DataUtils.BitsInUnmanagedArray<byte>( this._buttons.Count ) ];
		this._keys.CopyTo( keyData, 0 );
		this._buttons.CopyTo( buttonData, 0 );
		return Segmentation.Segment(
			DataUtils.ToBytes( this.MousePosition ),
			DataUtils.ToBytes( this.MouseScroll ),
			keyData,
			buttonData
		);
	}

	public override void Update( float time, float deltaTime ) {
		/*if ( Resources.Render.TryGetContext( out Context? context ) ) {
			Set( Keys.U, context.Window.KeyboardEvents[ Keys.U ] );
			Set( Keys.J, context.Window.KeyboardEvents[ Keys.J ] );
		}*/
	}
}
