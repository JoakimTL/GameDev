using Engine.GameLogic.ECPS;
using Engine.GameLogic.ECPS.Components;
using StandardPackage.ECPS.Components;

namespace StandardPackage.ECPS.Processors;
public class TestProcessor : ProcessorBase<TestCompositeComponent> {
	public TestProcessor() : base( typeof( RotationalMovement3Component ), typeof( Transform3Component ) ) {
	}
}
