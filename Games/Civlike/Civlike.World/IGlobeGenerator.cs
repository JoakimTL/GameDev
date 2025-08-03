namespace Civlike.World;

public interface IGlobeGenerator {
	int Subdivisions { get; }
	double Radius { get; }
	void GenerateInitialGlobeState( Globe globe );
}