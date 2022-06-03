using System.Numerics;
using Engine.Data.Datatypes;
using Engine.Rendering.Standard.Scenes;

namespace Engine.Rendering.Lighting;

internal interface IShadowBufferedLight : IDisposable {
	void UpdateShadowBuffers( IMatrixProvider camera, MultiSceneRenderer scenes, Vector3 cameraTranslation );
}