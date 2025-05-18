namespace Engine.Standard.Render.UserInterface.Standard;

public abstract class ValueDisplayComponentBase<TSelf, TValue>( UserInterfaceElementBase element, TValue? value ) : InteractableUserInterfaceComponentBase<TSelf>( element ) where TSelf : ValueDisplayComponentBase<TSelf, TValue> where TValue : class {
	public TValue? Value { get; } = value;
}