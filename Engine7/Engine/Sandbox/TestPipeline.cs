using Engine.Module.Entities.Container;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Entities.Components;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render;
using Engine.Standard.Render.Text.Services;
using OpenGL;

namespace Sandbox;

public sealed class TestPipeline( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, OldFontService fontService, UserInputEventService userInputEventService, CameraService cameraService, Temporary_TextureAssetService textureAssetService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly WindowService _windowService = windowService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;
	private readonly OldFontService _fontService = fontService;
	private readonly CameraService _cameraService = cameraService;
	private readonly Temporary_TextureAssetService _textureAssetService = textureAssetService;
	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;
	private Scene _scene = null!;

	public void Initialize() {
		//_font = _fontService.Get( "JetBrainsMono-Bold" );

		//var g = _font[ 'A' ];
		//g.CreateMeshTriangles();

		if (!this._dataBlockService.TryCreateUniformBlock( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ], out this._testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!this._dataBlockService.TryCreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out this._testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		this._dataBlocks = new DataBlockCollection( this._testUniforms, this._testShaderStorage );

		_textureAssetService.Get( "assets\\textures\\sampleTexture.png" );

		//Edge2<float> ed = new( (0, 0), (0, 1) );
		//int or1 = ed.Orientation( (1, 0) );
		//int or2 = ed.Orientation( (-1, 0) );

		//Collision2Calculation<float> collision2Calculation = new(
		//	new GJKConvexShape<Vector2<float>, float>( [ (0.1F, .5F) ] ),
		//	new GJKConvexShape<Vector2<float>, float>( [ 0, (0, 1), ( 1, 0) ] ) );

		//GJK.Intersects( collision2Calculation );

		this._scene = this._sceneService.GetScene( "test" );
		//userInputEventService.OnKey += OnKey;
		//userInputEventService.OnMouseButton += OnMouseButton;
		//userInputEventService.OnMouseMoved += OnMouseMoved;
	}

	//private void OnMouseMoved( MouseMoveEvent @event ) {
	//	if (this._panning) {
	//		var delta = @event.Movement - _lastMousePosition;
	//		_cameraService.Main.View3.Rotation = (Rotor3.FromAxisAngle( Vector3<float>.UnitY, (float) -delta.X * float.Pi * 0.001f ) * _cameraService.Main.View3.Rotation).Normalize<Rotor3<float>, float>();
	//		_cameraService.Main.View3.Rotation = (Rotor3.FromAxisAngle( _cameraService.Main.View3.Rotation.Left, (float) delta.Y * float.Pi * 0.001f ) * _cameraService.Main.View3.Rotation).Normalize<Rotor3<float>, float>();
	//	}
	//	_lastMousePosition = @event.Movement;
	//}

	//private void OnMouseButton( MouseButtonEvent @event ) {
	//	if (@event.Button == MouseButton.Right) {
	//		if (@event.InputType == TactileInputType.Press)
	//			this._panning = true;
	//		if (@event.InputType == TactileInputType.Release)
	//			this._panning = false;
	//	}
	//}

	//private void OnKey( KeyboardEvent @event ) {
	//	if (@event.InputType != TactileInputType.Press)
	//		return;
	//	if (@event.Key == Keys.W)
	//		_cameraService.Main.View3.Translation += _cameraService.Main.View3.Rotation.Forward * 0.05f;
	//	if (@event.Key == Keys.S)
	//		_cameraService.Main.View3.Translation -= _cameraService.Main.View3.Rotation.Forward * 0.05f;
	//	if (@event.Key == Keys.A)
	//		_cameraService.Main.View3.Translation += _cameraService.Main.View3.Rotation.Left * 0.1f;
	//	if (@event.Key == Keys.D)
	//		_cameraService.Main.View3.Translation -= _cameraService.Main.View3.Rotation.Left * 0.1f;
	//	if (@event.Key == Keys.Space)
	//		_cameraService.Main.View3.Translation += _cameraService.Main.View3.Rotation.Up * 0.1f;
	//	if (@event.Key == Keys.LeftShift)
	//		_cameraService.Main.View3.Translation -= _cameraService.Main.View3.Rotation.Up * 0.1f;
	//}

	public void PrepareRendering( double time, double deltaTime ) {
		this._testUniforms.Buffer.Write<uint, SceneCameraBlock>( 0, new SceneCameraBlock( _cameraService.Main.Camera3.Matrix, _cameraService.Main.View3.Rotation.Up, -_cameraService.Main.View3.Rotation.Left ) );
		_windowService.Window.Title = $"Camera: {_cameraService.Main.View3.Translation}";
	}

	public void DrawToScreen() {
		Gl.Enable( EnableCap.DepthTest );
		Gl.Enable( EnableCap.CullFace );
		Gl.DepthMask( true );
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		this._scene.Render( "default", this._dataBlocks, _ => { }, PrimitiveType.Triangles );
	}

	protected override bool InternalDispose() {
		return true;
	}
}

public sealed class RenderArchetype : ArchetypeBase {
	public RenderComponent RenderComponent { get; set; } = null!;
	public Transform3Component Transform3Component { get; set; } = null!;
	public TestRenderComponent TestRenderComponent { get; set; } = null!;
}

public sealed class TestRenderComponent : ComponentBase;
