namespace Sandbox.Logic.Setup;

public sealed class DefinitionList<TDefinition> where TDefinition : SelfIdentifyingBase {

	private readonly Dictionary<Type, TDefinition> _definitionsByType;
	private readonly Dictionary<Guid, TDefinition> _definitionsById;

	public DefinitionList() {
		_definitionsByType = [];
		_definitionsById = [];
		IEnumerable<ResolvedType> resolvedTypes = TypeManager.Registry.ImplementationTypes
			.Where( p => p.IsAssignableTo( typeof( TDefinition ) ) )
			.Select( p => p.Resolve() );
		foreach (ResolvedType resolvedType in resolvedTypes) {
			if (!resolvedType.HasParameterlessConstructor)
				throw new InvalidOperationException( $"{resolvedType.Type.Name} does not have a parameterless constructor." );
			TDefinition definition = resolvedType.CreateInstance( null ) as TDefinition ?? throw new InvalidOperationException( $"{resolvedType.Type.Name} could not be instantiated." );
			_definitionsByType.Add( resolvedType.Type, definition );
			_definitionsById.Add( definition.Id, definition );
		}
	}

	public T Get<T>() where T : TDefinition => (T) _definitionsByType[ typeof( T ) ];
	public TDefinition Get( Guid id ) => _definitionsById[ id ];
}
