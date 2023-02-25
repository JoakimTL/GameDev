using Engine.Rendering.Contexts.Objects.VAOs;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rendering.Contexts.Objects.Scenes;
public class Scene : ISceneRender
{

    //ISceneObject no longer has "HasTransparency", cause this should be handled on a per shaderpipeline basis rather than shaderbundle level. This adds some complexity to the "sorting" of the sceneobjects. 
    public void Render(IDataBlockCollection? dataBlocks, Action<bool>? blendActivationFunction, uint shaderUse = 0, PrimitiveType prim = PrimitiveType.Triangles)
    {
        throw new NotImplementedException();
    }
}
