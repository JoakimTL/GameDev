namespace Engine;

public sealed class InstanceInitializerExtension( IInstanceProvider instanceProvider ) : InstanceProviderExtensionBase<IInstanceProvider, IInitializable>( instanceProvider ), IUpdateable {
	private readonly List<Type> _initializedInstanceTypes = [];

	public void Update( double time, double deltaTime ) {
		foreach (IInitializable initializable in this.SortedInstances) {
			initializable.Initialize();
			this._initializedInstanceTypes.Add( initializable.GetType() );
		}

		if (this._initializedInstanceTypes.Count == this.SortedInstances.Count) {
			Clear();
		} else
			RemoveAll( this._initializedInstanceTypes );

		this._initializedInstanceTypes.Clear();
	}
}