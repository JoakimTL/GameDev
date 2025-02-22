using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Old.OldCiv.Research;
public abstract class TechnologyBase {
	protected TechnologyBase( string displayName, TechnologyKind kind, float requiredResearchProgress, params TechnologyFieldBase[] technologyFields ) {
		this.DisplayName = displayName;
		this.Kind = kind;
		this.RequiredDiscoveryProgress = requiredResearchProgress;
		this.TechnologyFields = technologyFields.ToHashSet();
		var requirements = TypeManager.ResolveType( GetType() ).GetAttributes<IProcessDirection>().Where( p => p.ProcessType.IsAssignableTo( typeof( ResearchRequirementBase ) ) );

	}

	public string DisplayName { get; }
	public TechnologyKind Kind { get; }
	public IReadOnlySet<TechnologyFieldBase> TechnologyFields { get; }
	public IReadOnlySet<Type> TechnologyRequirements { get; }
	public IReadOnlySet<Type> ResourceRequirements { get; }
	public IReadOnlySet<Type> BuildingRequirements { get; }
	public float RequiredDiscoveryProgress { get; }

	/// <summary>
	/// Modifies the discovery chance of the technology. Default value is <c>1</c>.
	/// </summary>
	public abstract float GetDiscoveryChanceModifier( TechnologyResearcher techHolder );
	/// <summary>
	/// Modifies the discovery progression rate of the technology. Default value is <c>1</c>.
	/// </summary>
	public abstract float GetResearchProgressionModifier( TechnologyResearcher techHolder );
}

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?