namespace Engine;

public sealed class InstanceUpdaterExtension( IInstanceProvider instanceProvider ) : InstanceProviderExtensionBase<IUpdateable, IUpdateable>( instanceProvider ), IUpdateable {
	public void Update( double time, double deltaTime ) {
		foreach (IUpdateable updateable in this.SortedInstances)
			updateable.Update( time, deltaTime );
	}
}
