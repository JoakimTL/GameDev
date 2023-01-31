using Engine.Structure.Attributes;
using System.Reflection;

namespace Engine.GameLogic.ECS;

public sealed class ComponentTypeCollectionService : Identifiable, IGameLogicService, IDisposable
{

    private readonly HashSet<ComponentTypeCollection> _componentTypeCollections;
    private readonly Dictionary<Type, ComponentTypeCollection> _requiredComponentTypes;
    private readonly Dictionary<ComponentTypeCollection, List<Type>> _systemsRequiringComponentTypes;
    private readonly Dictionary<Type, List<ComponentTypeCollection>> _collectionsContainingComponentType;

    public ComponentTypeCollectionService()
    {
        _componentTypeCollections = new();
        _requiredComponentTypes = new();
        _collectionsContainingComponentType = new();
        _systemsRequiringComponentTypes = new();
        AppDomain.CurrentDomain.AssemblyLoad += NewAssemblyLoaded;
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
            LoadAssembly(assembly);
    }

    private void LoadAssembly(Assembly assembly)
    {
        var validSystems = assembly.GetTypes().Where(p => p.IsAssignableTo(typeof(SystemBase)) && !p.IsAbstract).ToList();
        foreach (var valid in validSystems)
        {
            var requirements = valid.GetCustomAttributes<RequireAttribute>().Select(p => p.RequiredType).ToList();
            ComponentTypeCollection cTC = requirements.Count > 0 ? new(valid.GetCustomAttributes<RequireAttribute>().Select(p => p.RequiredType)) : ComponentTypeCollection.Empty;
            if (_componentTypeCollections.TryGetValue(cTC, out var matchingCTC))
            {
                cTC = matchingCTC;
            }
            else
            {
                _componentTypeCollections.Add(cTC);
            }

            _requiredComponentTypes.Add(valid, cTC);
            {
                if (!_systemsRequiringComponentTypes.TryGetValue(cTC, out var list))
                    _systemsRequiringComponentTypes.Add(cTC, list = new());
                list.Add(valid);
            }

            foreach (var componentType in cTC.ComponentTypes)
            {
                if (!_collectionsContainingComponentType.TryGetValue(componentType, out var list))
                    _collectionsContainingComponentType.Add(componentType, list = new());
                list.Add(cTC);
            }
        }
    }

    public ComponentTypeCollection? GetRequiredComponentTypes(Type systemType)
        => _requiredComponentTypes.TryGetValue(systemType, out var list) ? list : null;

    public IEnumerable<Type> GetSystemsRequiringComponentTypeCollection(ComponentTypeCollection collection)
        => _systemsRequiringComponentTypes.TryGetValue(collection, out var list) ? list : Enumerable.Empty<Type>();

    public IEnumerable<ComponentTypeCollection> GetRequiringComponentTypeCollections(Type componentType)
        => _collectionsContainingComponentType.TryGetValue(componentType, out var list) ? list : Enumerable.Empty<ComponentTypeCollection>();

    public void Dispose()
        => AppDomain.CurrentDomain.AssemblyLoad -= NewAssemblyLoaded;

    private void NewAssemblyLoaded(object? sender, AssemblyLoadEventArgs e)
        => LoadAssembly(e.LoadedAssembly);
}
