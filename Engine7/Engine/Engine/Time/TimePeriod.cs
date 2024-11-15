using System.Runtime.InteropServices;

namespace Engine.Time;

internal sealed class TimePeriod : IDisposable {

	private const int WINDEFAULTPERIOD = 16;

	private const string WINMM = "winmm.dll";

	private static TIMECAPS _timeCapabilities;

	private static int _inTimePeriod;

	static TimePeriod() {
		int result = timeGetDevCaps( ref _timeCapabilities, Marshal.SizeOf( typeof( TIMECAPS ) ) );
		if (result != 0) {
			throw new InvalidOperationException( "The request to get time capabilities was not completed because an unexpected error with code " + result + " occured." );
		}
	}

	[DllImport( WINMM, ExactSpelling = true )]
	private static extern int timeGetDevCaps( ref TIMECAPS ptc, int cbtc );

	[DllImport( WINMM, ExactSpelling = true )]
	private static extern int timeBeginPeriod( int uPeriod );

	[DllImport( WINMM, ExactSpelling = true )]
	private static extern int timeEndPeriod( int uPeriod );

	internal static int MinimumPeriod => _timeCapabilities.wPeriodMin;

	internal static int MaximumPeriod => _timeCapabilities.wPeriodMax;

	private static TimePeriod? _currentPeriod;

	public static int CurrentPeriod => _currentPeriod?.Period ?? WINDEFAULTPERIOD;

	internal static void Begin( int period ) {
		if (period > WINDEFAULTPERIOD) {
			_currentPeriod?.Dispose();
			_currentPeriod = null;
			return;
		}
		_currentPeriod?.Dispose();
		_currentPeriod = new( period );
	}

	private readonly int _period;

	private bool _disposed;

	private TimePeriod( int period ) {
		if (Interlocked.Increment( ref _inTimePeriod ) != 1) {
			Interlocked.Decrement( ref _inTimePeriod );
			throw new NotSupportedException( "The process is already within a time period. Nested time periods are not supported." );
		}

		if (period < _timeCapabilities.wPeriodMin || period > _timeCapabilities.wPeriodMax) {
			throw new ArgumentOutOfRangeException( "period", "The request to begin a time period was not completed because the resolution specified is out of range." );
		}

		int result = timeBeginPeriod( period );
		if (result != 0) {
			throw new InvalidOperationException( "The request to begin a time period was not completed because an unexpected error with code " + result + " occured." );
		}

		this._period = period;
	}


	internal int Period {
		get {
			if (this._disposed) {
				throw new ObjectDisposedException( "The time period instance has been disposed." );
			}

			return this._period;
		}
	}

	public void Dispose() {
		if (this._disposed)
			return;
		this._disposed = true;
		timeEndPeriod( this._period );
		Interlocked.Decrement( ref _inTimePeriod );
	}

	[StructLayout( LayoutKind.Sequential )]
	private struct TIMECAPS {
		internal int wPeriodMin;

		internal int wPeriodMax;
	}
}