using Engine;
using Engine.Modularity;

namespace Sandbox;
internal class TestModule() : ModuleBase( true, 100 ) {
	protected override void Tick( double time, double deltaTime ) {
		this.LogLine( $"Time: {time}, DeltaTime: {deltaTime}" );
		if (time > 5)
			Stop();
	}
}
