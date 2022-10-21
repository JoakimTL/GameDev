using System.Numerics;

namespace Engine.Rendering.Input;

internal class MouseData {
	public readonly bool[] buttons;
	public readonly InternalData cursor;
	public readonly InternalData lockedCursor;
	public readonly InternalData lastCursor;
	public readonly InternalData lastLockedCursor;
	public bool locked = false;
	public bool inside = true;

	public MouseData() {
		buttons = new bool[ 8 ];
		cursor = new InternalData();
		lockedCursor = new InternalData();
		lastCursor = new InternalData();
		lastLockedCursor = new InternalData();
	}

	public class InternalData {
		public Vector2 pos = new();
		public Vector2 posNDC = new();
		public Vector2 posNDCA = new();
	}
}
