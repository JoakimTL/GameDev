namespace Engine;

public interface IUpdateable {
	bool Active { get; }
	void Update( float time, float deltaTime );
}
