using Engine;
using Engine.Modules;

namespace UserTest;

[ThreadPriority( ThreadPriority.AboveNormal )]
internal class ModuleTest : ModuleBase {
	public ModuleTest() : base( new FrequencyDelayedLoopTimer( 20 ), new( 1 ), null ) {
	//public ModuleTest() : base( new NoDelayLoopTimer(), new( 1 ) ) {
	}

	protected override void OnInitialize() {
		this.LogLine( "Start!" );
	}

	protected override void OnUpdate( in double time, in double deltaTime ) {
		this.LogLine( $"Update! {time:N4}s" );
		if (time > 10) {
			this.LogLine( $"Stop! {time:N4}s" );
			Stop();
		}

	}

	protected override void OnDispose() {
		this.LogLine( $"Dispose!" );
	}
}
