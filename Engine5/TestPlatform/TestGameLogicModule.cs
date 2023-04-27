using Engine.GameLogic;
using Engine.GameLogic.ECPS;
using Engine.GameLogic.ECPS.Components;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;
using StandardPackage.ECPS.Components;
using StandardPackage.ECPS.Systems;
using StandardPackage.Rendering.Scenes;
using StandardPackage.Rendering.VertexArrayLayouts;
using System.Numerics;

namespace TestPlatform;

internal class TestGameLogicModule : GameLogicModuleBase, ITimedSystem
{

    private Entity? _e;
    private float lastTime;

    public int SystemTickInterval => 100;

    protected override void OnInitialize()
    {
        _e = Get<EntityContainerService>()._container.Create();
        _e.AddOrGet<LinearMovement3Component>().Impulse(new(0, 0, 0));
        _e.AddOrGet<Mass3Component>();
        _e.AddOrGet<RotationalMovement3Component>().Twirl(new(0, 0, 5));
        _e.AddOrGet<Transform3Component>();
        _e.AddOrGet<RenderInstanceDataComponent>();
        _e.AddOrGet<RenderMaterialAssetComponent>().SetMaterial("testMaterial");
        _e.AddOrGet<RenderMeshAssetComponent>().SetMesh("box");
        _e.AddOrGet<RenderSceneComponent>().SetScene<Default3Scene>();
    }

    //TODO: Find out why the fuck this module sometimes starts and other times just don't
    //TODO: interpolation between these ticks.

    protected override void OnUpdate(float time, float deltaTime)
    {
        if (_e is not null)
        {
            var transform = _e.Get<Transform3Component>();
            if (transform is not null)
                _e?.Get<RenderInstanceDataComponent>()?.SetData(stackalloc Entity3SceneData[] { new() { ModelMatrix = transform.Transform.Matrix, Color = Vector4.One } });
        }
        if (time - lastTime > 1)
        {
            Console.WriteLine(Get<EntityContainerService>());
            Console.WriteLine(Get<EntitySystemContainerService>());
            Console.WriteLine(_e);
            lastTime = time;
        }
    }
}


[ProcessBefore<Gravity3System, SystemBase>]
public class Gravity3ValueProvider : SystemBase, IGravity3ValueProvider
{
    public bool IsAcceleration => true;

    private Vector3 _centerOfUniverse;

    public Vector3 GetGravity(Vector3 globalTranslation)
    {
        var v = globalTranslation - _centerOfUniverse;
        var l = MathF.Max(v.LengthSquared(), 1);
        return v / l;
    }

    public override void Update(IEnumerable<Entity> entities, float time, float deltaTime)
    {
        _centerOfUniverse = new Vector3(MathF.Cos(time) * 100, 0, MathF.Sin(time) * 100);
    }
}
