namespace Engine.Modularity;
internal class FileName {
}

/// <summary>
/// Used to indicate you want this to be initialized after all modules have been initialized.
/// </summary>
public interface IModuleModification {
	void ModifyModule( ModuleBase module );
}