using Engine.Module.Entities.Container;

namespace Civlike.Logic.Nations;
public sealed class PopulationCenterComponent : ComponentBase {

	public string Name { get; private set; }

	public PopulationCenterComponent() {
		Name = $"Pop {Random.Shared.Next():X8}";
	}

	public void SetName( string name ) {
		if (string.IsNullOrWhiteSpace( name ))
			throw new ArgumentException( "Name cannot be null or whitespace.", nameof( name ) );
		if (name == Name)
			return;
		Name = name;
		InvokeComponentChanged();
	}

}
