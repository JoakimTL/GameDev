using Engine.Time;

namespace Engine.Tests.Time;

[TestFixture]
public sealed class ThreadBlockerTests {
	[Test]
	public void Cancel() {
		IThreadBlocker blocker = new ThreadBlocker();
		blocker.Cancel();
		Assert.That( blocker.Cancelled, Is.True );
		Assert.That( blocker.Block( 0 ), Is.False );
	}

	[Test]
	public void Dispose() {
		ThreadBlocker blocker = new();
		blocker.Dispose();

		Assert.That( blocker.Cancelled, Is.True );
		Assert.That( blocker.Block( 0 ), Is.False );
	}
}