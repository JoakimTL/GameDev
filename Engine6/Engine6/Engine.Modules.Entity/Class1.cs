namespace Engine.Modules.Entity;

public class Class1 {

}

public interface IComponentSerializable {

	abstract static Guid ComponentType { get; }

	

}

public interface INetworkSerializable : IComponentSerializable {

	bool HasChanges { get; }
	

}