﻿using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;
public sealed class UserInterfaceStateManager( UserInterfaceServiceAccess userInterfaceServiceAccess, GameStateProvider gameStateProvider ) : DisposableIdentifiable, ICapturingUserInputListener, IUpdateable {

	private readonly List<UserInterfaceElementBase> _baseElements = [];
	private readonly Queue<UserInterfaceElementBase> _elementsToInitialize = [];
	private readonly UserInterfaceServiceAccess _userInterfaceServiceAccess = userInterfaceServiceAccess;
	private readonly GameStateProvider _gameStateProvider = gameStateProvider;

	public void AddElement<T>() where T : UserInterfaceElementBase, new() {
		UserInterfaceElementBase element = new T();
		this._baseElements.Add( element );
		this._elementsToInitialize.Enqueue( element );
		element.SetServiceAccess( _userInterfaceServiceAccess );
	}

	public bool OnCharacter( KeyboardCharacterEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnCharacter( @event ))
				return true;
		}
		return false;
	}

	public bool OnKey( KeyboardEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnKey( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseButton( MouseButtonEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseButton( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseEnter( MouseEnterEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseEnter( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseMoved( MouseMoveEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseMoved( @event ))
				return true;
		}
		return false;
	}

	public bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		foreach (UserInterfaceElementBase element in this._baseElements) {
			if (!element.IsDisplayed)
				continue;
			if (element.OnMouseWheelScrolled( @event ))
				return true;
		}
		return false;
	}

	public void Update( double time, double deltaTime ) {
		while (this._elementsToInitialize.Count > 0) {
			UserInterfaceElementBase element = this._elementsToInitialize.Dequeue();
			element.Initialize( _gameStateProvider );
		}
		foreach (UserInterfaceElementBase element in this._baseElements) {
			element.UpdateDisplayState( _gameStateProvider );
			if (element.IsDisplayed)
				element.Update( time, deltaTime );
		}
	}

	protected override bool InternalDispose() {
		foreach (UserInterfaceElementBase element in this._baseElements)
			element.Dispose();
		return true;
	}
}