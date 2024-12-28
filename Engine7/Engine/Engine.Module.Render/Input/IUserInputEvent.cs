namespace Engine.Module.Render.Input;

public interface IUserInputEvent {
	double Time { get; }
	EventType EventType { get; }
}
