using Sandbox.Logic.Setup.Resources;
using System.Runtime.InteropServices;

namespace Sandbox.Logic.Setup.Technologies;
internal class Class1 {
}

[Guid( "FE48D989-2351-41FE-B508-D96DB93B6F15" )]
public sealed class Toolmaking() : TechnologyTypeBase( "Toolmaking", 0 );

[Guid( "2025E319-6361-436E-9D3B-4B025A3388C0" )]
[Requires<FireMastery>]
[Requires<AnimalProducts>]
public sealed class FireMastery() : TechnologyTypeBase( "Fire Mastery", 0 );

[Guid( "FE570957-310F-4A6F-B8A2-B9FFE8A1AA36" )]
public sealed class HuntingAndTracking() : TechnologyTypeBase( "Hunting and Tracking", 0 );

[Guid( "0851D620-C058-46D6-A20A-BE4D4EB7504D" )]
[Requires<Clay>]
public sealed class ClayWorking() : TechnologyTypeBase( "Clay Working", 0 );

[Guid( "87DD8E6E-8F64-4811-9793-D51B0161DB3D" )]
public sealed class CampfireCooking() : TechnologyTypeBase( "Campfire Cooking", 0 );

[Guid( "7FE439D1-D6AC-435C-8187-FF9724A9EFBF" )]
[Requires<ClayWorking>]
[Requires<FireMastery>]
public sealed class EarthenwarePottery() : TechnologyTypeBase( "Earthenware Pottery", 0 );

[Guid( "DCD6BDC3-3F38-4096-B66E-D9D95A749718" )]
[Requires<HuntingAndTracking>]
[Requires<AnimalProducts>]
public sealed class HideProcessing() : TechnologyTypeBase( "Hide Processing", 0 );

[Guid( "6B2E8402-EB4A-42F4-B83E-A59C3D602465" )]
[Requires<Foraging>]
public sealed class PlantCultivation() : TechnologyTypeBase( "Plant Cultivation", 0 );

[Guid( "68B64C8B-C793-49A8-8EFF-4B6295150ABB" )]
[Requires<PlantCultivation>]
public sealed class Threshing() : TechnologyTypeBase( "Threshing", 0 );

[Guid( "2640004A-42F2-449A-810E-3AD7E86840E2" )]
[Requires<Fibers>]
public sealed class Weaving() : TechnologyTypeBase( "Weaving", 0 );

[Guid( "5C0B2821-D490-4067-BED2-43B7A24045EB" )]
[Requires<PlantCultivation>]
[Requires<HuntingAndTracking>]
public sealed class Domestication() : TechnologyTypeBase( "Domestication", 0 );

[Guid( "0E64C42D-AEAD-4406-8E1B-3786EE1F0C15" )]
public sealed class Foraging() : TechnologyTypeBase( "Foraging", 0 );

[Guid("671BDBF5-69C4-45B0-8A04-1E5B5FB93A0B")]
public sealed class CelestialTracking() : TechnologyTypeBase( "Celestial Tracking", 0 );

[Guid("B6EB3D4E-52DB-49F3-8CE8-363494748073")]
public sealed class Counting() : TechnologyTypeBase( "Counting", 0 );

[Guid("E3DA2FA4-4EA8-4A99-9BB0-B8D403D54376")]
public sealed class Bartering() : TechnologyTypeBase( "Bartering", 0 );

[Guid( "10A9A9DE-07EC-432A-B289-F2CE1943C9B1" )]
[Requires<Toolmaking>]
[Requires<Stone>]
public sealed class PolishedStone() : TechnologyTypeBase( "Polished Stone", 40_000 );