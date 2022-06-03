using System.Numerics;
using Engine.Rendering.Colors;

namespace Engine.Rendering.Lighting;
public abstract class LightBase : DisposableIdentifiable {

	public readonly bool HasShadows;
	public Color16x3 Color { get; private set; }
	public float Intensity { get; private set; }

	public event Action? Changed;

	public LightBase( Color16x3 color, float intensity, bool hasShadows ) {
		if ( intensity <= 0 )
			throw new ArgumentException( $"{nameof( intensity )} must be greater than zero." );
		this.HasShadows = hasShadows;
		this.Color = color;
		this.Intensity = intensity;
	}

	public void SetColor( Color16x3 color ) {
		if ( this.Color == color )
			return;
		this.Color = color;
		TriggerChangedEvent();
	}

	public void SetIntensity( float intensity ) {
		if ( this.Intensity == intensity )
			return;
		if ( intensity <= 0 ) {
			this.LogWarning( $"{nameof( intensity )} must be greater than zero." );
			return;
		}
		this.Intensity = intensity;
		TriggerChangedEvent();
	}

	public abstract ISceneObject CreateRender();

	protected void TriggerChangedEvent() => Changed?.Invoke();

}

public class DirectionalLight : LightBase {

	public Vector3 Direction { get; private set; }
	public Quaternion DirectionQuaternion { get; private set; }

	public DirectionalLight( Vector3 direction, Color16x3 color, float intensity, bool hasShadows ) : base( color, intensity, hasShadows ) {
		if ( direction.X == 0 && direction.Y == 0 && direction.Z == 0 )
			throw new ArgumentException( $"{nameof( direction )} must be a non-zero vector." );
		this.Direction = Vector3.Normalize( direction );
		this.DirectionQuaternion = this.Direction.DirectionVectorToQuaternion();
	}

	public void SetDirection( Vector3 direction ) {
		if ( this.Direction == direction )
			return;
		if ( direction.X == 0 && direction.Y == 0 && direction.Z == 0 ) {
			this.LogWarning( $"{nameof( direction )} must be a non-zero vector." );
			return;
		}
		this.Direction = Vector3.Normalize( direction );
		this.DirectionQuaternion = this.Direction.DirectionVectorToQuaternion();
		TriggerChangedEvent();
	}

	public override ISceneObject CreateRender() {
		if ( this.HasShadows ) 
			return new Directional.DirectionalShadowLightRender( this, 0 );
		return new Directional.DirectionalNoShadowLightRender( this );
	}

	protected override bool OnDispose() => true;

}

public class PointLight : LightBase {

	public Vector3 Translation { get; private set; }
	public float Radius { get; private set; }

	public PointLight( Vector3 translation, float radius, Color16x3 color, float intensity, bool hasShadows ) : base( color, intensity, hasShadows ) {
		if ( radius <= 0 )
			throw new ArgumentException( $"{nameof( radius )} must be greater than zero." );
		this.Translation = translation;
		this.Radius = radius;
	}

	public void SetTranslation( Vector3 translation ) {
		if ( this.Translation == translation )
			return;
		this.Translation = translation;
		TriggerChangedEvent();
	}

	public void SetRadius( float radius ) {
		if ( this.Radius == radius )
			return;
		if (radius <= 0 ) {
			this.LogWarning( $"{nameof( radius )} must be greater than zero." );
			return;
		}
		this.Radius = radius;
		TriggerChangedEvent();
	}

	public override ISceneObject CreateRender() {
		if ( this.HasShadows )
			return null;//new Point.DirectionalShadowLightRender( this, 0 );
		return new Point.PointLightNoShadowRender( this );
	}

	protected override bool OnDispose() => true;
}

public class SpotLight : PointLight {

	public Vector3 Direction { get; private set; }
	public Quaternion DirectionQuaternion { get; private set; }

	public SpotLight( Vector3 direction, Vector3 translation, float radius, Color16x3 color, float intensity, bool hasShadows ) : base( translation, radius, color, intensity, hasShadows ) {
		if ( direction.X == 0 && direction.Y == 0 && direction.Z == 0 )
			throw new ArgumentException( $"{nameof( direction )} must be a non-zero vector." );
		this.Direction = Vector3.Normalize( direction );
		this.DirectionQuaternion = this.Direction.DirectionVectorToQuaternion();
	}

	public void SetDirection( Vector3 direction ) {
		if ( this.Direction == direction )
			return;
		if ( direction.X == 0 && direction.Y == 0 && direction.Z == 0 ) {
			this.LogWarning( $"{nameof( direction )} must be a non-zero vector." );
			return;
		}
		this.Direction = Vector3.Normalize( direction );
		this.DirectionQuaternion = this.Direction.DirectionVectorToQuaternion();
		TriggerChangedEvent();
	}

	public override ISceneObject CreateRender() => null;
}