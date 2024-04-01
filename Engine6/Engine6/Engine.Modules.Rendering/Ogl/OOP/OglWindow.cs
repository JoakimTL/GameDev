using Engine.Math.NewFolder;
using Engine.Modules.Rendering.Ogl.Services;
using Engine.Modules.Rendering.Ogl.Utilities;
using OpenGL;
using System.Numerics;

namespace Engine.Modules.Rendering.Ogl.OOP;

public sealed class OglWindow : DisposableIdentifiable {
    private readonly FramebufferStateService _framebufferState;
    private readonly ViewportStateService _viewport;
    private string _title;

    public event Action<OglWindow>? Resized;

    internal OglWindow( FramebufferStateService framebufferState, ViewportStateService viewport, nint handle ) {
        this._framebufferState = framebufferState;
        this._viewport = viewport;
        this.Handle = handle;
        this._title = AppDomain.CurrentDomain.FriendlyName;
        WindowUtilities.SetTitle( Handle, _title );
    }

    internal nint Handle { get; }
    public Vector2<int> Size { get; private set; }
    public Vector2 AspectRatioVector { get; private set; }
    public float AspectRatio { get; private set; }
    public string Title { get => _title; set => SetTitle( value ); }
    public bool ShouldClose => WindowUtilities.ShouldWindowClose( Handle );

    private void SetTitle( string value ) {
        if (_title == value)
            return;
        _title = value;
        WindowUtilities.SetTitle( Handle, value );
    }

    public void Bind() {
        _framebufferState.UnbindFramebuffer( FramebufferTarget.DrawFramebuffer );
        _viewport.Set( 0, Size );
    }

    internal void CheckResize() {
        WindowUtilities.GetWindowSize( Handle, out int w, out int h );
        if (Size.X != w || Size.Y != h) {
            Size = (w, h);
            UpdateAspectRatio();
            Resized?.Invoke( this );
        }
    }
    private void UpdateAspectRatio() {
        AspectRatio = (float) Size.X / Size.Y;
        float aspectRatioX = Size.X > Size.Y ? (float) Size.X / Size.Y : 1;
        float aspectRatioY = Size.Y > Size.X ? (float) Size.Y / Size.X : 1;
        AspectRatioVector = new Vector2( aspectRatioX, aspectRatioY );
    }

    internal void SwapBuffer() => WindowUtilities.SwapBuffer( Handle );

    protected override bool InternalDispose() {
        WindowUtilities.DestroyWindow( Handle );
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
