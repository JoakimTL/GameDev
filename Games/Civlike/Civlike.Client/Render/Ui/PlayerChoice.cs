using Engine;

namespace Civlike.Client.Render.Ui;

public sealed class PlayerChoice {

	public Guid PlayerId { get; }
	public string PlayerName { get; }
	public Vector4<float> Color { get; }
	public PlayerChoice( Guid playerId, string playerName, Vector4<float> color ) {
		PlayerId = playerId;
		PlayerName = playerName;
		this.Color = color;
	}
	public override string ToString() => PlayerName;
}