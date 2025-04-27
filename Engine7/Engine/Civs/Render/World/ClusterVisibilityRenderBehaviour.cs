using Civs.Logic.World;
using Engine;
using Engine.Module.Render.Entities;

namespace Civs.Render.World;

public sealed class ClusterVisibilityRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype> {
	public bool IsVisible { get; private set; } = false;

	public event Action? VisibilityChanged;

	public override void Update( double time, double deltaTime ) {
		CheckVisibilityAgainstCameraTranslation( RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation.Normalize<Vector3<float>, float>() );
	}

	public void SetVisibility( bool visible ) {
		if (IsVisible == visible)
			return;
		IsVisible = visible;
		VisibilityChanged?.Invoke();
	}

	public void CheckVisibilityAgainstCameraTranslation( Vector3<float> normalizedTranslation ) {
		bool shouldBeVisible = false;

		var bounds = Archetype.ClusterComponent.Bounds;
		Vector3<float> min = bounds.Minima;
		Vector3<float> max = bounds.Maxima;

		Span<Vector3<float>> boundsCorners =
		[
			(min.X, min.Y, min.Z),
			(min.X, min.Y, max.Z),
			(min.X, max.Y, max.Z),
			(min.X, max.Y, min.Z),
			(max.X, min.Y, min.Z),
			(max.X, min.Y, max.Z),
			(max.X, max.Y, max.Z),
			(max.X, max.Y, min.Z)
		];

		for (int i = 0; i < boundsCorners.Length; i++)
			if (normalizedTranslation.Dot( boundsCorners[ i ].Normalize<Vector3<float>, float>() ) >= 0 /*0.20629947401*/) {
				shouldBeVisible = true;
				break;
			}

		SetVisibility( shouldBeVisible );
	}
	protected override bool InternalDispose() {
		return true;
	}
}