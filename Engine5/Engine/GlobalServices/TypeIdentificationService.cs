using System.Reflection;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;

public sealed class TypeIdentificationService : IGlobalService
{

    private readonly Dictionary<string, Type> _typesFromIdentification;

    public TypeIdentificationService()
    {
        _typesFromIdentification = new();

        foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(p => p.GetTypes().Where(q => q.GetCustomAttribute<IdentityAttribute>() is not null)))
        {
            IdentityAttribute identity = type.GetCustomAttribute<IdentityAttribute>()!;
            if (_typesFromIdentification.TryGetValue(identity.Identity, out Type? occupyingType))
                throw new InvalidDataException($"{type.FullName}{Environment.NewLine}Identity \"{identity.Identity}\" already taken by:{Environment.NewLine}{occupyingType.FullName}");
            _typesFromIdentification.Add(identity.Identity, type);
        }
    }

    public Type? GetFromIdentity(IdentityAttribute? identification) => GetFromIdentity(identification?.Identity);
    public Type? GetFromIdentity(string? identification) => identification is not null && _typesFromIdentification.TryGetValue(identification, out Type? type) ? type : null;

}
