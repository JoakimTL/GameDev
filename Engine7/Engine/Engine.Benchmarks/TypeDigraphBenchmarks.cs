using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace Engine.Benchmarks;

[MemoryDiagnoser]
public class TypeDigraphBenchmarks {

	private sealed class Crafting;
	private sealed class Smelting;
	private sealed class Mining;
	private sealed class Log;
	[Process.After<Log, Crafting>]
	private sealed class Plank;
	[Process.After<Plank, Crafting>]
	private sealed class Stick;
	[Process.After<Plank, Crafting>]
	[Process.After<Stick, Crafting>]
	private sealed class WoodenPickaxe;
	[Process.After<WoodenPickaxe, Mining>]
	private sealed class Cobblestone;
	[Process.After<Cobblestone, Crafting>]
	[Process.After<Stick, Crafting>]
	private sealed class StonePickaxe;
	[Process.After<StonePickaxe, Mining>]
	private sealed class IronOre;
	[Process.After<Cobblestone, Crafting>]
	private sealed class Furnace;
	[Process.After<IronOre, Smelting>]
	[Process.After<Furnace, Smelting>]
	private sealed class IronIngot;
	[Process.After<IronIngot, Crafting>]
	[Process.After<Stick, Crafting>]
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
