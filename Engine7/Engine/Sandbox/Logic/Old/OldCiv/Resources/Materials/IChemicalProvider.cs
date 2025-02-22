namespace Sandbox.Logic.Old.OldCiv.Resources.Materials;

public interface IChemicalProvider {
	static abstract IReadOnlyList<Chemical> Chemicals { get; }
}