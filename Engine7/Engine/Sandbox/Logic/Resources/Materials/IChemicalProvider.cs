namespace Sandbox.Logic.Resources.Materials;

public interface IChemicalProvider {
	static abstract IReadOnlyList<Chemical> Chemicals { get; }
}