namespace Civlike.Client.Render.World;

//public sealed class ClusterVisibilityRenderBehaviour : DependentRenderBehaviourBase<WorldClusterArchetype> {
//	public bool IsVisible { get; private set; } = false;

//	public event Action? VisibilityChanged;

//	public override void Update( double time, double deltaTime ) {
//		CheckVisibilityAgainstCameraTranslation( this.RenderEntity.ServiceAccess.CameraProvider.Main.View3.Translation.Normalize<Vector3<float>, float>(), this.RenderEntity.ServiceAccess.CameraProvider.Main.Camera3.Matrix );
//	}

//	public void SetVisibility( bool visible ) {
//		if (this.IsVisible == visible)
//			return;
//		this.IsVisible = visible;
//		VisibilityChanged?.Invoke();
//	}

//	public void CheckVisibilityAgainstCameraTranslation( Vector3<float> normalizedTranslation, Matrix4x4<float> viewProjectionMatrix ) {
//		bool shouldBeVisible = false;

//		AABB<Vector3<float>> bounds = this.Archetype.ClusterComponent.Bounds;
//		Vector3<float> min = bounds.Minima;
//		Vector3<float> max = bounds.Maxima;

//		Span<Vector3<float>> boundsCorners =
//		[
//			(min.X, min.Y, min.Z),
//			(min.X, min.Y, max.Z),
//			(min.X, max.Y, max.Z),
//			(min.X, max.Y, min.Z),
//			(max.X, min.Y, min.Z),
//			(max.X, min.Y, max.Z),
//			(max.X, max.Y, max.Z),
//			(max.X, max.Y, min.Z)
//		];

//		for (int i = 0; i < boundsCorners.Length; i++)
//			if (normalizedTranslation.Dot( boundsCorners[ i ].Normalize<Vector3<float>, float>() ) >= 0 /*0.20629947401*/) {
//				shouldBeVisible = true;
//				break;
//			}

//		if (!shouldBeVisible)
//			SetVisibility( false );

//		shouldBeVisible = false;

//		for (int i = 0; i < boundsCorners.Length; i++) {
//			Vector3<float>? transformedCorner = boundsCorners[ i ].TransformWorld( viewProjectionMatrix );
//			if (!transformedCorner.HasValue) 				continue;
//			Vector3<float> transformed = transformedCorner.Value;
//			//Check if the transformed corner is within the view frustum
//			if (transformed.X <= 1 && transformed.X >= -1 && transformed.Y <= 1 && transformed.Y >= -1) {
//				shouldBeVisible = true;
//				break;
//			}
//		}

//		SetVisibility( shouldBeVisible );
//	}
//	protected override bool InternalDispose() {
//		return true;
//	}
//}