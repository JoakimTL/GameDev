﻿using Engine.Modularity;

namespace Engine.Standard.Render.UserInterface;

public abstract class UserInterfaceElementWithMessageNodeBase : UserInterfaceElementBase {

	protected readonly MessageBusNode MessageBusNode;

	protected UserInterfaceElementWithMessageNodeBase( string address ) {
		this.MessageBusNode = MessageBus.CreateNode( address );
		this.MessageBusNode.OnMessageProcessed += OnMessageReceived;
	}

	protected abstract void OnMessageReceived( Message message );

	protected void Publish( object content, string? address, bool log )
		=> this.MessageBusNode.Publish( content, address, log );

	protected override void OnUpdate( double time, double deltaTime )
		=> this.MessageBusNode.ProcessQueue();
}