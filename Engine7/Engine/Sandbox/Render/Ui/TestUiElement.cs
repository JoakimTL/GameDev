using Engine.Standard;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Render.Ui;
public sealed class TestUiElement : UserInterfaceElementBase {
	protected override void Initialize( GameStateProvider gameStateProvider ) {
		AddComponent( new Button( this, "test!", new( 0, 0, .25 ), (1, 1, 1, 1), (0.77, .77, .77, 1), (.5, .5, .5, 1) ) );
	}

	protected override bool ShouldDisplay( GameStateProvider gameStateProvider ) {
		return true;
	}
}
