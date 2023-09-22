namespace Engine.Modules.Communication;

public interface IMessageEmitter {
	public delegate void MessageHandler( in object message );
	event MessageHandler MessageEmitted;
}

public interface IMessageEmitter<T> : IMessageEmitter {
	public new delegate void MessageHandler( in T message );
	new event MessageHandler MessageEmitted;
}
