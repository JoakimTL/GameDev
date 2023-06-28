using Engine.Rendering.Contexts.Input.StateStructs;
using Engine.Rendering.Contexts.Services;
using GlfwBinding.Enums;

namespace EngineTests.Rendering.Contexts.Input;

[TestFixture]
public class KeyboardKeyStateTests {

	[Test]
	public void KeyboardState_RandomKeysTrue_StateReturnsSameKeysTrueToNewBoolArray() {
		// Arrange
		var keys = new bool[ InputEventService.KeyBooleanArrayLength ];
		//Set "random" (statically) keys to true
		keys[ (int) Keys.M ] = true;
		keys[ (int) Keys.Alpha0 ] = true;
		keys[ (int) Keys.F3 ] = true;
		keys[ (int) Keys.F25 ] = true;
		keys[ (int) Keys.Home ] = true;
		keys[ (int) Keys.Menu ] = true;
		keys[ (int) Keys.Insert ] = true;
		keys[ (int) Keys.Escape ] = true;
		keys[ (int) Keys.A ] = true;
		keys[ (int) Keys.Alpha5 ] = true;
		var keyboardState = KeyboardKeyState.CreateState( keys );

		// Act
		var newKeys = new bool[ InputEventService.KeyBooleanArrayLength ];
		keyboardState.FillKeyBoolArray( newKeys );

		// Assert
		CollectionAssert.AreEqual( keys, newKeys );
	}

	[Test]
	public void KeyboardState_AllKeysFalse_StateReturnsAllKeysFalse() {
		// Arrange
		var keys = new bool[ InputEventService.KeyBooleanArrayLength ];
		var keyboardState = KeyboardKeyState.CreateState( keys );

		// Act
		var newKeys = new bool[ InputEventService.KeyBooleanArrayLength ];
		keyboardState.FillKeyBoolArray( newKeys );

		// Assert
		CollectionAssert.AreEqual( keys, newKeys );
	}

	[Test]
	public void KeyboardState_AllKeysTrue_StateReturnsAllKeysTrue() {
		// Arrange
		var keys = new bool[ InputEventService.KeyBooleanArrayLength ];
		foreach ( var key in Enum.GetValues<Keys>() )
			if ( key != Keys.Unknown )
				keys[ (int) key ] = true;
		var keyboardState = KeyboardKeyState.CreateState( keys );

		// Act
		var newKeys = new bool[ InputEventService.KeyBooleanArrayLength ];
		keyboardState.FillKeyBoolArray( newKeys );

		// Assert
		CollectionAssert.AreEqual( keys, newKeys );
	}

}
