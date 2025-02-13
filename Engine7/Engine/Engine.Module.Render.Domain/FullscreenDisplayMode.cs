﻿namespace Engine.Module.Render.Domain;

public sealed class FullscreenDisplayMode() : IWindowDisplayMode {
	public Vector2<uint> Size { get; } = 0;
	public WindowDisplayMode Mode { get; } = WindowDisplayMode.Fullscreen;
}
