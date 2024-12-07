using Engine.Module.Entities.Container;
using Engine.Transforms;
using Engine.Transforms.Camera;

namespace Engine.Module.Entities.Render;

/// <summary>
/// If an entity has this component any rendering system will attempt to render it if the current scene matches the <see cref="SceneName"/>.
/// </summary>
public sealed class RenderComponent : ComponentBase {

	public string SceneName { get; private set; }
	public RenderRoleBase? RenderRole { get; private set; }

	public RenderComponent() {
		this.SceneName = "Default";
		this.RenderRole = null;
	}

	public void SetSceneName( string sceneName ) {
		this.SceneName = sceneName;
		InvokeComponentChanged();
	}

	public void SetRenderRole( RenderRoleBase? renderRole ) {
		this.RenderRole = renderRole;
		InvokeComponentChanged();
	}
}

public sealed class CameraComponent : ComponentBase {

	public float Fov { get; private set; }
	public float Far { get; private set; }
	public float Near { get; private set; }

	public CameraComponent() {
		this.Fov = 90;
		this.Far = Perspective.DEFAULT_FAR;
		this.Near = Perspective.DEFAULT_NEAR;
	}

	public void SetFov( float fov ) {
		if (fov == this.Fov)
			return;
		if (fov <= 0 || fov >= 180)
			throw new ArgumentOutOfRangeException( "FOV must be greater than 0 and less than 180!" );
		this.Fov = fov;
		InvokeComponentChanged();
	}

	public void SetFar( float far ) {
		if (far == this.Far)
			return;
		if (far <= this.Near)
			throw new ArgumentOutOfRangeException( "Far cannot be less than or equal to Near!" );
		this.Far = far;
		InvokeComponentChanged();
	}

	public void SetNear( float near ) {
		if (near == this.Near)
			return;
		if (near >= this.Far)
			throw new ArgumentOutOfRangeException( "Near cannot be greater than or equal to Far!" );
		this.Near = near;
		InvokeComponentChanged();
	}

	public void Set( float fov, float near, float far ) {
		if (fov == this.Fov && near == this.Near && far == this.Far)
			return;
		if (fov <= 0 || fov >= 180)
			throw new ArgumentOutOfRangeException( "FOV must be greater than 0 and less than 180!" );
		if (near >= far)
			throw new ArgumentOutOfRangeException( "Near cannot be greater than or equal to Far!" );
		this.Fov = fov;
		this.Near = near;
		this.Far = far;
		InvokeComponentChanged();
	}
}

public abstract class RenderRoleBase {
	public abstract void Apply( RenderEntity renderEntity );
}
