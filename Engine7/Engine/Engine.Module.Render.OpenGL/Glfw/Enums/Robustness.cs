namespace Engine.Module.Render.OpenGL.Glfw.Enums;

public enum Robustness {
	None = 0,
	NoResetNotification = 0x00031001,
	/// <summary>The context is lost on reset, use glGetGraphicsResetStatus for more information.</summary>
	LoseContextOnReset = 0x00031002
}
