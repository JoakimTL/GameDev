using Engine.Structure.Interfaces;

namespace Engine.Rendering;

public interface IRenderPipeline : IUpdateable
{
    void DrawToScreen();
}
