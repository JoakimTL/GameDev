using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Setup;
public abstract class TechnologyBase : SelfIdentifyingBase {
	protected TechnologyBase( string name, double researchProgressRequired ) {
		this.Name = name;
		this.ResearchProgressRequired = researchProgressRequired;
		Requirements = new( GetType() );
	}

	public string Name { get; }
	public double ResearchProgressRequired { get; }
	public RequirementList Requirements { get; }
}

public interface IRequirement {
	Type RequiredType { get; }
}

[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
public sealed class RequiresAttribute<T> : Attribute, IRequirement {
	public Type RequiredType { get; } = typeof( T );
}

public sealed class RequirementList {
	public RequirementList( Type requiringType ) {
		var requirements = TypeManager.ResolveType( requiringType ).GetAttributes<IRequirement>().Select( p => p.RequiredType ).ToList();
		TechnologyRequirements = requirements.Where( p => p.IsAssignableTo( typeof( TechnologyBase ) ) ).ToList();
		ResourceRequirements = requirements.Where( p => p.IsAssignableTo( typeof( ResourceBase ) ) ).ToList();
		BuildingRequirements = [];//requirements.Where( p => p.IsAssignableTo( typeof( BuildingBase ) ) ).ToList();
		ProfessionRequirements = [];//requirements.Where( p => p.IsAssignableTo( typeof( ProfessionBase ) ) ).ToList();
	}

	public IReadOnlyList<Type> TechnologyRequirements { get; }
	public IReadOnlyList<Type> ResourceRequirements { get; }
	public IReadOnlyList<Type> BuildingRequirements { get; }
	public IReadOnlyList<Type> ProfessionRequirements { get; }
}

public static class TechnologyList {
	private static readonly Dictionary<Type, TechnologyBase> _technologies;

	static TechnologyList() {
		_technologies = [];
		IEnumerable<Type> technologyTypes = TypeManager.Registry.ImplementationTypes.Where( p => p.IsAssignableTo( typeof( TechnologyBase ) ) );
		foreach (Type type in technologyTypes) {
			if (!TypeManager.ResolveType( type ).HasParameterlessConstructor)
				throw new InvalidOperationException( $"Technology {type.Name} does not have a parameterless constructor." );
			TechnologyBase technology = type.CreateInstance( null ) as TechnologyBase ?? throw new InvalidOperationException( $"Technology {type.Name} could not be instantiated." );
			_technologies.Add( type, technology );
		}
	}

	public static T Get<T>() where T : TechnologyBase => (T) _technologies[ typeof( T ) ];
}