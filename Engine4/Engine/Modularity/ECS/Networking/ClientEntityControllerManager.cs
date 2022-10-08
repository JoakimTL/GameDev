using System.Collections.Concurrent;
using Engine.Modularity.ECS.Components;
using Engine.Networking;
using Engine.Rendering.InputHandling;
using GLFW;

namespace Engine.Modularity.ECS.Networking;
public class ClientEntityControllerManager : ModuleService {

	private readonly NetworkManager _networkManager;
	private readonly EntityManager _entityManager;
	private readonly ComponentOrganizer<ClientInputComponent> _componentOrganizer;
	private readonly ConcurrentQueue<ClientInputComponent> _addQueue;
	private readonly ConcurrentQueue<ClientInputComponent> _removeQueue;
	private readonly HashSet<ClientInputComponent> _components;

	public ClientEntityControllerManager( NetworkManager networkManager, EntityManager entityManager ) {
		this._networkManager = networkManager ?? throw new ArgumentNullException( nameof( networkManager ) );
		this._entityManager = entityManager ?? throw new ArgumentNullException( nameof( entityManager ) );
		this._addQueue = new();
		this._removeQueue = new();
		this._components = new();
		this._componentOrganizer = new ComponentOrganizer<ClientInputComponent>( this._entityManager );
		this._componentOrganizer.OnComponentAdded += ComponentAdded;
		this._componentOrganizer.OnComponentRemoved += ComponentRemoved;
		Resources.GlobalService<ClientInput>().KeyPressed += OnKeyPressed;
		Resources.GlobalService<ClientInput>().KeyReleased += OnKeyReleased;
		Resources.GlobalService<ClientInput>().ButtonPressed += OnButtonPressed;
		Resources.GlobalService<ClientInput>().ButtonReleased += OnButtonReleased;
		Resources.GlobalService<ClientInput>().MovedVisible += OnMouseCursorMove;
		Resources.GlobalService<ClientInput>().MovedHidden += OnMouseLockedMove;
		Resources.GlobalService<ClientInput>().WheelScrolled += OnMouseScroll;
	}

	private void ComponentAdded( Entity e, ClientInputComponent c ) {
		if ( e.Owner != this._networkManager.ServerProvidedId )
			return;
		lock ( this._components )
			this._components.Add( c );
	}

	private void ComponentRemoved( Entity e, ClientInputComponent c ) {
		if ( e.Owner != this._networkManager.ServerProvidedId )
			return;
		lock ( this._components )
			this._components.Remove( c );
	}

	public void OnKeyReleased( Keys key, ModifierKeys mods, int scanCode ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.Set( key, false );
			}
	}

	public void OnKeyPressed( Keys key, ModifierKeys mods, int scanCode ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.Set( key, true );
			}
	}

	public void OnButtonReleased( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.Set( btn, false );
			}
	}

	public void OnButtonPressed( MouseButton btn, ModifierKeys modifier, MouseState state ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.Set( btn, true );
			}
	}

	public void OnMouseCursorMove( MouseState state ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.MouseMoved( state.LastVisible.Position - state.Visible.Position );
			}
	}

	public void OnMouseLockedMove( MouseState state ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.MouseMoved( state.LastHidden.Position - state.LastHidden.Position );
			}
	}

	public void OnMouseScroll( double xAxis, double yAxis, MouseState state ) {
		lock ( this._components )
			foreach ( ClientInputComponent c in this._components ) {
				c.MouseScrolled( (float) yAxis );
			}
	}

	protected override bool OnDispose() => true;

}
