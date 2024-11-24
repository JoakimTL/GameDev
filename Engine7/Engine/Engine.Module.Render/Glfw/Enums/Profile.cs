namespace Engine.Module.Render.Glfw.Enums;

public enum Profile {
	/// <summary><para>If requesting an OpenGL version below 3.2, this profile must be used.</para></summary>
	Any = 0x00000000,
	/// <summary><para>Only if requested OpenGL is greater than 3.2.</para></summary>
	Core = 0x00032001,
	/// <summary><para>Only if requested OpenGL is greater than 3.2.</para></summary>
	Compatibility = 0x00032002
}
