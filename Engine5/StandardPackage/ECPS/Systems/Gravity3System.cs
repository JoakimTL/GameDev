using Engine.GameLogic.ECPS;
using Engine.GameLogic.ECPS.Components;
using Engine.Structure.Attributes;
using StandardPackage.ECPS.Components;

namespace StandardPackage.ECPS.Systems;

[ProcessBefore<Movement3System, SystemBase>]
[Require<Transform3Component>]
[Require<LinearMovement3Component>]
public class Gravity3System : SystemBase {
	private readonly IGravity3ValueProvider _settings;

	public Gravity3System( IGravity3ValueProvider settings ) {
		this._settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
	}

	public override void Update( IEnumerable<Entity> entities, float time, float deltaTime ) {
		foreach ( Entity e in entities ) {
			Transform3Component transform = e.Get<Transform3Component>() ?? throw new NullReferenceException( nameof( Transform3Component ) );
			LinearMovement3Component linMov = e.Get<LinearMovement3Component>() ?? throw new NullReferenceException( nameof( LinearMovement3Component ) );

			var g = _settings.GetGravity( transform.Transform.GlobalTranslation );
			if ( _settings.IsAcceleration )
				linMov.Accelerate( g );
			else
				linMov.ApplyForce( g );
		}
	}
}
