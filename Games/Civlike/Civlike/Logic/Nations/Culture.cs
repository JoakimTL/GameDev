using Civlike.Logic.Setup;

namespace Civlike.Logic.Nations;
public sealed class Culture {

	private readonly Dictionary<CultureAspectBase, CultureAspect> _aspects;

	public Culture() {
		this._aspects = [];
	}


	public void AddAspect( CultureAspect aspect ) {
		this._aspects.Add( aspect.Aspect, aspect );
	}

	public void RemoveAspect( CultureAspect aspect ) {
		this._aspects.Remove( aspect.Aspect );
	}

	public CultureAspect GetAspect( CultureAspectBase aspect ) 
		=> this._aspects[aspect];

	public IEnumerable<CultureAspect> GetAspects() 
		=> this._aspects.Values;

}

public sealed class CultureAspect( CultureAspectBase aspect ) {
	public CultureAspectBase Aspect { get; } = aspect;
	public float Salience { get; private set; } = 0;
	public float Polarity { get; private set; } = 0;
	public float Rigidity { get; private set; } = 0;

	public void UpdateValues( float salience, float polarity, float rigidity ) {
		this.Salience = salience;
		this.Polarity = polarity;
		this.Rigidity = rigidity;
	}
}