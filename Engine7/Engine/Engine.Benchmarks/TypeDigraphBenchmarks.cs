using BenchmarkDotNet.Attributes;
using Engine.Processing;
using Engine.Structures;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class TypeDigraphBenchmarks {

	private sealed class Crafting;
	private sealed class Smelting;
	private sealed class Mining;
	private sealed class Log;
	[Do<Crafting>.After<Log>]
	private sealed class Plank;
	[Do<Crafting>.After<Plank>]
	private sealed class Stick;
	[Do<Crafting>.After<Plank>]
	[Do<Crafting>.After<Stick>]
	private sealed class WoodenPickaxe;
	[Do<Mining>.After<WoodenPickaxe>]
	private sealed class Cobblestone;
	[Do<Crafting>.After<Cobblestone>]
	[Do<Crafting>.After<Stick>]
	private sealed class StonePickaxe;
	[Do<Mining>.After<StonePickaxe>]
	private sealed class IronOre;
	[Do<Crafting>.After<Cobblestone>]
	private sealed class Furnace;
	[Do<Smelting>.After<IronOre>]
	[Do<Smelting>.After<Furnace>]
	private sealed class IronIngot;
	[Do<Crafting>.After<IronIngot>]
	[Do<Crafting>.After<Stick>]
	private sealed class IronPickaxe;

	private IEnumerable<Type>? _currentTree;
	private readonly TypeDigraph<Crafting> _craftingTree = new();

	[GlobalSetup]
	public void Setup() {
		this._craftingTree.Add<Log>();
		this._craftingTree.Add<Plank>();
		this._craftingTree.Add<Stick>();
		this._craftingTree.Add<WoodenPickaxe>();
		this._craftingTree.Add<Cobblestone>();
		this._craftingTree.Add<StonePickaxe>();
		this._craftingTree.Add<Furnace>();
		this._craftingTree.Add<IronOre>();
		this._craftingTree.Add<IronIngot>();
		this._craftingTree.Add<IronPickaxe>();
	}

	//I want a benchmark of all the crafting process types, with memory allocation as well.
	[Benchmark]
	public void Run() {
		this._craftingTree.AllocSort();
		this._currentTree = this._craftingTree.GetTypes();
	}

	//I want a benchmark of all the crafting process types, with memory allocation as well.
	[Benchmark]
	public void RunOptimized() {
		this._craftingTree.NoAllocSort();
		this._currentTree = this._craftingTree.GetTypes();
	}
}
