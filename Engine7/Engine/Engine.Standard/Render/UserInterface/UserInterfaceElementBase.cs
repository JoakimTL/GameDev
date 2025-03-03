﻿using Engine.Module.Render.Input;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceElementBase(uint baseLayer = 0) : DisposableIdentifiable {

	private readonly List<UserInterfaceComponentBase> _components = [];

	public UserInterfaceServiceAccess UserInterfaceServiceAccess { get; private set; } = null!;
	public GameStateProvider GameStateProvider { get; private set; } = null!;

	public bool IsDisplayed { get; private set; }

	public uint BaseLayer { get; } = baseLayer;

	internal protected abstract void Initialize();
	protected abstract void OnUpdate( double time, double deltaTime );

	/// <summary>
	/// Determines if the element should be displayed based on the current game state.
	/// </summary>
	protected abstract bool ShouldDisplay();

	public bool UpdateDisplayState() {
		bool oldValue = this.IsDisplayed;
		this.IsDisplayed = this.ShouldDisplay();
		return oldValue != this.IsDisplayed;
	}
	protected void AddComponent( UserInterfaceComponentBase component )
		=> this._components.Add( component );

	internal void Hide() {
		foreach (UserInterfaceComponentBase component in this._components)
			component.Hide();
	}

	internal void Show() {
		foreach (UserInterfaceComponentBase component in this._components)
			component.Show();
	}

	internal void UiSpaceChanged( Vector2<double> newAspectVector ) {
		foreach (UserInterfaceComponentBase component in this._components)
			component.UiSpaceChanged( newAspectVector );
	}

	internal bool OnMouseButton( MouseButtonEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseButton( @event ))
				return true;
		}
		return false;
	}

	internal bool OnMouseWheelScrolled( MouseWheelEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseWheelScrolled( @event ))
				return true;
		}
		return false;
	}

	internal bool OnMouseMoved( MouseMoveEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseMoved( @event ))
				return true;
		}
		return false;
	}

	internal bool OnMouseEnter( MouseEnterEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnMouseEnter( @event ))
				return true;
		}
		return false;
	}

	internal bool OnKey( KeyboardEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnKey( @event ))
				return true;
		}
		return false;
	}

	internal bool OnCharacter( KeyboardCharacterEvent @event ) {
		foreach (UserInterfaceComponentBase component in this._components) {
			if (component.OnCharacter( @event ))
				return true;
		}
		return false;
	}

	internal void Update( double time, double deltaTime ) {
		OnUpdate(time, deltaTime);
		foreach (UserInterfaceComponentBase component in this._components)
			component.Update( time, deltaTime );
	}

	internal void SetServices( UserInterfaceServiceAccess userInterfaceServiceAccess, GameStateProvider gameStateProvider ) {
		this.UserInterfaceServiceAccess = userInterfaceServiceAccess;
		this.GameStateProvider = gameStateProvider;
	}

	protected override bool InternalDispose() {
		foreach (UserInterfaceComponentBase component in this._components)
			component.Dispose();
		return true;
	}
}
