using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Research;
public abstract class TechnologyBase {
	protected TechnologyBase( string displayName, TechnologyKind kind, float requiredDiscoveryProgress, params TechnologyFieldBase[] technologyFields ) {
		this.DisplayName = displayName;
		this.Kind = kind;
		this.RequiredDiscoveryProgress = requiredDiscoveryProgress;
		this.TechnologyFields = technologyFields.ToHashSet();
	}

	public string DisplayName { get; }
	public TechnologyKind Kind { get; }
	public IReadOnlySet<TechnologyFieldBase> TechnologyFields { get; }
	public float RequiredDiscoveryProgress { get; }

	/// <summary>
	/// Discovery can't happen unless the prerequisites are met.
	/// </summary>
	public abstract bool HasPrerequisites( TileTechnologyHolder techHolder );
	/// <summary>
	/// The chance of discovering the technology each day per person working within the field of this technology. This is a value between 0 and 1.<br/>
	/// Discovery happens either by pure luck through hitting the discovery chance or by progressing discovery to 100% through <see cref="GetDiscoveryProgression(TileTechnologyHolder)"/>.
	/// </summary>
	public abstract float GetDiscoveryChance( TileTechnologyHolder techHolder );
	/// <summary>
	/// The progression of discovery for this technology each day per person working within the field of this technology. This is a value between 0 and 1.<br/>
	/// Discovery happens either by pure luck through hitting the <see cref="GetDiscoveryChance(TileTechnologyHolder)"/> or by progressing discovery to 100%.
	/// </summary>
	public abstract float GetDiscoveryProgression( TileTechnologyHolder techHolder );
}

//We need to have the list of tech, and also the ongoing research per player.

//Research is performed by having an educated populace. In the beginning many techs are "simple" enough that they will eventually be found anyways. Research progress ever so slowly as long as someone works with the related tech field. If someone work with crops, then crop rotation will eventually be discovered. For brand new research then a "eureka moment" might be necessary?