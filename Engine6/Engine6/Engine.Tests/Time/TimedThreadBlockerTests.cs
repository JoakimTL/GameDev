using Engine.Time;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Engine.Tests.Time;

[TestFixture]
public sealed class TimedThreadBlockerTests {

	public sealed class TestTickSupplier : ITickSupplier {
		public static long Frequency { get; set; }
		public static long Ticks { get; set; }
	}

	[Test]
	public void Block_VariableWorkload_20MsPeriod() {
		IThreadBlocker blocker = Substitute.For<IThreadBlocker>();
		blocker.Block( Arg.Any<uint>() ).Returns( true );
		TestTickSupplier.Ticks = 0;
		TestTickSupplier.Frequency = 1000;
		TimedThreadBlocker<TestTickSupplier> timedBlocker = new( blocker, 20 );
		TimedBlockerState state;

		TestTickSupplier.Ticks = 7;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 13 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 25;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 15 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 55;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 5 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 85;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.NonBlocking ) );
		blocker.DidNotReceive().Block( Arg.Any<uint>() );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 95;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 5 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 108;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 12 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 165;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Skipping ) );
		blocker.DidNotReceive().Block( Arg.Any<uint>() );
		blocker.ClearReceivedCalls();
	}

	[Test]
	public void Block_AfterSet_20MsPeriod() {
		IThreadBlocker blocker = Substitute.For<IThreadBlocker>();
		blocker.Block( Arg.Any<uint>() ).Returns( true );
		TestTickSupplier.Ticks = 0;
		TestTickSupplier.Frequency = 1000;
		TimedThreadBlocker<TestTickSupplier> timedBlocker = new( blocker, 20 );
		TimedBlockerState state;

		timedBlocker.Set();
		TestTickSupplier.Ticks = 7;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 13 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 20;
		timedBlocker.Set();

		TestTickSupplier.Ticks = 25;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 15 ) );
		blocker.ClearReceivedCalls();

		TestTickSupplier.Ticks = 60;
		timedBlocker.Set();

		TestTickSupplier.Ticks = 67;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 13 ) );
		blocker.ClearReceivedCalls();
	}

	[Test]
	public void Block_Cancelled() {
		IThreadBlocker blocker = new ThreadBlocker();
		TestTickSupplier.Ticks = 0;
		TestTickSupplier.Frequency = 1000;
		TimedThreadBlocker<TestTickSupplier> timedBlocker = new( blocker, 20 );
		TimedBlockerState state;

		Assert.That( blocker.Cancelled, Is.False );

		blocker.Cancel();
		TestTickSupplier.Ticks = 7;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Cancelled ) );
		Assert.That( blocker.Cancelled, Is.True );
	}

	[Test]
	public void Block_Cancelled_MidWait() {
		IThreadBlocker blocker = new ThreadBlocker();
		TestTickSupplier.Ticks = 0;
		TestTickSupplier.Frequency = 1000;
		TimedThreadBlocker<TestTickSupplier> timedBlocker = new( blocker, 500 );
		TimedBlockerState state;

		TestTickSupplier.Ticks = 5;
		Task.Run( async () => {
			await Task.Delay( 250 );
			blocker.Cancel();
		} );
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Cancelled ) );
		Assert.That( blocker.Cancelled, Is.True );
	}

	[Test]
	public void Dispose() {
		IThreadBlocker blocker = new ThreadBlocker();
		TestTickSupplier.Ticks = 0;
		TestTickSupplier.Frequency = 1000;
		TimedThreadBlocker<TestTickSupplier> timedBlocker = new( blocker, 20 );

		timedBlocker.Dispose();

		Assert.That( blocker.Cancelled, Is.True );
	}

	[Test]	
	public void Block_VariableWorkload_VariablePeriod() {
		IThreadBlocker blocker = Substitute.For<IThreadBlocker>();
		blocker.Block( Arg.Any<uint>() ).Returns( true );
		TestTickSupplier.Ticks = 0;
		TestTickSupplier.Frequency = 1000;
		TimedThreadBlocker<TestTickSupplier> timedBlocker = new( blocker, 20 );
		TimedBlockerState state;

		TestTickSupplier.Ticks = 7;
		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 13 ) );
		blocker.ClearReceivedCalls();

		timedBlocker.SetPeriod( 10 );
		TestTickSupplier.Ticks = 25;

		state = timedBlocker.Block();

		Assert.That( state, Is.EqualTo( TimedBlockerState.Blocking ) );
		blocker.Received().Block( Arg.Is<uint>( 5 ) );
		blocker.ClearReceivedCalls();
	}

	[Test]
	public void Throws_NullBlocker() {
		Assert.Throws<ArgumentNullException>( () => new TimedThreadBlocker<TestTickSupplier>( null!, 20 ) );
	}

}
