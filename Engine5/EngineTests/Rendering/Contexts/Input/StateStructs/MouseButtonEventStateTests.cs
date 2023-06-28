using Engine.Rendering.Contexts.Input.StateStructs;
using GlfwBinding.Enums;

namespace EngineTests.Rendering.Contexts.Input.StateStructs;

[TestFixture]
public class MouseButtonEventStateTests {

    [Test]
    public void MouseButterEventStateConstructor_LeftMouseButtonStateTrueNoModifier_PropertiesMatchConstructorParameters() {
        // Arrange
        var button = MouseButton.Left;
        var state = true;
        ModifierKeys modifierKeys = 0;

        // Act
        var mouseButtonEventState = new MouseButtonEventState( button, state, modifierKeys );

        // Assert
        Assert.That( mouseButtonEventState.Button, Is.EqualTo( button ) );
        Assert.That( mouseButtonEventState.Depressed, Is.EqualTo( state ) );
        Assert.That( mouseButtonEventState.ModifierKeys, Is.EqualTo( modifierKeys ) );
    }

    [Test]
    public void MouseButterEventStateConstructor_RightMouseButtonStateFalseNoModifier_PropertiesMatchConstructorParameters() {
        // Arrange
        var button = MouseButton.Right;
        var state = false;
        ModifierKeys modifierKeys = 0;

        // Act
        var mouseButtonEventState = new MouseButtonEventState( button, state, modifierKeys );

        // Assert
        Assert.That( mouseButtonEventState.Button, Is.EqualTo( button ) );
        Assert.That( mouseButtonEventState.Depressed, Is.EqualTo( state ) );
        Assert.That( mouseButtonEventState.ModifierKeys, Is.EqualTo( modifierKeys ) );
    }

    [Test]
    public void MouseButterEventStateConstructor_MiddleMouseButtonStateTrueSomeModifiers_PropertiesMatchConstructorParameters() {
        // Arrange
        var button = MouseButton.Middle;
        var state = true;
        ModifierKeys modifierKeys = ModifierKeys.Alt | ModifierKeys.Control;

        // Act
        var mouseButtonEventState = new MouseButtonEventState( button, state, modifierKeys );

        // Assert
        Assert.That( mouseButtonEventState.Button, Is.EqualTo( button ) );
        Assert.That( mouseButtonEventState.Depressed, Is.EqualTo( state ) );
        Assert.That( mouseButtonEventState.ModifierKeys, Is.EqualTo( modifierKeys ) );
    }


}


[TestFixture]
public class KeyEventStateTests {


    [Test]
    public void KeyEventStateConstructor_AKeyStateTrueNoModifier_PropertiesMatchConstructorParameters() {
        // Arrange
        var key = Keys.A;
        var state = true;
        ModifierKeys modifierKeys = 0;

        // Act
        var keyEventState = new KeyEventState( key, state, modifierKeys );

        // Assert
        Assert.That( keyEventState.Key, Is.EqualTo( key ) );
        Assert.That( keyEventState.Depressed, Is.EqualTo( state ) );
        Assert.That( keyEventState.ModifierKeys, Is.EqualTo( modifierKeys ) );
    }

    [Test]
    public void KeyEventStateConstructor_BKeyStateFalseNoModifier_PropertiesMatchConstructorParameters() {
        // Arrange
        var key = Keys.B;
        var state = false;
        ModifierKeys modifierKeys = 0;

        // Act
        var keyEventState = new KeyEventState( key, state, modifierKeys );

        // Assert
        Assert.That( keyEventState.Key, Is.EqualTo( key ) );
        Assert.That( keyEventState.Depressed, Is.EqualTo( state ) );
        Assert.That( keyEventState.ModifierKeys, Is.EqualTo( modifierKeys ) );
    }

    [Test]
    public void KeyEventStateConstructor_CKeyStateTrueSomeModifiers_PropertiesMatchConstructorParameters() {
        // Arrange
        var key = Keys.C;
        var state = true;
        ModifierKeys modifierKeys = ModifierKeys.Alt | ModifierKeys.Control;

        // Act
        var keyEventState = new KeyEventState( key, state, modifierKeys );

        // Assert
        Assert.That( keyEventState.Key, Is.EqualTo( key ) );
        Assert.That( keyEventState.Depressed, Is.EqualTo( state ) );
        Assert.That( keyEventState.ModifierKeys, Is.EqualTo( modifierKeys ) );
    }

}