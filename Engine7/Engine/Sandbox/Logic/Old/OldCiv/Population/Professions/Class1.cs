namespace Sandbox.Logic.Old.OldCiv.Population.Professions;
internal class Class1 {
}

public sealed class Peasant() : ProfessionBase( "Peasant"/*, TechnologyFieldList.GetTechField<SubsistenceFarming>()*/ );

public sealed class Farmer() : ProfessionBase( "Farmer"/*, TechnologyFieldList.GetTechField<SubsistenceFarming>(), TechnologyFieldList.GetTechField<MonetizedFarming>()*/ );

