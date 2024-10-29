namespace Engine.Tests;

[TestFixture]
public class TypeDigraphTest {

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
	[Process.After<IronPickaxe, Crafting>]
	[Process.Before<Plank, Crafting>]
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