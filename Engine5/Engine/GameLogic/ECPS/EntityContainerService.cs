using Engine.Structure.Interfaces;

namespace Engine.GameLogic.ECPS;

public sealed class EntityContainerService : Identifiable, IGameLogicService, IUpdateable {

	public readonly EntityContainer _container;
	protected override string UniqueNameTag => _container.ToString();

	public EntityContainerService() {
		_container = new();
	}

    public void Update( float time, float deltaTime ) => _container.Update( time, deltaTime );

    public IEnumerable<T> GetComponents<T>() where T : ComponentBase
		=> _container.GetComponents<T>();

	//Update manipulator list
	// (Check if all active manipulators are valid still)
	// (Check is any new components added validates other manipulators)
	//Run Update on manipulators

}

