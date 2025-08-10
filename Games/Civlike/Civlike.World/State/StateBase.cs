namespace Civlike.World.State;

public abstract class StateBase<TContainer>
	where TContainer : StateContainerBase<TContainer> {
	public TContainer StateContainer { get; private set; } = null!;

	internal void SetStateContainer( TContainer stateContainer )
		=> StateContainer = stateContainer;

	public void InvokeStateChanged()
		=> StateContainer.InvokeStateChanged( this );
}
