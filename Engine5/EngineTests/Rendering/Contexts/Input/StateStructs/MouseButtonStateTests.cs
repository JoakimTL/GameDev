using Engine.Rendering.Contexts.Input.StateStructs;
using GlfwBinding.Enums;

namespace EngineTests.Rendering.Contexts.Input.StateStructs;
[TestFixture]
public class MouseButtonStateTests
{

    //Test that mouse button state is correctly set and returns expected values
    [Test]
    public void MouseButtonState_AlternatingTrueFalse_ShowsCorrectValues()
    {
        MouseButtonState state = new(true, false, true, false, true, false, true, false);
        Assert.Multiple(() =>
        {
            Assert.That(state[MouseButton.Left], Is.True);
            Assert.That(state[MouseButton.Right], Is.False);
            Assert.That(state[MouseButton.Middle], Is.True);
            Assert.That(state[MouseButton.Button4], Is.False);
            Assert.That(state[MouseButton.Button5], Is.True);
            Assert.That(state[MouseButton.Button6], Is.False);
            Assert.That(state[MouseButton.Button7], Is.True);
            Assert.That(state[MouseButton.Button8], Is.False);
        });
    }

    [Test]
    public void MouseButtonState_AlternatingFalseTrue_ShowsCorrectValues()
    {
        MouseButtonState state = new(false, true, false, true, false, true, false, true);
        Assert.Multiple(() =>
        {
            Assert.That(state[MouseButton.Left], Is.False);
            Assert.That(state[MouseButton.Right], Is.True);
            Assert.That(state[MouseButton.Middle], Is.False);
            Assert.That(state[MouseButton.Button4], Is.True);
            Assert.That(state[MouseButton.Button5], Is.False);
            Assert.That(state[MouseButton.Button6], Is.True);
            Assert.That(state[MouseButton.Button7], Is.False);
            Assert.That(state[MouseButton.Button8], Is.True);
        });
    }

    [Test]
    public void MouseButtonState_AllTrue_ShowsCorrectValues()
    {
        MouseButtonState state = new(true, true, true, true, true, true, true, true);
        Assert.Multiple(() =>
        {
            Assert.That(state[MouseButton.Left], Is.True);
            Assert.That(state[MouseButton.Right], Is.True);
            Assert.That(state[MouseButton.Middle], Is.True);
            Assert.That(state[MouseButton.Button4], Is.True);
            Assert.That(state[MouseButton.Button5], Is.True);
            Assert.That(state[MouseButton.Button6], Is.True);
            Assert.That(state[MouseButton.Button7], Is.True);
            Assert.That(state[MouseButton.Button8], Is.True);
        });
    }

    [Test]
    public void MouseButtonState_AllFalse_ShowsCorrectValues()
    {
        MouseButtonState state = new(false, false, false, false, false, false, false, false);
        Assert.Multiple(() =>
        {
            Assert.That(state[MouseButton.Left], Is.False);
            Assert.That(state[MouseButton.Right], Is.False);
            Assert.That(state[MouseButton.Middle], Is.False);
            Assert.That(state[MouseButton.Button4], Is.False);
            Assert.That(state[MouseButton.Button5], Is.False);
            Assert.That(state[MouseButton.Button6], Is.False);
            Assert.That(state[MouseButton.Button7], Is.False);
            Assert.That(state[MouseButton.Button8], Is.False);
        });
    }

    [Test]
    public void MouseButtonState_AlternatingTrueFalse_CorrectValueOnProperties()
    {
        MouseButtonState state = new(true, false, true, false, true, false, true, false);
        Assert.Multiple(() =>
        {
            Assert.That(state.LeftButton, Is.True);
            Assert.That(state.RightButton, Is.False);
            Assert.That(state.MiddleButton, Is.True);
            Assert.That(state.Button4, Is.False);
            Assert.That(state.Button5, Is.True);
            Assert.That(state.Button6, Is.False);
            Assert.That(state.Button7, Is.True);
            Assert.That(state.Button8, Is.False);
        });
    }

    [Test]
    public void MouseButtonState_AlternatingFalseTrue_CorrectValueOnProperties()
    {
        MouseButtonState state = new(false, true, false, true, false, true, false, true);
        Assert.Multiple(() =>
        {
            Assert.That(state.LeftButton, Is.False);
            Assert.That(state.RightButton, Is.True);
            Assert.That(state.MiddleButton, Is.False);
            Assert.That(state.Button4, Is.True);
            Assert.That(state.Button5, Is.False);
            Assert.That(state.Button6, Is.True);
            Assert.That(state.Button7, Is.False);
            Assert.That(state.Button8, Is.True);
        });
    }
}
