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

	private Button _btnExit = null!;

	protected override void Initialize() {
		AddComponent( new Button( this, "New Game", "calibrib", new( (-.7, .6), 0, (.25, .1) ), (1, 1, 1, 1), (0.77, .77, .77, 1), (.5, .5, .5, 1) ) );
		AddComponent( _btnExit = new Button( this, "Exit", "calibrib", new( (-.7, .3), 0, (.25, .1) ), (1, 1, 1, 1), (0.77, .77, .77, 1), (.5, .5, .5, 1) ) );
		_btnExit.ButtonClicked += OnExitButtonClicked;
	}

	private void OnExitButtonClicked() {
		GameStateProvider.Set( "closegame", true );
	}

	protected override bool ShouldDisplay() {
		return true;
	}
}
