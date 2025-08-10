using Civlike.World.State;
using System.Text.Json;

namespace Civlike.Diseases;
public sealed class DiseaseStrain {

	private readonly DiseaseStrain? _parent;
	private readonly Tile? _origin;
	private readonly AntibodyCode _antibodyCode;
	private readonly List<DiseaseSymptom> _symptoms;
	private readonly List<DiseaseTransmission> _transmissionModes;

	public DiseaseStrain( IEnumerable<DiseaseSymptom> symptoms, IEnumerable<DiseaseTransmission> transmissions, AntibodyCode antibodyCode, Tile origin ) {
		this._origin = origin;
		this._parent = null;
		this._symptoms = symptoms.ToList();
		this._transmissionModes = transmissions.ToList();
		this._antibodyCode = antibodyCode;
	}

	public DiseaseStrain( DiseaseStrain parent, IEnumerable<DiseaseSymptom> symptoms, IEnumerable<DiseaseTransmission> transmissions, AntibodyCode antibodyCode ) {
		this._parent = parent;
		this._origin = null;
		this._symptoms = symptoms.ToList();
		this._transmissionModes = transmissions.ToList();
		this._antibodyCode = antibodyCode;
	}

	public DiseaseStrain? Parent => this._parent;
	public AntibodyCode AntibodyCode => this._antibodyCode;
	public Tile Origin => (this._parent?.Origin ?? this._origin) ?? throw new Exception( "No origin assigned for disease" );
	public IReadOnlyList<DiseaseSymptom> Symptoms => this._symptoms;
	public IReadOnlyList<DiseaseTransmission> TransmissionModes => this._transmissionModes;
	public int IncubationDaysMean { get; }
	public int InfectiousDaysMean { get; }

	//public float GetFatalityRate() {
	//	return ...;
	//}

	//public DiseaseStrain Mutate( Random random ) {
	//	...;

	//	return ...;
	//}

}

public sealed record DiseaseSymptom( Symptom Symptom, float Severity );
public sealed record DiseaseTransmission( TransmissionBase Transmission, float Effectiveness );

public abstract class TransmissionBase {

}

public sealed class SymptomService {

	private readonly List<Symptom> _symptoms;

	public SymptomService() {
		string baseDirectory = "res/diseases/symptoms/";
		if (!Directory.Exists( baseDirectory ))
			throw new Exception( $"Could not find directory: {baseDirectory}" );
		string[] files = Directory.GetFiles( baseDirectory );
		this._symptoms = files.SelectMany( p => JsonSerializer.Deserialize<List<Symptom>>( File.ReadAllText( p ) ) ?? Enumerable.Empty<Symptom>() ).OfType<Symptom>().ToList();
	}

	public IReadOnlyList<Symptom> Symptoms => this._symptoms;

}

public sealed record Symptom( string Tag, string Name, string Description, float Severity, float Detectability );