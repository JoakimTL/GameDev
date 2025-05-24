using Engine;

namespace Civlike.Client.Render.Ui;

public sealed class PlayerChoice( Guid playerId, string playerName, Vector4<float> color ) {

	public Guid PlayerId { get; } = playerId;
	public string PlayerName { get; } = playerName;
	public Vector4<float> Color { get; } = color;

	public override string ToString() => this.PlayerName;
}