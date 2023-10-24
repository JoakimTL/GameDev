using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.World.People;

public sealed class EmotionComponent {

	private readonly Dictionary<EmotionType, Emotion> _emotions;

}

public enum EmotionType {
	JOY,
	BOLDNESS,
	INTEREST,
	TRUST
}

public sealed class Emotion {

}

public sealed class PersonalityTraitComponent {

	private readonly Dictionary<PersonalityTraitType, PersonalityTrait> _personalities;

}

public enum PersonalityTraitType {
	HOTHEADED,
	OPTIMISTIC,
	PESSIMISTIC,
	REALISTIC,
	IDEALISTIC,
	CURIOUS,
	CAUTIOUS,
	CONFIDENT,
	SELF_CONSCIOUS,
	SELFISH,
	SELFLESS,
	EMPATHETIC,
	PRAGMATIC,

}

public sealed class PersonalityTrait {

}

public sealed class StatusEffectComponent {

	private readonly Dictionary<StatusEffectType, StatusEffect> _statusEffects;

}

public enum StatusEffectType {
	DEPRESSION,
	STRESSSED,
}

public sealed class StatusEffect {

}