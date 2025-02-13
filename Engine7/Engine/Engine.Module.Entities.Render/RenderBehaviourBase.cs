﻿namespace Engine.Module.Render.Entities;

public abstract class RenderBehaviourBase : DisposableIdentifiable, IUpdateable {
	private RenderEntity? _renderEntity;
	protected RenderEntity RenderEntity => this._renderEntity ?? throw new InvalidOperationException( "RenderEntity is not set." );

	internal void SetRenderEntity( RenderEntity renderEntity ) {
		if (this._renderEntity is not null)
			throw new InvalidOperationException( "Behaviour owner can't be changed." );
		this._renderEntity = renderEntity;
		OnRenderEntitySet();
	}

	protected virtual void OnRenderEntitySet() { }

	public abstract void Update( double time, double deltaTime );
}
