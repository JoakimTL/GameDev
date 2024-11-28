using Engine.Processing;
using Engine.Structures;

namespace Engine.Tests;

[TestFixture]
public class TypeDigraphTest {

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
	[Do<Crafting>.After<IronPickaxe>]
	[Do<Crafting>.Before<Plank>]
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
