namespace Engine.GlobalServices.LoggedInput;
public sealed class TimedEvent {

	public float Time { get; }
	public EventBase Event { get; }

	public TimedEvent(float time, EventBase @event) {
		this.Time = time;
		this.Event = @event;
	}

}
