using System.Runtime.InteropServices;

namespace Civs.Logic.Setup.Resources;

[Guid( "1B7CA83B-9B82-4EA6-9C76-3B649E46E60A" )]
public sealed class LogsResource() : ResourceTypeBase( "Logs", [ "Wood" ] );

[Guid("97743373-CBF6-4A35-885F-D49B72D41BCF")]
public sealed class MalachiteResource() : ResourceTypeBase( "Malachite", [ "Copper Ore", "Ore" ] );

[Guid("F3D1C02E-17A6-46D0-836A-373BB942D325")]
public sealed class AzuriteResource() : ResourceTypeBase( "Azurite", [ "Copper Ore", "Ore" ] );

[Guid("340F4514-8910-471F-8F6A-9070F88D657A")]
public sealed class ChalcopyriteResource() : ResourceTypeBase( "Chalcopyrite", [ "Copper Ore", "Ore" ] );

[Guid("0DF392C0-CD90-4E44-BDB1-1623756882C3")]
public sealed class BorniteResource() : ResourceTypeBase( "Bornite", [ "Copper Ore", "Ore" ] );

[Guid("354B0B3B-4E9D-4E3F-B342-1A21C54649EE")]
public sealed class CopperConcentrateResource() : ResourceTypeBase( "Copper Concentrate", [ "Copper Concentrate", "Metal Concentrate" ] );

[Guid("592F49A5-6E38-4B53-909F-3C835B7D5563")]
public sealed class CopperResource() : ResourceTypeBase( "Copper", [ "Copper", "Metal" ] );