namespace Sandbox.Logic.Research;

public static class TechnologyFieldList {
	private static readonly Dictionary<Type, TechnologyFieldBase> _technologyFields;

	static TechnologyFieldList() {
		_technologyFields = [];

		foreach (Type technologyFieldType in TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( TechnologyFieldBase ) ) )) {
			TechnologyFieldBase instance = technologyFieldType.CreateInstance( null ) as TechnologyFieldBase ?? throw new InvalidOperationException( $"Failed to create instance of {technologyFieldType}." );
			_technologyFields[ technologyFieldType ] = instance;
		}

		foreach (TechnologyFieldBase techFieldInstance in _technologyFields.Values) {
			techFieldInstance.SetParentFields( TypeManager.ResolveType( techFieldInstance.GetType() ).GetAttributes<ISubfieldOf>().Select( p => _technologyFields[ p.SubfieldOfType ] ).ToHashSet() );
		}
	}

	public static T GetTechField<T>() where T : TechnologyFieldBase
		=> _technologyFields[ typeof( T ) ] as T ?? throw new InvalidOperationException( $"Failed to get instance of {typeof( T )}." );
}

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?