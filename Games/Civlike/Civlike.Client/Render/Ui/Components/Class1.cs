using Engine.Module.Render.Input;
using Engine.Standard.Render.UserInterface.Standard;
using Engine.Standard.Render.UserInterface;
using Engine;

namespace Civlike.Client.Render.Ui.Components;

public sealed class PlayerChoiceButton : ValueDisplayComponentBase<PlayerChoiceButton, PlayerChoice> {
	private readonly VisualButton _visuals;

	public PlayerChoiceButton( UserInterfaceElementBase element, PlayerChoice? value ) : base( element, value ) {
		this._visuals = AddChild( new VisualButton( element, value?.ToString() ?? "No value", "calibrib" ) );
		this._visuals.Background.Color = value?.Color.CastSaturating<float, double>() ?? 1;

		this.ClickEnabled = true;
		OnMouseEntered += DefaultOnEnter;
		OnMouseExited += DefaultOnExit;
		OnPressed += DefaultOnPressed;
		OnReleased += DefaultOnReleased;
	}

	protected override void OnPlacementChanged() { }

	protected override void DoHide() { }

	protected override void DoShow() { }

	protected override void OnUpdate( double time, double deltaTime ) { }

	private static void DefaultOnExit( PlayerChoiceButton button, MouseMoveEvent @event )
		=> button._visuals.Background.Color = button.Value?.Color.CastSaturating<float, double>() ?? 1;
	private static void DefaultOnEnter( PlayerChoiceButton button, MouseMoveEvent @event )
		=> button._visuals.Background.Color = button.Value?.Color.CastSaturating<float, double>().MultiplyEntrywise( (0.9, 0.9, 0.9, 1) ) ?? (.9, .9, .9, 1);
	private static void DefaultOnPressed( PlayerChoiceButton button, MouseButtonEvent @event )
		=> button._visuals.Background.Color = button.Value?.Color.CastSaturating<float, double>().MultiplyEntrywise( (0.75, 0.75, 0.75, 1) ) ?? (.75, .75, .75, 1);
	private static void DefaultOnReleased( PlayerChoiceButton button, MouseButtonEvent @event )
		=> button._visuals.Background.Color = button.Value?.Color.CastSaturating<float, double>() ?? 1;
}
