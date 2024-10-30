namespace Engine;

public interface IInstanceLibrary {

	/// <summary>
	/// Assigns a implementation to a contract, the <see cref="{TImplementation}"/> type must be instantiable (not abstract or an interface).
	/// </summary>
	/// <returns>True if the contract could be connected to the implementation. If the contract is already assigned this method returns false.</returns>
	bool Connect<TContract, TImplementation>() where TImplementation : TContract;

}
