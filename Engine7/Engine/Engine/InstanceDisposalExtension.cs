namespace Engine;

public sealed class InstanceDisposalExtension( IInstanceProvider instanceProvider ) : InstanceProviderExtensionBase<IDisposable, IDisposable>( instanceProvider ), IDisposable {
	public void Dispose() {
		foreach (IDisposable disposable in this.SortedInstances)
			disposable.Dispose();
	}
}
