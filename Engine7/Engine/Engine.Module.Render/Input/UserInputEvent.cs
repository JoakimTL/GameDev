namespace Engine.Module.Render.Input;
public unsafe struct UserInputEvent : IUserInputEvent {
	public double Time { get; }
	public EventType EventType { get; }
	public fixed byte Content[ 128 ];
}
