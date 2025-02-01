using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Logic.Research.Technologies;

public sealed class Wheel() : TechnologyBase( "Wheel", TechnologyKind.Research ) {
	public override float GetDiscoveryChance( PlayerComponent player ) {
		throw new NotImplementedException();
	}

	public override float GetDiscoveryProgression( PlayerComponent player ) {
		return 1f;
	}

	public override bool HasPrerequisites( PlayerComponent player ) {
		return true;
	}
}
public sealed class Geometrics() : TechnologyBase( "Geometrics", TechnologyKind.Research ) {
	public override bool HasPrerequisites( PlayerComponent player ) {
		return player.Technology.HasResearched<Wheel>();
	}
}
//public sealed class Candles() : TechnologyBase( "Candles", TechnologyKind.Research ) {
//}

//public sealed class Agriculture() : TechnologyBase( "Agriculture", TechnologyKind.Research ) {
//}

//public sealed class AnimalHusbandry() : TechnologyBase( "Animal Husbandry", TechnologyKind.Research ) {
//}

//public sealed class Archery() : TechnologyBase( "Archery", TechnologyKind.Research ) {
//}

//public sealed class BronzeWorking() : TechnologyBase( "Bronze Working", TechnologyKind.Research ) {
//}

//public sealed class Calendar() : TechnologyBase( "Calendar", TechnologyKind.Research ) {
//}

//public sealed class Construction() : TechnologyBase( "Construction", TechnologyKind.Research ) {
//}

//public sealed class Currency() : TechnologyBase( "Currency", TechnologyKind.Research ) {
//}

//public sealed class Engineering() : TechnologyBase( "Engineering", TechnologyKind.Research ) {
//}

//public sealed class HorsebackRiding() : TechnologyBase( "Horseback Riding", TechnologyKind.Research ) {
//}

//public sealed class IronWorking() : TechnologyBase( "Iron Working", TechnologyKind.Research ) {
//}

//public sealed class Masonry() : TechnologyBase( "Masonry", TechnologyKind.Research ) {
//}

//public sealed class Mathematics() : TechnologyBase( "Mathematics", TechnologyKind.Research ) {
//}

//public sealed class MetalCasting() : TechnologyBase( "Metal Casting", TechnologyKind.Research ) {
//}

//public sealed class Monarchy() : TechnologyBase( "Monarchy", TechnologyKind.Research ) {
//}




