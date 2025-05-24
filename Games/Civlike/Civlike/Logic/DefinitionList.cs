using Engine;
using Engine.Logging;
using System.Reflection;

namespace Civlike.Logic;

public sealed class DefinitionList<TDefinition> where TDefinition : SelfIdentifyingBase {

	private readonly List<TDefinition?> _definitions;
	private readonly Dictionary<Type, TDefinition> _definitionsByType;
	private readonly Dictionary<Guid, TDefinition> _definitionsById;

	public DefinitionList() {
		this._definitions = [];
		this._definitions.Add( null );
		this._definitionsByType = [];
		this._definitionsById = [];
		IEnumerable<ResolvedType> resolvedTypes = TypeManager.Registry.ImplementationTypes
			.Where( p => p.IsAssignableTo( typeof( TDefinition ) ) )
			.Select( p => p.Resolve() );
		PropertyInfo memoryIdProperty = typeof( SelfIdentifyingBase ).GetProperty( nameof( SelfIdentifyingBase.MemoryId ) ) ?? throw new InvalidOperationException( $"{nameof( SelfIdentifyingBase.MemoryId )} property not found." );
		TypePropertyAccessor propertyAccessor = typeof( SelfIdentifyingBase ).Resolve().GetPropertyAccessor( memoryIdProperty );
		foreach (ResolvedType resolvedType in resolvedTypes) {
			if (!resolvedType.HasParameterlessConstructor)
				throw new InvalidOperationException( $"{resolvedType.Type.Name} does not have a parameterless constructor." );
			TDefinition definition = resolvedType.CreateInstance( null ) as TDefinition ?? throw new InvalidOperationException( $"{resolvedType.Type.Name} could not be instantiated." );
			propertyAccessor.WriteProperty( definition, this._definitions.Count );
			this._definitions.Add( definition );
			this._definitionsByType.Add( resolvedType.Type, definition );
			this._definitionsById.Add( definition.Id, definition );
		}
	}

	public uint Count => (uint) this._definitions.Count;

	public TDefinition? Get( uint memoryId )
		=> memoryId >= this._definitions.Count
			? this.LogWarningThenReturnDefault<TDefinition>( $"Definition with memory id {memoryId} not found." )
			: this._definitions[ (int) memoryId ];
	public T Get<T>() where T : TDefinition => (T) this._definitionsByType[ typeof( T ) ];
	public TDefinition Get( Guid id ) => this._definitionsById[ id ];
}
