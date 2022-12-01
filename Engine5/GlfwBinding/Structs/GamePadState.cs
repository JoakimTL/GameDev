using GlfwBinding.Enums;
using System.Runtime.InteropServices;

namespace GlfwBinding.Structs;

[StructLayout( LayoutKind.Sequential )]
public readonly struct GamePadState {
	[MarshalAs( UnmanagedType.ByValArray, SizeConst = 15 )]
	private readonly byte[] _states;

	[MarshalAs( UnmanagedType.ByValArray, SizeConst = 6 )]
	private readonly float[] _axes;

	public InputState GetButtonState( GamePadButton button ) => ( InputState) this._states[ (int) button ];
	public float GetAxis( GamePadAxis axis ) => this._axes[ (int) axis ];
}