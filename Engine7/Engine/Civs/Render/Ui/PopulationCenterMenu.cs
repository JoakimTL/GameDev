using Civs.Logic.Nations;
using Civs.World;
using Engine;
using Engine.Modularity;
using Engine.Module.Render.Entities.Providers;
using Engine.Standard.Render.UserInterface;
using Engine.Standard.Render.UserInterface.Standard;

namespace Civs.Render.Ui;

public sealed class PopulationCenterMenu() : UserInterfaceElementWithMessageNodeBase( "ui_popcenter" ) {

	private Label _populationCenterNameLabel = null!;

	protected override void Initialize() {
		_populationCenterNameLabel = new Label( this ) {
			Text = "noupdate",
			FontName = "calibrib",
			TextScale = 0.5f,
			Color = (1, 1, 1, 1),
			HorizontalAlignment = Alignment.Center,
			VerticalAlignment = Alignment.Negative
		};
		_populationCenterNameLabel.Placement.Set( new( (0, -.1), 0, (.4, .1) ), Alignment.Center, Alignment.Positive );

	}

	protected override void OnMessageReceived( Message message ) {

	}

	protected override void OnUpdate( double time, double deltaTime ) {
		base.OnUpdate( time, deltaTime );
		FaceOwnershipComponent? populationCenter = GetSelectedTileOwner();
		if (populationCenter is null) {
			_populationCenterNameLabel.Text = "No population center selected";
			return;
		}
		_populationCenterNameLabel.Text = $"{populationCenter.Entity.GetComponentOrDefault<PopulationCenterComponent>()?.Name ?? $"Missing {nameof( PopulationCenterComponent )}!"}";
	}

	protected override bool ShouldDisplay() {
		return GetSelectedTileOwner() is not null;
	}

	private FaceOwnershipComponent? GetSelectedTileOwner() {
		Face? selectedTile = GameStateProvider.Get<Face>( "selectedTile" );
		if (selectedTile is null)
			return null;
		Guid? localPlayer = GameStateProvider.Get<Guid?>( "localPlayerId" );
		if (!localPlayer.HasValue)
			return null;
		Engine.Module.Entities.Container.SynchronizedEntityContainer? container = UserInterfaceServiceAccess.Get<SynchronizedEntityContainerProvider>().SynchronizedContainers
			.FirstOrDefault();
		if (container is null)
			return null;
		List<Engine.Module.Entities.Container.SynchronizedEntity> entitiesOwnedByPlayer = container.SynchronizedEntities.Where( p => p.EntityCopy?.ParentId == localPlayer.Value ).ToList();
		List<FaceOwnershipComponent> focs = entitiesOwnedByPlayer.Select( p => p.EntityCopy?.GetComponentOrDefault<FaceOwnershipComponent>() ).OfType<FaceOwnershipComponent>().ToList();
		return focs.FirstOrDefault( p => p.OwnedFaces.Contains( selectedTile ) );
	}
}