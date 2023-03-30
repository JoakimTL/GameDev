using Engine;
using Engine.GameLogic.ECPS;
using Engine.Rendering.Contexts.Objects.Scenes;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using System.Reflection;

namespace StandardPackage.ECPS.Components;

public sealed class RenderSceneComponent : ComponentBase, ICustomizedSerializable
{
	public static Guid SerializationIdentity { get; } = new("cd0dfbda-b786-490f-abb0-2347ad4a7478");
	public bool ShouldSerialize => true;

	private Type? _currentSceneType;
	public string? SceneIdentity { get; private set; }

	public void SetScene<T>() where T : Scene, new()
	{
		if (typeof(T) != _currentSceneType)
		{
			_currentSceneType = typeof(T);
			SceneIdentity = _currentSceneType.GetCustomAttribute<IdentityAttribute>()?.Identity;
			AlertComponentChanged();
		}
	}

	public bool DeserializeData(byte[] data)
	{
		if (data.Length % sizeof(char) != 0)
			return Log.WarningThenReturn($"Data for name must be a multiple of {sizeof(char)}.", false);
		string? deserialized = data.CreateString();
		if (deserialized is null)
			return false;
		SceneIdentity = deserialized;
		AlertComponentChanged();
		return true;
	}

	public byte[] SerializeData() => SceneIdentity?.ToBytes() ?? Array.Empty<byte>();
}
