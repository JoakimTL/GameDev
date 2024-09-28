using UserTest.AITest.Actions.Actions;

namespace UserTest.AITest.Actions;

public sealed class ActionPath : IAgentAction {
	private readonly LinkedList<IAgentAction> _actions;
	private LinkedListNode<IAgentAction>? _currentActionNode;
	private MoveTo3 _moveTo3;

	public ActionPath( IEnumerable<IAgentAction> actions ) {
		_actions = new LinkedList<IAgentAction>( actions );
		_currentActionNode = _actions.First;
	}

	public ActionPath( MoveTo3 moveTo3 ) {
		this._moveTo3 = moveTo3;
	}

	public ActionState PerformAction() {
		if ( _currentActionNode is null)
			return ActionState.NoAction;
		ActionState state = _currentActionNode.Value.PerformAction();
		if (state == ActionState.Failed)
			return ActionState.Failed;
		if (state == ActionState.Completed) {
			_currentActionNode = _currentActionNode.Next;
			if (_currentActionNode is null)
				return ActionState.Completed;
		}
		if (state == ActionState.Reevaluate)
			return ActionState.Reevaluate;
		return ActionState.Performing;
	}
}