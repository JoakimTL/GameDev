namespace UserTest.AITest;

/// <summary>
/// This is the traits which an agent views another.
/// </summary>
public enum RelationshipTrait {
	//TaskDependency,
	//CriticalDependency,
	//Reliability,
	//Consistency,
	//Trustworthiness,
	//Respect,
	//Compatibility,
	//Communication,
	//ConflictResolution,
	//EmotionalSupport,
	//PhysicalSupport,
	//FinancialSupport,
	//IntellectualSupport,
	//Companionship,
	//Admiration,
	//Attraction,
	Interest,
	//Fairness, //Goes under respect?
	//Generosity, //Goes under goodness?
	ResourceDependency,
	WorkCompatibility,
	SkillComplementarity,
	TaskEfficiency,
	/// <summary>
	/// How trustworthy this agent perceives the other agent to be.
	/// </summary>
	Trustworthiness,
	/// <summary>
	/// How kind this agent perceives the other agent to be.
	/// </summary>
	Kindness,
	/// <summary>
	/// How much respect this agent has for the other agent.
	/// </summary>
	Respect,
	/// <summary>
	/// How compatible this agent perceives the personality of the other agent to be with it's own.
	/// </summary>
	Compatibility,
	/// <summary>
	/// How much authority this agent perceives the other agent to have.
	/// </summary>
	Authority,
	Familiality,
	BloodRelation,
	Recklessness,
	Empathy,
	Wealth
}
