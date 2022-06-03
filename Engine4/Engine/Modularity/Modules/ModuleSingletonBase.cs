namespace Engine.Modularity.Modules;

public abstract class ModuleSingletonBase : DisposableIdentifiable {
	private readonly Module? _owner;
	protected Module Owner => this._owner ?? throw new NullReferenceException( nameof( this._owner ) );
}