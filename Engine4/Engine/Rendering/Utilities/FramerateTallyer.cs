using Engine.Time;

namespace Engine.Rendering.Utilities;

internal class FramerateTallyer {

	private int _current;
	private readonly float[] _deltaTimes;
	public float AverageDeltaTime { get; private set; }
	public float AverageFramesPerSecond { get; private set; }
	private float _lastTime;
	private readonly Clock32 _internalClock;

	public FramerateTallyer( int count ) {
		this._deltaTimes = new float[ count ];
		this._internalClock = new Clock32();
	}

	public void Tally() {
		float time = this._internalClock.Time;
		float deltaTime = time - this._lastTime;
		this._lastTime = time;
		this._deltaTimes[ this._current++ ] = deltaTime;
		if ( this._current >= this._deltaTimes.Length )
			this._current = 0;
		this.AverageDeltaTime = 0;
		for ( int i = 0; i < this._deltaTimes.Length; i++ )
			this.AverageDeltaTime += this._deltaTimes[ i ];
		this.AverageDeltaTime /= this._deltaTimes.Length;
		this.AverageFramesPerSecond = 1f / this.AverageDeltaTime;
	}
}
