using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Input;

namespace Engine.Standard.Render.Entities.Services;

public sealed class RenderEntityContainerService : DisposableIdentifiable, IUpdateable {

	private readonly Dictionary<EntityContainer, RenderEntityContainer> _containerPairs = [];
	private readonly RenderEntityServiceAccess _renderEntityServiceAccess;
	private readonly UserInputEventService _userInput;

	public RenderEntityContainerService( RenderEntityServiceAccess renderEntityServiceAccess, UserInputEventService userInput ) {
		this._renderEntityServiceAccess = renderEntityServiceAccess;
		this._userInput = userInput;
		userInput.OnCharacter += (e) => this.LogLine( $"Character input received {e.Character} {e.KeyCode} {e.Modifiers} {e.Time}", Log.Level.VERBOSE );
	}

	internal void RegisterEntityContainer( EntityContainer entityContainer ) {
		if (this._containerPairs.ContainsKey( entityContainer )) {
			this.LogLine( $"{entityContainer} already registered.", Log.Level.VERBOSE );
			return;
		}
		this._containerPairs.Add( entityContainer, new RenderEntityContainer( entityContainer, this._renderEntityServiceAccess ) );
		entityContainer.OnDisposed += OnContainerDisposed;
	}

	private void OnContainerDisposed( IListenableDisposable disposable ) {
		List<EntityContainer> containersToRemove = this._containerPairs.Keys.Where( p => p.Disposed ).ToList();

		foreach (EntityContainer container in containersToRemove)
			this._containerPairs.Remove( container );
	}

	protected override bool InternalDispose() {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in this._containerPairs)
			pair.Value.Dispose();
		return true;
	}

	public void Update( double time, double deltaTime ) {
		foreach (KeyValuePair<EntityContainer, RenderEntityContainer> pair in this._containerPairs)
			pair.Value.Update( time, deltaTime );
	}
}