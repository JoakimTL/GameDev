using Engine.Logging;
using Engine.Module.Render.Input;
using Engine.Module.Render.Ogl.Providers;
using Engine.Physics;
using Engine.Standard.Render.Entities.Behaviours.Shaders;
using Engine.Standard.Render.Input.Services;
using Engine.Standard.Render.Meshing.Services;
using Engine.Standard.Render.Text.Services;
using Engine.Standard.Render.Text.Typesetting;
using Engine.Transforms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Standard.Render.UserInterface.Standard;

public sealed class Button : UserInterfaceComponentBase {

	public event Action? ButtonClicked;
	private bool _hovering;
	private bool _hoveringAtPress;

	private readonly Collider2Shape _collider;
	private readonly Collision2Calculation<double> _collision;
	private readonly SceneInstance<Entity2SceneData> _sceneInstance;
	private readonly TextLayout _textLayout;

	public Vector4<double> DefaultColor { get; set; }
	public Vector4<double> HoverColor { get; set; }
	public Vector4<double> PressedColor { get; set; }

	public Button( UserInterfaceElementBase element, string text, string fontName, TransformData<Vector2<double>, double, Vector2<double>> transform, Vector4<double> defaultColor, Vector4<double> hoverColor, Vector4<double> pressedColor ) : base( element ) {
		_userInterfaceComponentPlacement.SetTransform( transform );
		_collider = new Collider2Shape();
		_collider.SetBaseVertices( [ (-1, -1), (1, -1), (1, 1), (-1, 1) ] );
		_collider.SetTransform( TransformInterface );
		_collision = new Collision2Calculation<double>( _collider, element.UserInterfaceServiceAccess.Get<MouseColliderProvider>().ColliderNDCA );
		_sceneInstance = element.UserInterfaceServiceAccess.RequestSceneInstance<SceneInstance<Entity2SceneData>>( 0 );
		_sceneInstance.SetVertexArrayObject( element.UserInterfaceServiceAccess.CompositeVertexArrayProvider.GetVertexArray<Vertex2, Entity2SceneData>() );
		_sceneInstance.SetShaderBundle( element.UserInterfaceServiceAccess.ShaderBundleProvider.GetShaderBundle<Primitive2ShaderBundle>() );
		_sceneInstance.SetMesh( element.UserInterfaceServiceAccess.Get<PrimitiveMesh2Provider>().Get( Meshing.Primitive2.Rectangle ) );
		_textLayout = element.UserInterfaceServiceAccess.RequestTextLayout( 1 );
		_textLayout.FontName = fontName;
		_textLayout.Text = text;
		_textLayout.TextScale = 0.5f;
		_textLayout.VerticalAlignment = Alignment.Centered;
		_textLayout.HorizontalAlignment = Alignment.Centered;
		this.DefaultColor = defaultColor;
		this.HoverColor = hoverColor;
		this.PressedColor = pressedColor;
	}

	protected override void OnUpdate( double time, double deltaTime ) {
		if (!_collision.Evaluate())
			this.LogWarning( "Collision calculation failed." );
		_hovering = _collision.CollisionResult.IsColliding;
		var color = _hoveringAtPress
			? PressedColor
			: _hovering
				? HoverColor
				: DefaultColor;

		var colorUshort = (color * ushort.MaxValue).Clamp<Vector4<double>, double>( 0, ushort.MaxValue ).CastSaturating<double, ushort>();

		_sceneInstance.Write( new Entity2SceneData( TransformInterface.Matrix.CastSaturating<double, float>(), colorUshort ) );

		_textLayout.Update( time, deltaTime );
	}

	protected override void OnPlacementChanged() {
		_textLayout.TextRotation = (float) TransformInterface.GlobalRotation;
		_textLayout.TextArea = AABB.Create(
			[(TransformInterface.GlobalTranslation - TransformInterface.GlobalScale).CastSaturating<double, float>(),
			(TransformInterface.GlobalTranslation + TransformInterface.GlobalScale).CastSaturating<double, float>()] );
	}

	protected internal override bool OnMouseButton( MouseButtonEvent @event ) {
		if (@event.InputType == TactileInputType.Press && _hovering) {
			_hoveringAtPress = _hovering;
			return true;
		}
		if (@event.InputType == TactileInputType.Release && _hoveringAtPress) {
			if (_hovering)
				ButtonClicked?.Invoke();
			_hoveringAtPress = false;
			return true;
		}
		return false;
	}

	protected override bool InternalDispose() => true;
}
