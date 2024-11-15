using Engine.Structures;

namespace Engine.Tests;

[TestFixture]
public class TypeDigraphTest {

	private sealed class Crafting;
	private sealed class Smelting;
	private sealed class Mining;
	private sealed class Log;
	[Do.After<Log, Crafting>]
	private sealed class Plank;
	[Do.After<Plank, Crafting>]
	private sealed class Stick;
	[Do.After<Plank, Crafting>]
	[Do.After<Stick, Crafting>]
	private sealed class WoodenPickaxe;
	[Do.After<WoodenPickaxe, Mining>]
	private sealed class Cobblestone;
	[Do.After<Cobblestone, Crafting>]
	[Do.After<Stick, Crafting>]
	private sealed class StonePickaxe;
	[Do.After<StonePickaxe, Mining>]
	private sealed class IronOre;
	[Do.After<Cobblestone, Crafting>]
	private sealed class Furnace;
	[Do.After<IronOre, Smelting>]
	[Do.After<Furnace, Smelting>]
	private sealed class IronIngot;
	[Do.After<IronIngot, Crafting>]
	[Do.After<Stick, Crafting>]
	private sealed class IronPickaxe;
	[Do.After<IronPickaxe, Crafting>]
	[Do.Before<Plank, Crafting>]
	private sealed class Looper;

	[Test]
	public void TypeWithDifferentLayeredDependencies() {
		TypeDigraph<Crafting> craftingTree = new();

		craftingTree.Add<Log>();
		craftingTree.Add<Plank>();
		craftingTree.Add<Stick>();
		craftingTree.Add<WoodenPickaxe>();
		craftingTree.Add<Cobblestone>();
		craftingTree.Add<StonePickaxe>();
		craftingTree.Add<Furnace>();
		craftingTree.Add<IronOre>();
		craftingTree.Add<IronIngot>();
		craftingTree.Add<IronPickaxe>();

		CollectionAssert.AreEqual( new[] {
			typeof( Log ),
			typeof( Cobblestone ),
			typeof( IronOre ),
			typeof( IronIngot ),
			typeof( Plank ),
			typeof( Furnace ),
			typeof( Stick ),
			typeof( WoodenPickaxe ),
			typeof( StonePickaxe ),
			typeof( IronPickaxe )
		}, craftingTree.GetTypes() );
	}
	[Test]
	public void TypeWithDifferentLayeredDependenciesOptimized() {
		TypeDigraph<Crafting> craftingTree = new();

		craftingTree.Add<Log>();
		craftingTree.Add<Plank>();
		craftingTree.Add<Stick>();
		craftingTree.Add<WoodenPickaxe>();
		craftingTree.Add<Cobblestone>();
		craftingTree.Add<StonePickaxe>();
		craftingTree.Add<Furnace>();
		craftingTree.Add<IronOre>();
		craftingTree.Add<IronIngot>();
		craftingTree.Add<IronPickaxe>();

		craftingTree.NoAllocSort();

		CollectionAssert.AreEqual( new[] {
			typeof( Log ),
			typeof( Cobblestone ),
			typeof( IronOre ),
			typeof( IronIngot ),
			typeof( Plank ),
			typeof( Furnace ),
			typeof( Stick ),
			typeof( WoodenPickaxe ),
			typeof( StonePickaxe ),
			typeof( IronPickaxe )
		}, craftingTree.GetTypes() );
	}

	[Test]
	public void LoopTest() {
		TypeDigraph<Crafting> craftingTree = new();

		craftingTree.Add<Log>();
		craftingTree.Add<Plank>();
		craftingTree.Add<Stick>();
		craftingTree.Add<WoodenPickaxe>();
		craftingTree.Add<Cobblestone>();
		craftingTree.Add<StonePickaxe>();
		craftingTree.Add<Furnace>();
		craftingTree.Add<IronOre>();
		craftingTree.Add<IronIngot>();
		craftingTree.Add<IronPickaxe>();
		craftingTree.Add<Looper>();

		Assert.Throws<InvalidOperationException>( craftingTree.AllocSort );
	}

	[Test]
	public void LoopTestOptimized() {
		TypeDigraph<Crafting> craftingTree = new();

		craftingTree.Add<Log>();
		craftingTree.Add<Plank>();
		craftingTree.Add<Stick>();
		craftingTree.Add<WoodenPickaxe>();
		craftingTree.Add<Cobblestone>();
		craftingTree.Add<StonePickaxe>();
		craftingTree.Add<Furnace>();
		craftingTree.Add<IronOre>();
		craftingTree.Add<IronIngot>();
		craftingTree.Add<IronPickaxe>();
		craftingTree.Add<Looper>();


		Assert.Throws<InvalidOperationException>( craftingTree.NoAllocSort );
	}
}
