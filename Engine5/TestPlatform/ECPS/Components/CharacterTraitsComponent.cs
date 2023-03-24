using Engine.GameLogic.ECPS;

namespace TestPlatform.ECPS.Components;
public class CharacterAttributesComponent : ComponentBase {

	private readonly float[] _attributeValues;

	public CharacterAttributesComponent() {
		_attributeValues = new float[ Enum.GetValues<CharacterAttribute>().Length ];
	}

	public float this[ CharacterAttribute skill ] {
		get => _attributeValues[ (int) skill ];
		set => _attributeValues[ (int) skill ] = value;
	}

}
public class CharacterSkillsComponent : ComponentBase {

	private readonly float[] _skillValues;

	public CharacterSkillsComponent() {
		_skillValues = new float[ Enum.GetValues<CharacterSkills>().Length ];
	}

	public float this[ CharacterSkills skill ] {
		get => _skillValues[ (int) skill ];
		set => _skillValues[ (int) skill ] = value;
	}

}

public enum CharacterAttribute {
	Strength,
	Dexterity,
	Constitution,
	Intelligence,
	Wisdom,
	Charisma
}

public enum CharacterSkills {
	[SkillDependency( CharacterAttribute.Strength, 1 )]
	Athletics,
	[SkillDependency( CharacterAttribute.Dexterity, 1 )]
	Stealth,
	[SkillDependency( CharacterAttribute.Dexterity, 1 )]
	SleightOfHand,
	[SkillDependency( CharacterAttribute.Dexterity, 1 )]
	Acrobatics,
	[SkillDependency( CharacterAttribute.Intelligence, 4 )]
	[SkillDependency( CharacterAttribute.Wisdom, 1 )]
	Investigation,
	[SkillDependency( CharacterAttribute.Intelligence, 2 )]
	[SkillDependency( CharacterAttribute.Wisdom, 1 )]
	Religion,
	[SkillDependency( CharacterAttribute.Intelligence, 1 )]
	[SkillDependency( CharacterAttribute.Wisdom, 2 )]
	Perception,
	[SkillDependency( CharacterAttribute.Intelligence, 1 )]
	[SkillDependency( CharacterAttribute.Wisdom, 3 )]
	Survival,
	[SkillDependency( CharacterAttribute.Wisdom, 1 )]
	Medicine,
	[SkillDependency( CharacterAttribute.Charisma, 1 )]
	Deception,
	[SkillDependency( CharacterAttribute.Strength, 1 )]
	[SkillDependency( CharacterAttribute.Charisma, 2 )]
	Intimidation,
	[SkillDependency( CharacterAttribute.Intelligence, 1 )]
	[SkillDependency( CharacterAttribute.Charisma, 4 )]
	Persuasion,
}

[AttributeUsage( AttributeTargets.Field, AllowMultiple = true )]
public sealed class SkillDependencyAttribute : Attribute {
	public readonly CharacterAttribute Attribute;
	public readonly float Weight;

	public SkillDependencyAttribute( CharacterAttribute attribute, float weight ) {
		this.Attribute = attribute;
		this.Weight = weight;
	}
}
