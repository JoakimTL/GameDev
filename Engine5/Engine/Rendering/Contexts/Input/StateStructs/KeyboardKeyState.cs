using Engine.Rendering.Contexts.Services;
using GlfwBinding.Enums;
using System.Runtime.InteropServices;

namespace Engine.Rendering.Contexts.Input.StateStructs;

[StructLayout( LayoutKind.Explicit )]
public readonly struct KeyboardKeyState {
    [FieldOffset( 0 )]
    private readonly ulong _state0_63;
    [FieldOffset( 8 )]
    private readonly ulong _state64_127;

    public void FillKeyBoolArray( bool[] keys ) {
        if ( keys.Length < InputEventService.KeyBooleanArrayLength )
            throw new ArgumentException( "The length of the keys array must be at least KeyboardInputEventManager.MaxKeyIndex" );
        unsafe {
            fixed ( int* keyIndexPtr = _keyList ) {
                for ( int i = 0; i < 64; i++ ) {
                    byte value = (byte) ( _state0_63 >> i & 1 );
                    keys[ keyIndexPtr[ i ] ] = *(bool*) &value;
                }
                int maxIterationIndex = _keyList.Length - 64;
                for ( int i = 0; i < maxIterationIndex; i++ ) {
                    byte value = (byte) ( _state64_127 >> i & 1 );
                    keys[ keyIndexPtr[ i + 64 ] ] = *(bool*) &value;
                }
            }
        }
    }

    public string GetBitsAsString() => $"{_state0_63.ToBinaryString()} {_state64_127.ToBinaryString()}";

    private static readonly int[] _keyList = Enum.GetValues<Keys>().Where( p => (int) p >= 0 ).Select( p => (int) p ).ToArray();

    public static KeyboardKeyState CreateState( bool[] keys ) {
        if ( keys.Length < InputEventService.KeyBooleanArrayLength )
            throw new ArgumentException( "The length of the keys array must be at least KeyboardInputEventManager.MaxKeyIndex" );
        KeyboardKeyState state = new();
        unsafe {
            byte* ptr = (byte*) &state;
            fixed ( int* keyIndexPtr = _keyList )
            fixed ( bool* keyStatesPtr = keys ) {
                byte* keyStatesAsBytesPtr = (byte*) keyStatesPtr;
                for ( int i = 0; i <= _keyList.Length / 8; i++ ) {
                    int startIndex = i * 8;
                    byte* currentPtr = ptr + i;
                    int maxIteration = 8;
                    if ( i == _keyList.Length / 8 )
                        maxIteration = _keyList.Length - startIndex;
                    for ( int j = 0; j < maxIteration; j++ )
                        *currentPtr |= (byte) ( keyStatesAsBytesPtr[ keyIndexPtr[ startIndex + j ] ] << j );
                }
            }
        }
        return state;
    }
}
