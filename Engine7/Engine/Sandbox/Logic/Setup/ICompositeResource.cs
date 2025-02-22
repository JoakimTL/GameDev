namespace Sandbox.Logic.Setup;

/// <summary>
/// If a type implements this interface, then it's component parts can be accessed by processing the raw resource.
/// </summary>
public interface ICompositeResource {
	public IReadOnlySet<Type> ComponentResources { get; }
}