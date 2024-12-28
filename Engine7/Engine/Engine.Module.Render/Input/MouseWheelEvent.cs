namespace Engine.Module.Render.Input;

public readonly struct MouseWheelEvent( double time, double xAxis, double yAxis ) {
	public readonly double Time = time;
	public readonly Vector2<double> Movement = new( xAxis, yAxis );
}
