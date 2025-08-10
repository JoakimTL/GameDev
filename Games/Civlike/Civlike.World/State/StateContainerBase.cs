using System.Diagnostics.CodeAnalysis;

namespace Civlike.World.State;

public abstract class StateContainerBase<TContainer>
	where TContainer : StateContainerBase<TContainer> {
	private readonly Dictionary<Type, StateBase<TContainer>> _states = [];
	public event Action<StateBase<TContainer>>? StateChanged;

	public TState GetStateOrThrow<TState>() where TState : StateBase<TContainer> {
		if (!_states.TryGetValue( typeof( TState ), out StateBase<TContainer>? state ))
			throw new ArgumentException( "State of type " + typeof( TState ) + " does not exist." );
		return (TState) state;
	}

	public TState? GetState<TState>() where TState : StateBase<TContainer> {
		if (!_states.TryGetValue( typeof( TState ), out StateBase<TContainer>? state ))
			return null;
		return (TState) state;
	}

	public bool TryGetState<TState>( [NotNullWhen( true )] out TState? state ) where TState : StateBase<TContainer> {
		state = null;
		if( !_states.TryGetValue( typeof( TState ), out StateBase<TContainer>? stateBase ))
			return false;
		state = (TState) stateBase;
		return true;
	}

	public void AddState<TState>( TState state ) where TState : StateBase<TContainer> {
		if (!_states.TryAdd( typeof( TState ), state ))
			throw new ArgumentException( "State of type " + typeof( TState ) + " already exists." );
		TContainer container = this as TContainer ?? throw new InvalidOperationException();
		state.SetStateContainer( container );
	}

	internal void InvokeStateChanged( StateBase<TContainer> nodeState )
		=> StateChanged?.Invoke( nodeState );
}