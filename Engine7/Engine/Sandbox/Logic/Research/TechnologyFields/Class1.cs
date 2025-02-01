using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Research.TechnologyFields;
internal class Class1 {
}

public sealed class Sciences() : TechnologyFieldBase( "Sciences" );
[SubfieldOf<Sciences>]
public sealed class Mathematics() : TechnologyFieldBase( "Mathematics" );
[SubfieldOf<Mathematics>]
public sealed class Physics() : TechnologyFieldBase( "Physics" );
[SubfieldOf<Physics>]
public sealed class Astronomy() : TechnologyFieldBase( "Astronomy" );

[SubfieldOf<Sciences>]
public sealed class Chemistry() : TechnologyFieldBase( "Chemistry" );
[SubfieldOf<Sciences>]
public sealed class Alchemy() : TechnologyFieldBase( "Alchemy" );
[SubfieldOf<Chemistry>]
public sealed class Biology() : TechnologyFieldBase( "Biology" );

[SubfieldOf<Sciences>]
public sealed class Engineering() : TechnologyFieldBase( "Engineering" );
[SubfieldOf<Engineering>]
[SubfieldOf<Physics>]
public sealed class CivilEngineering() : TechnologyFieldBase( "Civil Engineering" );
[SubfieldOf<Engineering>]
[SubfieldOf<Chemistry>]
public sealed class ChemicalEngineering() : TechnologyFieldBase( "Chemical Engineering" );


public sealed class Manufacturing() : TechnologyFieldBase( "Manufacturing" );
public sealed class ArtisanManufacturing() : TechnologyFieldBase( "Artisan Manufacturing" );
public sealed class FactoryManufacturing() : TechnologyFieldBase( "Factory Manufacturing" );
public sealed class AutomatedManufacturing() : TechnologyFieldBase( "Automated Manufacturing" );
[SubfieldOf<ArtisanManufacturing>]
public sealed class Baking() : TechnologyFieldBase( "Baking" );
[SubfieldOf<FactoryManufacturing>]
public sealed class AutomobileManufacturing() : TechnologyFieldBase( "Automobile Manufacturing" );


public sealed class Agriculture() : TechnologyFieldBase( "Agriculture" );
[SubfieldOf<Agriculture>]
public sealed class SustenanceFarming() : TechnologyFieldBase( "Sustenance Farming" );
[SubfieldOf<Agriculture>]
public sealed class MonetizedFarming() : TechnologyFieldBase( "Monetized Farming" );