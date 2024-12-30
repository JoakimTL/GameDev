using Engine.Logging;
using Engine.Module.Entities.Container;
using Engine.Module.Entities.Render;
using Engine.Module.Render.Domain;
using Engine.Module.Render.Entities.Components;
using Engine.Module.Render.Glfw.Enums;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.OOP.DataBlocks;
using Engine.Module.Render.Ogl.Scenes;
using Engine.Module.Render.Ogl.Services;
using Engine.Physics;
using Engine.Shapes;
using Engine.Standard.Entities.Components;
using Engine.Standard.Render;
using Engine.Standard.Render.Meshing;
using Engine.Standard.Render.Text.Fonts;
using Engine.Standard.Render.Text.Services;
using OpenGL;
using System.Numerics;

namespace Sandbox;

public sealed class TestPipeline( WindowService windowService, DataBlockService dataBlockService, SceneService sceneService, OldFontService fontService, UserInputEventService userInputEventService, CameraService cameraService ) : DisposableIdentifiable, IRenderPipeline, IInitializable {
	private readonly WindowService _windowService = windowService;
	private readonly DataBlockService _dataBlockService = dataBlockService;
	private readonly SceneService _sceneService = sceneService;
	private readonly OldFontService _fontService = fontService;
	private readonly CameraService _cameraService = cameraService;
	private UniformBlock _testUniforms = null!;
	private ShaderStorageBlock _testShaderStorage = null!;
	private DataBlockCollection _dataBlocks = null!;
	private Scene _scene = null!;
	private OldFont? _font;

	private bool _panning;
	private Vector2<double> _lastMousePosition;

	public void Initialize() {
		//_font = _fontService.Get( "JetBrainsMono-Bold" );

		//var g = _font[ 'A' ];
		//g.CreateMeshTriangles();

		if (!this._dataBlockService.CreateUniformBlock( nameof( SceneCameraBlock ), 256, [ ShaderType.VertexShader ], out this._testUniforms! ))
			throw new InvalidOperationException( "Couldn't create uniform block." );
		if (!this._dataBlockService.CreateShaderStorageBlock( "testShaderStorageBlock", 4, [ ShaderType.VertexShader ], out this._testShaderStorage! ))
			throw new InvalidOperationException( "Couldn't create shader storage block." );
		this._dataBlocks = new DataBlockCollection( this._testUniforms, this._testShaderStorage );

		Edge2<float> ed = new( (0, 0), (0, 1) );
		var or1 = ed.Orientation( (1, 0) );
		var or2 = ed.Orientation( (-1, 0) );

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
		Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
		Gl.Enable( EnableCap.DepthTest );
		//Gl.Enable( EnableCap.CullFace );
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

public sealed class TestRenderBehaviour : SynchronizedRenderBehaviourBase<RenderArchetype> {

	private SceneInstance<Entity2SceneData>? _sceneInstance;

	private Matrix4x4<float> _preparedTransformMatrix = Matrix4x4<float>.MultiplicativeIdentity;
	private Matrix4x4<float> _transformMatrix = Matrix4x4<float>.MultiplicativeIdentity;

	protected override void OnRenderEntitySet() {
		base.OnRenderEntitySet();
		this._sceneInstance = this.RenderEntity.RequestSceneInstance<SceneInstance<Entity2SceneData>>( "test", 0 );
		this._sceneInstance.SetShaderBundle( this.RenderEntity.ServiceAccess.ShaderBundleProvider.GetShaderBundle<TestShaderBundle>() );
		this._sceneInstance.SetVertexArrayObject( this.RenderEntity.ServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex3, Entity2SceneData>() );
		IcosphereGenerator.GenerateIcosphereVectors<float>( 3, out var vectors, out var indices );
		OcTree<TriangleVertex, float> ocTree = new( 5 );
		foreach (var v in vectors)
			ocTree.Add( new( v ) );
		this.LogLine( $"Vertices: {vectors.Count}" );
		this.LogLine( $"Triangles: {indices.Count / 3}" );
		List<Vertex3> vertices = new List<Vertex3>();
		Vector2<float> polarSpace = (float.Pi, float.Pi / 2);
		List<uint> ind3 = [];
		Random r = new( 42 );
		//for (int i = 0; i < vectors.Count; i++) {
		//	Vertex3 v = new( vectors[ i ].CastSaturating<double, float>(), 0, 0, ((byte) r.Next( 100, 255 ), (byte) r.Next( 100, 255 ), (byte) r.Next( 100, 255 ), 255) );
		//	vertices.Add( v );
		//}
		var sub9Indices = indices[ 2 ];
		for (int i = 0; i < sub9Indices.Count; i += 3) {
			Vector4<byte> color = ((byte) r.Next( 100, 255 ), (byte) r.Next( 100, 255 ), (byte) r.Next( 100, 255 ), 255);
			var v1 = vectors[ (int) sub9Indices[ i ] ];
			Vector2<float> cp1 = v1.ToNormalizedPolar().DivideEntrywise( polarSpace );
			Vector4<byte> c1 = ((byte) (double.Abs( cp1.X ) * 255), (byte) (double.Abs( cp1.Y ) * 255), 0, 255);
			Vertex3 v = new( vectors[ (int) sub9Indices[ i ] ], 0, 0, c1 );
			vertices.Add( v );
			v = new( vectors[ (int) sub9Indices[ i + 1 ] ], 0, 0, c1 );
			vertices.Add( v );
			v = new( vectors[ (int) sub9Indices[ i + 2 ] ], 0, 0, c1 );
			vertices.Add( v );
			ind3.Add( (uint) vertices.Count - 3 );
			ind3.Add( (uint) vertices.Count - 2 );
			ind3.Add( (uint) vertices.Count - 1 );
		}
		//List<uint> ind2 = [];
		//for (int i = 0; i < indices.Count; i += 3) {
		//	ind2.Add( indices[ i ] );
		//	ind2.Add( indices[ i + 1 ] );
		//	ind2.Add( indices[ i + 1 ] );
		//	ind2.Add( indices[ i + 2 ] );
		//	ind2.Add( indices[ i + 2 ] );
		//	ind2.Add( indices[ i ] );
		//}
		this._sceneInstance.SetMesh( this.RenderEntity.ServiceAccess.MeshProvider.CreateMesh( vertices.ToArray(), ind3.ToArray() ) );
		//double minTriangleArea = double.MaxValue;
		//double maxTriangleArea = double.MinValue;
		//double avgTriangleArea = 0;
		//double maxInnerAngle = double.MinValue;
		//double minInnerAngle = double.MaxValue;
		//double avgInnerAngle = 0;
		//for (int i = 0; i < indices.Count; i += 3) {
		//	Vector3<double> a = vectors[ (int) sub9Indices[ i ] ];
		//	Vector3<double> b = vectors[ (int) sub9Indices[ i + 1 ] ];
		//	Vector3<double> c = vectors[ (int) sub9Indices[ i + 2 ] ];
		//	Vector3<double> ab = b - a;
		//	Vector3<double> ac = c - a;
		//	double area = Math.Abs( ab.Cross( ac ).Magnitude<Vector3<double>, double>() * 0.5 );
		//	minTriangleArea = Math.Min( minTriangleArea, area );
		//	maxTriangleArea = Math.Max( maxTriangleArea, area );
		//	avgTriangleArea += area;
		//	double innerAngle = Math.Acos( ab.Dot( ac ) / (ab.Magnitude<Vector3<double>, double>() * ac.Magnitude<Vector3<double>, double>()) );
		//	minInnerAngle = Math.Min( minInnerAngle, innerAngle );
		//	maxInnerAngle = Math.Max( maxInnerAngle, innerAngle );
		//	avgInnerAngle += innerAngle;
		//}
		//avgTriangleArea /= indices.Count;
		//avgTriangleArea *= 3;
		//this.LogLine( $"Min triangle area: {minTriangleArea}, Max triangle area: {maxTriangleArea}, Avg: {avgTriangleArea}" );
		//this.LogLine( $"Diff max/min: {maxTriangleArea / minTriangleArea}, max/avg {maxTriangleArea / avgTriangleArea}, avg/min {avgTriangleArea / minTriangleArea}" );

		//this.LogLine( $"Avg represent of earth area {avgTriangleArea * 510100000}" );
		//this.LogLine( $"Avg represent of earth area {avgTriangleArea * 510100000 * indices.Count / 3}" );

		//avgInnerAngle /= indices.Count;
		//avgInnerAngle *= 3;
		//this.LogLine( $"Min inner angle: {minInnerAngle}, Max inner angle: {maxInnerAngle}, Avg: {avgInnerAngle}" );
		//this.LogLine( $"Diff max-min: {maxInnerAngle - minInnerAngle}, max-avg {maxInnerAngle - avgInnerAngle}, avg-min {avgInnerAngle - minInnerAngle}" );
		//this._sceneInstance.SetMesh( this.RenderEntity.ServiceAccess.MeshProvider.CreateMesh(
		//	[ new Vertex2( (-.5f, -.5f), (255, 255, 0, 255) ),
		//	new Vertex2( (-.5f, .5f), (255, 0, 255, 255) ),
		//	new Vertex2( (.5f, .5f), (0, 255, 255, 255) ),
		//	new Vertex2( (.5f, -.5f), (255, 0, 0, 255) ) ],
		//	[ 2, 1, 0, 0, 3, 2 ] ) );
	}

	protected override void OnUpdate( double time, double deltaTime ) {
	}

	public class TriangleVertex( Vector3<float> vector ) : IOctreeLeaf<float> {
		public Vector3<float> Vector { get; } = vector;
		public uint Level { get; } = 0;
	}

	protected override bool PrepareSynchronization( ComponentBase component ) {
		if (component is Transform3Component t3c) {
			this._preparedTransformMatrix = t3c.Transform.Matrix.CastSaturating<double, float>();
			return true;
		}
		return false;
	}

	protected override void Synchronize() {
		this._transformMatrix = this._preparedTransformMatrix;
		this._sceneInstance?.Write( new Entity2SceneData( this._transformMatrix ) );
	}

	protected override bool InternalDispose() {
		return true;
	}
}

public interface IOctreeLeaf<TScalar> where TScalar : unmanaged, INumber<TScalar> {
	Vector3<TScalar> Vector { get; }
}

public sealed class OcTree<T, TScalar>
	where T : IOctreeLeaf<TScalar>
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	private readonly OcTreeBranch<T, TScalar> _root;

	public int Count => _root.Count;

	public OcTree( uint depth ) {
		_root = new( AABB.Create<Vector3<TScalar>>( [ TScalar.NegativeOne, TScalar.One ] ), depth );
	}

	/// <param name="level">Deepest level is 0, and goes higher from there.</param>
	public void GetBoundsAtLevel( out List<AABB<Vector3<TScalar>>> bounds, uint level = 0 ) {
		bounds = [];
		_root.GetBoundsAtLevel( level, bounds );
	}

	public void GetAll( AABB<Vector3<TScalar>> area, List<T> output ) => _root.GetAll( area, output );

	public void Add( T item ) => _root.Add( item );

	public void Remove( T item ) => _root.Remove( item );

}

public sealed class OcTreeBranch<T, TScalar>
	where T : IOctreeLeaf<TScalar>
	where TScalar : unmanaged, IFloatingPointIeee754<TScalar> {

	public AABB<Vector3<TScalar>> BranchDomain { get; }
	public uint Level { get; }

	private readonly OcTreeBranch<T, TScalar>[]? _subBranches;
	private readonly HashSet<T> _contents;

	public int Count => _contents?.Count ?? this._subBranches?.Sum( p => p.Count ) ?? 0;

	public OcTreeBranch( AABB<Vector3<TScalar>> branchDomain, uint level ) {
		this.BranchDomain = branchDomain;
		this.Level = level;
		this._contents = [];
		if (level == 0) {
			return;
		}
		this._subBranches = new OcTreeBranch<T, TScalar>[ 8 ];
		int index = 0;
		Vector3<TScalar> halfSpan = branchDomain.GetCenter() - branchDomain.Minima;
		for (int x = 0; x <= 1; x++) {
			for (int y = 0; y <= 1; y++) {
				for (int z = 0; z <= 1; z++) {
					Vector3<TScalar> walk = new Vector3<int>( x, y, z ).CastSaturating<int, TScalar>();
					AABB<Vector3<TScalar>> childDomain = new( branchDomain.Minima + halfSpan.MultiplyEntrywise( walk ), branchDomain.GetCenter() + halfSpan.MultiplyEntrywise( walk ) );
					this._subBranches[ index++ ] = new( childDomain, level - 1 );
				}
			}
		}
	}

	public void GetBoundsAtLevel( uint level, List<AABB<Vector3<TScalar>>> bounds ) {
		if (Level == level) {
			bounds.Add( BranchDomain );
			return;
		}
		if (Level == 0) {
			this.LogLine( $"Concluding search at level 0, when intended search level was {level}" );
			return;
		}
		if (_subBranches is null)
			throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
		for (int i = 0; i < 8; i++)
			_subBranches[ i ].GetBoundsAtLevel( level, bounds );
	}

	public void GetAll( AABB<Vector3<TScalar>> volume, List<T> output ) {
		if (Level == 0) {
			output.AddRange( _contents );
			return;
		}
		if (_subBranches is null)
			throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
		for (int i = 0; i < 8; i++)
			if (_subBranches[ i ].BranchDomain.Intersects( volume ))
				_subBranches[ i ].GetAll( volume, output );
	}

	public bool Add( T item ) {
		if (Level == 0) {
			_contents.Add( item );
			return true;
		}
		if (_subBranches is null)
			throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
		for (int i = 0; i < 8; i++)
			if (_subBranches[ i ].BranchDomain.Contains( item.Vector ))
				if (_subBranches[ i ].Add( item ))
					return true;
		return false;
	}

	public void Remove( T item ) {
		if (Level == 0) {
			_contents.Remove( item );
			return;
		}
		if (_subBranches is null)
			throw new InvalidOperationException( "Subbranches are null, but level is not 0." );
		for (int i = 0; i < 8; i++)
			if (_subBranches[ i ].BranchDomain.Contains( item.Vector ))
				_subBranches[ i ].Remove( item );
	}

	public override string ToString() => $"Level {Level}, {BranchDomain}, {Count} items";

}