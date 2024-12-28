namespace Engine.Module.Render.Input;

public readonly struct MouseEnterEvent( double time, bool enterState ) {
	public readonly double Time = time;
	public readonly bool State = enterState;
}
