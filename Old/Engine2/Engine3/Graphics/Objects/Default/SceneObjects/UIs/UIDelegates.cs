using Engine.GLFrameWork;
using Engine.LinearAlgebra;
using System;

namespace Engine.Graphics.Objects.Default.SceneObjects.UIs {
	public delegate void ElementRelationChangeHandler( UIElement element, UIElement other );
	public delegate void ManagerChangeHandler( UIManager manager, UIElement child );
	public delegate void KeyEventHandler( IntPtr winPtr, uint id, Keys key, ref bool triggered );
	public delegate void MouseButtonEventHandler( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data, ref bool triggered );
	public delegate void KeyWrittenHandler( IntPtr winPtr, uint id, char c, ref bool triggered );
	public delegate void MouseMoveHandler( IntPtr winPtr, uint id, MouseInputEventData data, ref bool triggered );
	public delegate void MouseWheelMoveHandler( IntPtr winPtr, uint id, float delta, MouseInputEventData data, ref bool triggered );
	public delegate void WindowResizeHandler( Vector2 arv, Vector2i size );
	public delegate void UpdateHandler( MouseInputEventData data );
	public delegate void TactileKeyEventHandler( IntPtr winPtr, uint id, Keys key );
	public delegate void TactileEventHandler( IntPtr winPtr, uint id, MouseButton button, MouseInputEventData data );

	public delegate void ConstraintModifierHandler( ConstraintTransform transform, float time, UIElement e, GLWindow window );
}
