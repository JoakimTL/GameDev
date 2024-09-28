using Engine.Time;

namespace Engine.Tests.Time;

[TestFixture]
public sealed class ClockTests {
	public sealed class TestTickSupplier : ITickSupplier {
		public static long Frequency { get; set; }
		public static long Ticks { get; set; }
	}

	[Test]
	public void SingleSession_Realtime_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 1 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2500;

		Assert.That( clock.Time, Is.EqualTo( 2.5 ).Within( 0.001 ) );
	}

	[Test]
	public void MultipleSessions_Realtime_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 1 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Paused = true;
		clock.Paused = true;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2000;

		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Paused = false;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 3000;

		Assert.That( clock.Time, Is.EqualTo( 2 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 4500;

		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 2.5 ).Within( 0.001 ) );
	}

	[Test]
	public void SingleSession_FasterThanRealtime_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 2 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 2 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2500;

		Assert.That( clock.Time, Is.EqualTo( 5 ).Within( 0.001 ) );
	}

	[Test]
	public void MultipleSessions_FasterThanRealtime_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 2 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 2 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 2 ).Within( 0.001 ) );

		clock.Paused = true;
		clock.Paused = true;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2000;

		Assert.That( clock.Time, Is.EqualTo( 2 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 2 ).Within( 0.001 ) );

		clock.Paused = false;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 3000;

		Assert.That( clock.Time, Is.EqualTo( 4 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 2 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 4500;

		Assert.That( clock.Time, Is.EqualTo( 7 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 5 ).Within( 0.001 ) );
	}

	[Test]
	public void SingleSession_SlowerThanRealtime_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 0.5 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 0.5 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2500;

		Assert.That( clock.Time, Is.EqualTo( 1.25 ).Within( 0.001 ) );
	}

	[Test]
	public void MultipleSessions_SlowerThanRealtime_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 0.5 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 0.5 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 0.5 ).Within( 0.001 ) );

		clock.Paused = true;
		clock.Paused = true;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2000;

		Assert.That( clock.Time, Is.EqualTo( 0.5 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 0.5 ).Within( 0.001 ) );

		clock.Paused = false;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 3000;

		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 0.5 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 4500;

		Assert.That( clock.Time, Is.EqualTo( 1.75 ).Within( 0.001 ) );
		Assert.That( clock.Session, Is.EqualTo( 1.25 ).Within( 0.001 ) );
	}

	[Test]
	public void MultipleSessions_Speedchange_ElapsedTime() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 1 );

		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Dilation = 2;

		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2000;

		Assert.That( clock.Time, Is.EqualTo( 3 ).Within( 0.001 ) );

		clock.Dilation = 0.5;

		Assert.That( clock.Time, Is.EqualTo( 3 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 3000;

		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );

		clock.Paused = true;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 4000;

		Assert.That( clock.Session, Is.EqualTo( 0.5 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );

		clock.Dilation = 0.25;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 5000;

		Assert.That( clock.Session, Is.EqualTo( 0.25 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );

		clock.Paused = false;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 3.5 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 6000;

		Assert.That( clock.Session, Is.EqualTo( 0.25 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 3.75 ).Within( 0.001 ) );
	}

	[Test]
	public void MultipleSessions_Realtime_Get() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 0;
		Clock<double, TestTickSupplier> clock = new( 1 );

		clock.Get( out double totalTime, out double sessionTime );

		Assert.That( totalTime, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( sessionTime, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 1000;

		clock.Get( out totalTime, out sessionTime );

		Assert.That( totalTime, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( sessionTime, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Paused = true;
		clock.Paused = true;

		clock.Get( out totalTime, out sessionTime );

		Assert.That( sessionTime, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( totalTime, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 2000;

		clock.Get( out totalTime, out sessionTime );

		Assert.That( sessionTime, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( totalTime, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Paused = false;

		clock.Get( out totalTime, out sessionTime );

		Assert.That( sessionTime, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( totalTime, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 3000;

		clock.Get( out totalTime, out sessionTime );

		Assert.That( sessionTime, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( totalTime, Is.EqualTo( 2 ).Within( 0.001 ) );
	}

	[Test]
	public void MultipleSessions_Realtime_CreationClockTickOffset_Time() {
		TestTickSupplier.Frequency = 1000;
		TestTickSupplier.Ticks = 8912;
		Clock<double, TestTickSupplier> clock = new( 1 );

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 0 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 9912;

		Assert.That( clock.Session, Is.EqualTo( 1 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Paused = true;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 10412;

		Assert.That( clock.Session, Is.EqualTo( 0.5 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		clock.Paused = false;

		Assert.That( clock.Session, Is.EqualTo( 0 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 1 ).Within( 0.001 ) );

		TestTickSupplier.Ticks = 10912;

		Assert.That( clock.Session, Is.EqualTo( 0.5 ).Within( 0.001 ) );
		Assert.That( clock.Time, Is.EqualTo( 1.5 ).Within( 0.001 ) );

	}
}
