using Engine.Data;
using Engine.Structure;

namespace Engine.Modularity.Modules;
public static class SubmodulePriorityManager {

	private static readonly BidirectionalTreeStructureProvider _treeProvider;
	private static readonly Dictionary<Type, uint> _sortedSubmoduleTypes;

	static SubmodulePriorityManager() {
		_treeProvider = new( typeof( IUpdateable ) );
		_sortedSubmoduleTypes = new Dictionary<Type, uint>();

		foreach( Type type in ReflectionMagic.GetAllTypes<Submodule>() )
			_treeProvider.Add( type );

		uint i = 0;
		foreach( Type type in _treeProvider.WalkTreeForward())
			_sortedSubmoduleTypes.Add( type, i++ );
	}

	public static uint GetPriority( Type t ) => _sortedSubmoduleTypes.TryGetValue( t, out uint priority ) ? priority : uint.MaxValue;
}
