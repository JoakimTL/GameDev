using Engine.Logging;
using System.Reflection;

namespace Sandbox.Logic.Setup;

public sealed class DefinitionList<TDefinition> where TDefinition : SelfIdentifyingBase {

	private readonly List<TDefinition?> _definitions;
	private readonly Dictionary<Type, TDefinition> _definitionsByType;
	private readonly Dictionary<Guid, TDefinition> _definitionsById;

	public DefinitionList() {
		_definitions = [];
		_definitions.Add( null );
		_definitionsByType = [];
		_definitionsById = [];
		IEnumerable<ResolvedType> resolvedTypes = TypeManager.Registry.ImplementationTypes
			.Where( p => p.IsAssignableTo( typeof( TDefinition ) ) )
			.Select( p => p.Resolve() );
		PropertyInfo memoryIdProperty = typeof( SelfIdentifyingBase ).GetProperty( nameof( SelfIdentifyingBase.MemoryId ) ) ?? throw new InvalidOperationException( $"{nameof( SelfIdentifyingBase.MemoryId )} property not found." );
		TypePropertyAccessor propertyAccessor = typeof( SelfIdentifyingBase ).Resolve().GetPropertyAccessor( memoryIdProperty );
		foreach (ResolvedType resolvedType in resolvedTypes) {
			if (!resolvedType.HasParameterlessConstructor)
				throw new InvalidOperationException( $"{resolvedType.Type.Name} does not have a parameterless constructor." );
			TDefinition definition = resolvedType.CreateInstance( null ) as TDefinition ?? throw new InvalidOperationException( $"{resolvedType.Type.Name} could not be instantiated." );
			propertyAccessor.WriteProperty( definition, _definitions.Count );
			_definitions.Add( definition );
			_definitionsByType.Add( resolvedType.Type, definition );
			_definitionsById.Add( definition.Id, definition );
		}
	}

	public uint Count => (uint) _definitions.Count;

	public TDefinition? Get( uint memoryId )
		=> memoryId >= _definitions.Count
			? this.LogWarningThenReturnDefault<TDefinition>( $"Definition with memory id {memoryId} not found." )
			: _definitions[ (int) memoryId ];
	public T Get<T>() where T : TDefinition => (T) _definitionsByType[ typeof( T ) ];
	public TDefinition Get( Guid id ) => _definitionsById[ id ];
}
