using Engine.Module.Render.Ogl.Services;
using Engine.Module.Render.Ogl.Utilities;
using OpenGL;

namespace Engine.Module.Render.Ogl.OOP;

public sealed class OglWindow : DisposableIdentifiable, IResizableAspectRatioSurface<int, float> {
	private readonly FramebufferStateService _framebufferState;
	private readonly ViewportStateService _viewport;
	private string _title;

	public event Action<IResizableSurface<int>>? OnResized;

	internal OglWindow( FramebufferStateService framebufferState, ViewportStateService viewport, nint handle ) {
		this._framebufferState = framebufferState;
		this._viewport = viewport;
		this.Handle = handle;
		this._title = AppDomain.CurrentDomain.FriendlyName;
		WindowUtilities.SetTitle( this.Handle, this._title );
	}

	internal nint Handle { get; }
	public Vector2<int> Size { get; private set; }
	public Vector2<float> AspectRatioVector { get; private set; }
	public float AspectRatio { get; private set; }
	public string Title { get => this._title; set => SetTitle( value ); }
	public bool ShouldClose => WindowUtilities.ShouldWindowClose( this.Handle );

	private void SetTitle( string value ) {
		if (this._title == value)
			return;
		this._title = value;
		WindowUtilities.SetTitle( this.Handle, value );
	}

	public void Bind() {
		this._framebufferState.UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
		this._viewport.Set( 0, this.Size );
	}

	internal void CheckResize() {
		WindowUtilities.GetWindowSize( this.Handle, out int w, out int h );
		if (this.Size.X != w || this.Size.Y != h) {
			this.Size = (w, h);
			UpdateAspectRatio();
			OnResized?.Invoke( this );
		}
	}
	private void UpdateAspectRatio() {
		this.AspectRatio = (float) this.Size.X / this.Size.Y;
		float aspectRatioX = this.Size.X > this.Size.Y ? (float) this.Size.X / this.Size.Y : 1;
		float aspectRatioY = this.Size.Y > this.Size.X ? (float) this.Size.Y / this.Size.X : 1;
		this.AspectRatioVector = new( aspectRatioX, aspectRatioY );
	}

	internal void SwapBuffer() => WindowUtilities.SwapBuffer( this.Handle );

	protected override bool InternalDispose() {
		WindowUtilities.DestroyWindow( this.Handle );
		return true;
	}
}

public sealed class OglSynchronization {


	public OglSynchronization() {
		//      Gl.FenceSync(SyncCondition.SyncGpuCommandsComplete, 0);
		//Gl.GetSync(0, SyncParameterName.SyncStatus, );
		//Gl.MapBuffer
	}
}
