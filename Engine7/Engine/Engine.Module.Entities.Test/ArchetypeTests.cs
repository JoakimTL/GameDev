namespace Engine.Module.Entities.Test;

[TestFixture]
public class ArchetypeTests {

	public sealed class TestComponent : ComponentBase {
		public int Value { get; set; }
	}


	public sealed class TestComponent2 : ComponentBase {
		public int Value { get; set; }
	}

	public sealed class TestArchetype : ArchetypeBase {
		public TestComponent TestComponent { get; set; } = null!;
		public TestComponent2 TestComponent2 { get; set; } = null!;
	}

	[Test]
	public void TestArchetypeProperlyInstantiated() {
		EntityContainer entityContainer = new();
		Entity entity = entityContainer.CreateEntity();
		entity.AddComponent<TestComponent>().Value = 10;
		entity.AddComponent<TestComponent2>();
		TestArchetype? archetype = entityContainer.ArchetypeManager.GetArchetypes<TestArchetype>().FirstOrDefault();

		Assert.NotNull( archetype );

		bool success = entity.TryGetComponent( out TestComponent? testComponent );
		Assert.That( success, Is.True );
		success = entity.TryGetComponent( out TestComponent2? testComponent2 );
		Assert.That( success, Is.True );

		Assert.That( archetype.Entity, Is.EqualTo( entity ) );
		Assert.That( archetype.TestComponent, Is.EqualTo( testComponent ) );
		Assert.That( archetype.TestComponent2, Is.EqualTo( testComponent2 ) );
	}

}

public class SystemManagerTests {

	public sealed class TestComponent : ComponentBase {
		public int Value { get; set; }
	}


	public sealed class TestComponent2 : ComponentBase {
		public int Value { get; set; }
	}

	public sealed class TestArchetype : ArchetypeBase {
		public TestComponent TestComponent { get; set; } = null!;
		public TestComponent2 TestComponent2 { get; set; } = null!;
	}

	public sealed class TestSystem : SystemBase<TestArchetype> {
		protected override void ProcessEntity( TestArchetype archetype, double time, double deltaTime ) {
			archetype.TestComponent.Value += 1;
			archetype.TestComponent2.Value += 1;
		}
	}

	[Test]
	public void TestSystemProcessesEntities() {
		Assert.That( TypeManager.AllTypes.Contains( typeof( TestSystem ) ), Is.True );
		EntityContainer entityContainer = new();
		Entity entity = entityContainer.CreateEntity();
		entity.AddComponent<TestComponent>().Value = 10;
		entity.AddComponent<TestComponent2>();

		entityContainer.SystemManager.Update( 0, 0 );

		bool success = entity.TryGetComponent( out TestComponent? testComponent );
		Assert.That( success, Is.True );
		success = entity.TryGetComponent( out TestComponent2? testComponent2 );
		Assert.That( success, Is.True );

		Assert.That( testComponent.Value, Is.EqualTo( 11 ) );
		Assert.That( testComponent2.Value, Is.EqualTo( 1 ) );
	}

}