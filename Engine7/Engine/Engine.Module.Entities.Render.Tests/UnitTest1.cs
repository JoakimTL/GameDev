using Engine.Buffers;
using Engine.Module.Entities.Container;
using Engine.Module.Render.Entities;
using Engine.Module.Render.Entities.Components;
using Engine.Serialization;
using System.Runtime.InteropServices;

namespace Engine.Module.Entities.Render.Tests;

[TestFixture]
public class RenderEntityContainerTests {

	public class Translation3Component : ComponentBase {
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

	public class Motion3Component : ComponentBase {
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

	public class Moving3Archetype : ArchetypeBase {
		public Translation3Component Translation { get; set; } = null!;
		public Motion3Component Motion { get; set; } = null!;
	}

	public class Moving3System : SystemBase<Moving3Archetype> {
		protected override void ProcessEntity( Moving3Archetype archetype, double time, double deltaTime ) {
			archetype.Translation.Translation += archetype.Motion.Velocity * deltaTime;
		}
	}

	[Guid( "62F2FFD7-743E-4294-B791-57B854BAC5B7" )]
	public sealed class Translation3ComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<Translation3Component>( serializerProvider ) {
		protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, Translation3Component target ) {
			if (serializedData.Length < 24)
				return false;
			target.Translation = MemoryMarshal.Read<Vector3<double>>( serializedData );
			return true;
		}

		protected override void PerformSerialization( ThreadedByteBuffer buffer, Translation3Component t ) {
			Span<byte> data = stackalloc byte[ 24 ];
			MemoryMarshal.Write( data, t.Translation );
			buffer.Add( data );
		}
	}

	[Guid( "C9EBF287-2BEE-44E0-8DB3-685BDC68EF13" )]
	public sealed class Motion3ComponentSerializer( SerializerProvider serializerProvider ) : SerializerBase<Motion3Component>( serializerProvider ) {
		protected override bool PerformDeserialization( ReadOnlySpan<byte> serializedData, Motion3Component target ) {
			if (serializedData.Length < 24)
				return false;
			target.Velocity = MemoryMarshal.Read<Vector3<double>>( serializedData );
			return true;
		}

		protected override void PerformSerialization( ThreadedByteBuffer buffer, Motion3Component t ) {
			Span<byte> data = stackalloc byte[ 24 ];
			MemoryMarshal.Write( data, t.Velocity );
			buffer.Add( data );
		}
	}

	public class Moving3RenderBehaviour : DependentRenderBehaviourBase<Moving3Archetype> {
		public Vector3<float> CurrentTranslation { get; private set; }
		public Vector3<float> CurrentVelocity { get; private set; }

		public override void Update( double time, double deltaTime ) {
			CurrentTranslation = Archetype.Translation.Translation.CastSaturating<double, float>();
			CurrentVelocity = Archetype.Motion.Velocity.CastSaturating<double, float>();
		}

		protected override bool InternalDispose() {
			return true;
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

		IInstanceProvider instanceProvider = InstanceManagement.CreateProvider();
		var sec = new SynchronizedEntityContainer( container, instanceProvider.Get<SerializerProvider>() );

		Assert.That( sec.IncomingEntities, Is.EqualTo( 1 ) );

		IInstanceProvider instanceProvider2 = InstanceManagement.CreateProvider();
		RenderEntityContainer renderContainer = new( sec, null! );

		container.SystemManager.Update( 0, 0 );
		renderContainer.Update( 0, 0 );
		Assert.That( renderContainer.PendingEntitiesToAdd, Is.EqualTo( 0 ) );
		Assert.That( renderContainer.RenderEntities.Count, Is.EqualTo( 1 ) );
		RenderEntity renderEntity = renderContainer.RenderEntities.First();
		bool success = renderEntity.TryGetBehaviour( out Moving3RenderBehaviour? renderBehaviour );
		Assert.IsTrue( success );
		Assert.That( renderBehaviour, Is.Not.Null );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( Vector3<float>.Zero ) );

		container.SystemManager.Update( 0.25, 0.25 );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( Vector3<float>.Zero ) );
		renderContainer.Update( 0.25, 0.25 );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( new Vector3<float>( 0.25f, 0.5f, 0.75f ) ) );

		entity.RemoveComponent<RenderComponent>();

		container.SystemManager.Update( 0.5, 0.25 );
		Assert.That( renderBehaviour.CurrentVelocity, Is.EqualTo( new Vector3<float>( 1, 2, 3 ) ) );
		Assert.That( renderBehaviour.CurrentTranslation, Is.EqualTo( new Vector3<float>( 0.25f, 0.5f, 0.75f ) ) );
		renderContainer.Update( 0.5, 0.25 );
		Assert.That( renderContainer.PendingEntitiesToRemove, Is.EqualTo( 0 ) );
		Assert.That( renderContainer.RenderEntities.Count, Is.EqualTo( 0 ) );


	}

}