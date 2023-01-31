using System.Reflection;
using Engine.Structure.Attributes;
using Engine.Structure.Interfaces;

namespace Engine.GlobalServices;

public sealed class IdentityTypeService : IGlobalService
{

    private readonly Dictionary<string, Type> _typesFromIdentification;

    public IdentityTypeService(TypeService typeService)
    {
        _typesFromIdentification = new();

        foreach (var type in typeService.AllTypes.Where(p => p.GetCustomAttribute<IdentityAttribute>() is not null))
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


public sealed class TypeService : IGlobalService
{

    public readonly IReadOnlyList<Type> AllTypes;
    public readonly IReadOnlyList<Type> AbstractTypes;
    public readonly IReadOnlyList<Type> SealedTypes;
    public readonly IReadOnlyList<Type> DerivedTypes;
    public readonly IReadOnlyList<Type> InterfaceTypes;

    public TypeService()
    {
        List<Type> allTypes = new();
        AllTypes = allTypes;
        List<Type> abstractTypes = new();
        AbstractTypes = abstractTypes;
        List<Type> sealedTypes = new();
        SealedTypes = sealedTypes;
        List<Type> interfaceTypes = new();
        InterfaceTypes = interfaceTypes;
        List<Type> derivedTypes = new();
        DerivedTypes = derivedTypes;

        foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(p => p.GetTypes()))
        {
            allTypes.Add(type);
            if (type.IsAbstract)
                abstractTypes.Add(type);
            if (type.IsInterface)
                interfaceTypes.Add(type);
            if (type.IsSealed)
                sealedTypes.Add(type);
            if (type.BaseType is not null)
                derivedTypes.Add(type);
        }
    }

}
