namespace Engine.Module.Render.Input;

public readonly struct MouseMoveEvent( double time, double xAxis, double yAxis ) {
	public readonly double Time = time;
	public readonly Vector2<double> Position = new( xAxis, yAxis );
}
