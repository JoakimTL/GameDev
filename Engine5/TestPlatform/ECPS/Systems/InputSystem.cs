using Engine.GameLogic.ECPS;
using Engine.GlobalServices;
using Engine.Structure.Attributes;
using StandardPackage.ECPS.Components;
using StandardPackage.ECPS.Systems;
using TestPlatformClient.ECPS.Components;

namespace TestPlatformClient.ECPS.Systems;

[Require<UnmappedInputStateComponent>]
[Require<InputComponent>]
[ProcessAfter<UnmappedInputSystem, SystemBase>]
public class InputSystem : SystemBase {
    public InputSystem(SettingsService settingsService) {
        settingsService.Get<InputSettings>();
    }

    public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
        foreach ( Entity entity in entities ) {
            var unmapped = entity.GetOrThrow<UnmappedInputStateComponent>();
            var input = entity.GetOrThrow<InputComponent>();

        }
    }
}