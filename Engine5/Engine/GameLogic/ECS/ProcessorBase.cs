using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GameLogic.ECS;
/// <summary>
/// Takes entity events and removes/adds components to the entity.
/// Example could be: MeshAssetComponent + ShaderAssetCompoent + MaterialAssetComponent -> Render3Component
/// This to sometimes "certify" component combinations, as well as creating composite components that are not serializable.
/// </summary>
public class ProcessorBase {
}
