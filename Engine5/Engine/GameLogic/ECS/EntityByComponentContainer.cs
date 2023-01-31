namespace Engine.GameLogic.ECS;

public class EntityByComponentContainer : Identifiable
{

    private readonly ComponentTypeCollectionService _componentTypeCollectionService;
    private readonly Dictionary<ComponentTypeCollection, HashSet<Entity>> _entitiesByCTC;

    public event Action<ComponentTypeCollection>? ComponentTypeCollectionAdded;
    public event Action<ComponentTypeCollection>? ComponentTypeCollectionRemoved;

    protected override string UniqueNameTag => $"{_entitiesByCTC.Values.Sum(p => p.Count)} E / {_entitiesByCTC.Count} CTCs";

    public EntityByComponentContainer(ComponentTypeCollectionService componentTypeCollectionService)
    {
        _componentTypeCollectionService = componentTypeCollectionService ?? throw new ArgumentNullException(nameof(componentTypeCollectionService));
        _entitiesByCTC = new();
    }

    public IEnumerable<Entity> GetEntities(ComponentTypeCollection ctc)
        => _entitiesByCTC.TryGetValue(ctc, out var entities) ? entities : Enumerable.Empty<Entity>();

    public void ComponentAdded(ComponentBase component)
    {
        var requiringComponentTypeCollections = _componentTypeCollectionService.GetRequiringComponentTypeCollections(component.GetType());
        if (requiringComponentTypeCollections is not null)
        {
            var ownerEntity = component.Owner;
            if (ownerEntity is null)
                return;
            foreach (var componentTypeCollection in requiringComponentTypeCollections)
                if (ownerEntity.HasAllComponents(componentTypeCollection))
                {
                    if (!_entitiesByCTC.TryGetValue(componentTypeCollection, out var entities))
                    {
                        _entitiesByCTC.Add(componentTypeCollection, entities = new());
                        ComponentTypeCollectionAdded?.Invoke(componentTypeCollection);
                    }
                    entities.Add(ownerEntity);
                }
        }
    }

    public void ComponentRemoved(ComponentBase component)
    {
        var requiringComponentTypeCollections = _componentTypeCollectionService.GetRequiringComponentTypeCollections(component.GetType());
        if (requiringComponentTypeCollections is not null)
        {
            var ownerEntity = component.Owner;
            if (ownerEntity is null)
                return;
            foreach (var componentTypeCollection in requiringComponentTypeCollections)
            {
                if (_entitiesByCTC.TryGetValue(componentTypeCollection, out var entities))
                {
                    entities.Remove(ownerEntity);
                    if (entities.Count == 0)
                    {
                        _entitiesByCTC.Remove(componentTypeCollection);
                        ComponentTypeCollectionRemoved?.Invoke(componentTypeCollection);
                    }
                }
            }
        }
    }

    public bool Any() => _entitiesByCTC.Any();
}
