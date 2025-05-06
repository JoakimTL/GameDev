namespace Civs.Logic.Setup;

public abstract class RecipeTypeBase : SelfIdentifyingBase {
	public string Name { get; }
	public StaticResourceBundle Input { get; }
	public StaticResourceBundle Output { get; }
	public IReadOnlyList<ResourceTypeBase> Byproducts { get; }

	protected RecipeTypeBase( string name, StaticResourceBundle input, StaticResourceBundle output, IEnumerable<ResourceTypeBase> byproducts ) {
		this.Name = name;
		this.Input = input;
		this.Output = output;
		this.Byproducts = byproducts.ToList().AsReadOnly();
	}

	public bool Process( ResourceContainer container, bool keepByproducts ) {
		if (!container.Subtract( Input ))
			return false;
		if (keepByproducts) {
			container.Add( Output );
			return true;
		}
		container.AddAllExcept( Output, Byproducts );
		return true;
	}
}

