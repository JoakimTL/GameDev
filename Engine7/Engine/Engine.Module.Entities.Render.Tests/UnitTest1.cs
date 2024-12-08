using Engine.Module.Entities.Container;

namespace Engine.Module.Entities.Render.Tests;

[TestFixture]
public class RenderEntityContainerTests {

	private class Translation3Component : ComponentBase {
		private Vector3<double> _translation;
		public Vector3<double> Translation {
			get => this._translation;
			set {
				if (this._translation == value)
					return;
				this._translation = value;
				InvokeComponentChanged();
			}
		}
	}

	private class Motion3Component : ComponentBase {
		private Vector3<double> _velocity;
		public Vector3<double> Velocity {
			get => this._velocity;
			set {
				if (this._velocity == value)
					return;
				this._velocity = value;
				InvokeComponentChanged();
			}
		}
	}

	private class Moving3Archetype : ArchetypeBase {
		public Translation3Component Translation { get; set; } = null!;
		public Motion3Component Motion { get; set; } = null!;
	}

	private class Moving3System : SystemBase<Moving3Archetype> {
		protected override void ProcessEntity( Moving3Archetype archetype, double time, double deltaTime ) {
			archetype.Translation.Translation += archetype.Motion.Velocity * deltaTime;
		}
	}

	private class Moving3RenderBehaviour : SynchronizedRenderBehaviourBase<Moving3Archetype> {
		public Vector3<double> _newTranslation;
		public Vector3<double> _newVelocity;
		public Vector3<float> CurrentTranslation { get; private set; }
		public Vector3<float> CurrentVelocity { get; private set; }

		//protected override void Initialize() {
		//	this._newTranslation = this.Archetype.Translation.Translation;
		//	this._newVelocity = this.Archetype.Motion.Velocity;
		//	this.CurrentTranslation = this._newTranslation.CastSaturating<double, float>();
		//	this.CurrentVelocity = this._newVelocity.CastSaturating<double, float>();
		//}

		protected override bool PrepareSynchronization( ComponentBase component ) {
			if (component is Translation3Component translation) {
				this._newTranslation = translation.Translation;
				return true;
			}

			if (component is Motion3Component motion) {
				this._newVelocity = motion.Velocity;
				return true;
			}

			return false;
		}

		protected override void Synchronize() {
			this.CurrentTranslation = this._newTranslation.CastSaturating<double, float>();
			this.CurrentVelocity = this._newVelocity.CastSaturating<double, float>();
		}
	}

	[Test]
	public void DependentRenderBehaviourIsAddedAndRemovedCorrectly() {
		EntityContainer container = new();
		Entity entity = container.CreateEntity();
		Translation3Component translation = entity.AddComponent<Translation3Component>();
		Motion3Component motion = entity.AddComponent<Motion3Component>();
		motion.Velocity = new Vector3<double>( 1, 2, 3 );
		entity.AddComponent<RenderComponent>();

		RenderEntityContainer renderContainer = new( container, null );

		Assert.That( renderContainer.PendingEntitiesToAdd, Is.EqualTo( 1 ) );
		container.SystemManager.Update( 0, 0);
		renderContainer.Update( 0, 0 );
		Assert.That( renderContainer.PendingEntitiesToAdd, Is.EqualTo( 0 ) );
		Assert.That( renderContainer.RenderEntities.Count, Is.EqualTo( 1 ) );
		RenderEntity renderEntity = renderContainer.RenderEntities.First();
		bool success = renderEntity.TryGetBehaviour( out Moving3RenderBehaviour? renderBehaviour );
		Assert.IsTrue( success );
		Assert.That( renderBehaviour, Is.Not.Null );
		Assert.That( renderBehaviour!._newVelocity, Is.EqualTo( new Vector3<double>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour._newTranslation, Is.EqualTo( Vector3<double>.Zero ) );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( Vector3<float>.Zero ) );

		container.SystemManager.Update( 0.25, 0.25 );
		Assert.That( renderBehaviour!._newVelocity, Is.EqualTo( new Vector3<double>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour._newTranslation, Is.EqualTo( new Vector3<double>( 0.25, 0.5, 0.75 ) ) );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( Vector3<float>.Zero ) );
		renderContainer.Update( 0.25, 0.25 );
		Assert.That( renderBehaviour!._newVelocity, Is.EqualTo( new Vector3<double>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour._newTranslation, Is.EqualTo( new Vector3<double>( 0.25, 0.5, 0.75 ) ) );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( new Vector3<float>( 0.25f, 0.5f, 0.75f ) ) );

		entity.RemoveComponent<RenderComponent>();
		Assert.That( renderContainer.PendingEntitiesToRemove, Is.EqualTo( 1 ) );

		container.SystemManager.Update( 0.5, 0.25 );
		Assert.That( renderBehaviour!._newVelocity, Is.EqualTo( new Vector3<double>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour._newTranslation, Is.EqualTo( new Vector3<double>( 0.5, 1, 1.5 ) ) );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( new Vector3<float>( 0.25f, 0.5f, 0.75f ) ) );
		renderContainer.Update( 0.5, 0.25 );
		Assert.That( renderContainer.PendingEntitiesToRemove, Is.EqualTo( 0 ) );
		Assert.That( renderContainer.RenderEntities.Count, Is.EqualTo( 0 ) );


	}

}