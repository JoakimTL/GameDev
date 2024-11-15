using BenchmarkDotNet.Attributes;

namespace Engine.Module.Entities.Benchmarks;

[MemoryDiagnoser]
public class EntityArchetypeTypeManagerBenchmarks {

	public sealed class TestComponent : ComponentBase {
		public int Value { get; set; }
	}

	public sealed class TestComponent2 : ComponentBase {
		public int Value { get; set; }
	}

	public sealed class TestArchetype : ArchetypeBase {
		public TestComponent TestComponent { get; set; } = null!;
		public TestComponent2 TestComponent2 { get; set; } = null!;

		//public static TestArchetype Initialize( Entity entity ) {
		//	TestArchetype a = new();
		//	a.SetEntity( entity );
		//	entity.TryGetComponent( out TestComponent? testComponent  );
		//	entity.TryGetComponent( out TestComponent2? testComponent2 );
		//	a.TestComponent = testComponent!;
		//	a.TestComponent2 = testComponent2!;
		//	return a;
		//}
	}

	private TestArchetype? _currentArchetype;
	private readonly EntityContainer _entityContainer = new();
	private Entity? _e;

	[GlobalSetup]
	public void Setup() {
		this._e = this._entityContainer.CreateEntity();
		this._e.AddComponent<TestComponent>().Value = 10;
		this._e.AddComponent<TestComponent2>();
	}

	[Benchmark]
	public void Run() {
		this._currentArchetype = (TestArchetype) this._e!.CreateArchetypeInstance( typeof( TestArchetype ) );
	}

	//[Benchmark]
	//public void RunPrecompiled() {
	//	_currentArchetype = TestArchetype.Initialize(_e);
	//}

}
