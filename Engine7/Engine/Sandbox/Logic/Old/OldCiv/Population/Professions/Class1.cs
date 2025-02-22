using Sandbox.Logic.Old.OldCiv.Population;
using Sandbox.Logic.Research;
using Sandbox.Logic.Research.TechnologyFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Old.OldCiv.Population.Professions;
internal class Class1 {
}

public sealed class Peasant() : ProfessionBase( "Peasant"/*, TechnologyFieldList.GetTechField<SubsistenceFarming>()*/ );

public sealed class Farmer() : ProfessionBase( "Farmer"/*, TechnologyFieldList.GetTechField<SubsistenceFarming>(), TechnologyFieldList.GetTechField<MonetizedFarming>()*/ );

