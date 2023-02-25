using GlfwBinding.Enums;
using System.Numerics;

namespace Engine.Rendering.Contexts.Input;

public class MouseState
{
    private readonly MouseData _data;

    /// <summary>
    /// The current position of the visible cursor. This value does not update if the mouse pointer is locked <b>(not visible)</b>.
    /// </summary>
    public readonly InternalState Visible;
    /// <summary>
    /// The current position of the hidden cursor. This value does not update if the mouse pointer is unlocked <b>(visible)</b>.
    /// </summary>
    public readonly InternalState Hidden;
    /// <summary>
    /// The last position of the visible cursor. This value does not update if the mouse pointer is locked <b>(not visible)</b>.
    /// </summary>
    public readonly InternalState LastVisible;
    /// <summary>
    /// The last position of the hidden cursor. This value does not update if the mouse pointer is unlocked <b>(visible)</b>.
    /// </summary>
    public readonly InternalState LastHidden;

    /// <summary>
    /// Whether the mouse if locked or not (hidden or not).
    /// </summary>
    public bool IsLocked => _data.locked;

    /// <summary>
    /// Whether the mouse if locked or not (hidden or not).
    /// </summary>
    public bool IsInside => _data.inside;

    internal MouseState(MouseData data)
    {
        _data = data;
        Visible = new InternalState(data.cursor);
        Hidden = new InternalState(data.lockedCursor);
        LastVisible = new InternalState(data.lastCursor);
        LastHidden = new InternalState(data.lastLockedCursor);
    }

    /// <summary>
    /// Returns whether the mouse button is held down or not.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <returns>The state of the button, true if pressed, false if not.</returns>
    public bool this[MouseButton button] => _data.buttons[(int)button];

    public class InternalState
    {
        private readonly MouseData.InternalData _internalData;

        internal InternalState(MouseData.InternalData iData)
        {
            _internalData = iData;
        }

        /// <summary>
        /// The position vector for the mouse pointer, this is in pixels.
        /// </summary>
        public Vector2 Position => _internalData.pos;
        /// <summary>
        /// The Normalized Device Coordinates of the mouse pointer.
        /// </summary>
        public Vector2 PositionNDC => _internalData.posNDC;
        /// <summary>
        /// The Normalized Device Coordinates with the Aspect Vector from the window multiplied in.<br/>
        /// This causes the absolute values to be greater than 1 if they surpass the axis with shortest resolution on the window.
        /// </summary>
        public Vector2 PositionNDCA => _internalData.posNDCA;
    }
}
