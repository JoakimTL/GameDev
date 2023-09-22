using Engine.Modules.Communication;

namespace Engine.Modules;
internal class Class1 {
}

public sealed class Module {

	public delegate void NewEmitterHandler( in IMessageEmitter newEmitter );

	private readonly List<IMessageEmitter> _emitters;

	public IReadOnlyList<IMessageEmitter> Emitters => this._emitters;

	public Module() {
		this._emitters = new();
	}

}