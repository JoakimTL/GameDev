using System.Numerics;

namespace StandardPackage.ECS.Systems;

public interface IGravity3ValueProvider {
	/// <summary>
	/// Whether or not the value from <see cref="GetGravity(Vector3)"/> is an acceleration-vector <b>(true)</b> or force-vector <b>(false)</b>.<br/>
	/// </summary>
	public bool IsAcceleration { get; }
	Vector3 GetGravity( Vector3 globalTranslation );
}