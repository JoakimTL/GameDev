using Engine.Datatypes.Vectors;
using Engine.GameLogic.ECS.Components;
using Engine.Utilities;
using System.Buffers;
using System.Numerics;

namespace Engine.GameLogic.ECS;

public class EntitySpatialGrid3Service : IGameLogicService
{

    public const float GridScale = 64;
    public const float InverseGridScale = 1f / GridScale;

    private readonly Dictionary<Entity, AABB3i> _volumeOccupiedByEntity;
    private readonly Dictionary<Vector3i, EntityByComponentContainer> _entityContainerGrid;
    private readonly EntityContainerService _entityContainerService;
    private readonly ComponentTypeCollectionService _componentTypeCollectionService;

    public EntitySpatialGrid3Service(EntityContainerService entityContainerService, ComponentTypeCollectionService componentTypeCollectionService)
    {
        this._entityContainerService = entityContainerService ?? throw new ArgumentNullException(nameof(entityContainerService));
        this._componentTypeCollectionService = componentTypeCollectionService ?? throw new ArgumentNullException(nameof(componentTypeCollectionService));
        _entityContainerGrid = new();
        _volumeOccupiedByEntity = new();

        this._entityContainerService._container.ComponentAdded += OnComponentAdded;
        this._entityContainerService._container.ComponentRemoved += OnComponentRemoved;
    }

    private void OnComponentAdded(ComponentBase component)
    {
        if (component.Owner is null)
            return;

        var type = component.GetType();
        if (component is Transform3Component t3c)
        {
            Vector3 gScale = t3c.Transform.GlobalScale;
            Vector3 gTranslation = t3c.Transform.GlobalTranslation;
            var equalSidedBound = new Vector3(MathF.MaxMagnitude(MathF.MaxMagnitude(gScale.X, gScale.Y), gScale.Z) * Constants.Sqrt2);
            var volume = new AABB3i(GetGridCoordinate(gTranslation - equalSidedBound), GetGridCoordinate(gTranslation + equalSidedBound));
            _volumeOccupiedByEntity[component.Owner] = volume;

            AddToGrids(volume.GetPointsInVolumeInclusive(), component.Owner);
            t3c.ComponentChanged += TransformChanged;
        }
        else
        {
            if (component.Owner.Get<Transform3Component>() is not null && _volumeOccupiedByEntity.TryGetValue(component.Owner, out var volume))
            {
                foreach (var gridCoordinate in volume.GetPointsInVolumeInclusive())
                    GetGrid(gridCoordinate).ComponentAdded(component);
            }
        }
    }

    private void OnComponentRemoved(ComponentBase component)
    {
        if (component.Owner is null)
            return;
        if (component is Transform3Component t3c)
        {
            if (_volumeOccupiedByEntity.TryGetValue(component.Owner, out var volume))
                RemoveFromGrids(volume.GetPointsInVolumeInclusive(), component.Owner);
            t3c.ComponentChanged -= TransformChanged;
            _volumeOccupiedByEntity.Remove(component.Owner);
        }
        else
        {
            if (component.Owner.Get<Transform3Component>() is not null && _volumeOccupiedByEntity.TryGetValue(component.Owner, out var volume))
            {
                foreach (var gridCoordinate in volume.GetPointsInVolumeInclusive())
                    GetGrid(gridCoordinate).ComponentRemoved(component);
            }
        }
    }

    private void TransformChanged(ComponentBase component)
    {
        if (component.Owner is null || component is not Transform3Component t3c)
            return;

        var volume = _volumeOccupiedByEntity[component.Owner];

        Vector3 gScale = t3c.Transform.GlobalScale;
        Vector3 gTranslation = t3c.Transform.GlobalTranslation;
        var equalSidedBound = new Vector3(MathF.MaxMagnitude(MathF.MaxMagnitude(gScale.X, gScale.Y), gScale.Z) * Constants.Sqrt2);
        var newVolume = new AABB3i(GetGridCoordinate(gTranslation - equalSidedBound), GetGridCoordinate(gTranslation + equalSidedBound));

        if (volume == newVolume)
            return;

        var newGrids = newVolume.ExceptInclusive(volume);
        var oldGrids = volume.ExceptInclusive(newVolume);

        AddToGrids(newGrids, component.Owner);
        RemoveFromGrids(oldGrids, component.Owner);

        _volumeOccupiedByEntity[component.Owner] = newVolume;
    }

    private void AddToGrids(IEnumerable<Vector3i> gridCoordinates, Entity e)
    {
        var grids = gridCoordinates.Select(GetGrid).ToArray();
        foreach (var c in e.Components)
            foreach (var g in grids)
                g.ComponentAdded(c);
    }

    private void RemoveFromGrids(IEnumerable<Vector3i> gridCoordinates, Entity e)
    {
        var grids = gridCoordinates.Select(GetGrid).ToArray();
        foreach (var c in e.Components)
            foreach (var g in grids)
                g.ComponentRemoved(c);
    }

    private EntityByComponentContainer GetGrid(Vector3i gridCoordinate)
    {
        if (!_entityContainerGrid.TryGetValue(gridCoordinate, out var grid))
            _entityContainerGrid.Add(gridCoordinate, grid = new(_componentTypeCollectionService));
        return grid;
    }

    public IEnumerable<EntityByComponentContainer> GetActiveGrids()
    {
        return _entityContainerGrid.Values.Where(p => p.Any()); //Can be optimized to not use LINQ
    }

    private Vector3i GetGridCoordinate(Vector3 worldTranslation) => Vector3i.Floor(worldTranslation * InverseGridScale);

}
