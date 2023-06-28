using Engine.GameLogic.ECPS;
using Engine.Structure.Attributes;
using StandardPackage.ECPS.Components;
using StandardPackage.ECPS.Systems;
using TestPlatformClient.ECPS.Components;

namespace TestPlatformClient.ECPS.Systems;

[Require<UnmappedInputStateComponent>]
[Require<InputComponent>]
[ProcessAfter<UnmappedInputSystem, SystemBase>]
public class InputSystem : SystemBase {
    public InputSystem() { }

    public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
        throw new NotImplementedException();
    }
}