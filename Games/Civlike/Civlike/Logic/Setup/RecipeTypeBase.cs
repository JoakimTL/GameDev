namespace Civlike.Logic.Setup;

public abstract class RecipeTypeBase( string name, StaticResourceBundle input, StaticResourceBundle output, IEnumerable<ResourceTypeBase> byproducts ) : SelfIdentifyingBase {
	public string Name { get; } = name;
	public StaticResourceBundle Input { get; } = input;
	public StaticResourceBundle Output { get; } = output;
	public IReadOnlyList<ResourceTypeBase> Byproducts { get; } = byproducts.ToList().AsReadOnly();

	public bool Process( ResourceContainer container, bool keepByproducts ) {
		if (!container.Subtract( this.Input ))
			return false;
		if (keepByproducts) {
			container.Add( this.Output );
			return true;
		}
		container.AddAllExcept( this.Output, this.Byproducts );
		return true;
	}
}

