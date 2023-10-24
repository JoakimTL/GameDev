using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.VoxelCitySim.World.People.PersonalityTraits;

/*
 * Create classes from this list of negative personality traits, exchanging "character" for "NPC":
Abrasive: This NPC is known for their abrasive personality; they tend to irritate or wear down others with their harsh and unpleasant manner.
Aggressive: This NPC exhibits an aggressive nature; they are inclined to be forceful and competitive, often initiating confrontations with others.
Apathetic: This character has an apathetic demeanor; they lack enthusiasm and tend to show indifference or emotional detachment.
Argumentative: This NPC can be quite argumentative; they often engage in disagreements and debates, which can lead to confrontations with other characters.
Boring: Interacting with this character can be a bit dull; they lack excitement and stimulation, making their conversations and actions monotonous.
Cruel: This NPC can be quite cruel; they demonstrate a lack of empathy and are willing to inflict suffering or harm on other characters.
Cynical: This character has a cynical outlook; they tend to distrust the motives and sincerity of others and often have a pessimistic view of human nature.
Cowardly: This character displays cowardly traits; they often show a lack of courage and fearfulness when faced with challenges or danger.
Disloyal: This character can be disloyal; they are unfaithful and lack loyalty to commitments, responsibilities, or relationships.
Disorganized: This NPC tends to be disorganized; they lack order and structure in their actions, often leading to confusion and chaos.
Disrespectful: Interactions with this character can be disrespectful; they often fail to show proper respect or courtesy towards other NPCs, displaying a rude or offensive manner.
Disruptive: This character can be disruptive; they tend to interrupt and disturb, often causing disorder or confusion in interactions with others.
Egotistical: This NPC is known for their egotistical behavior; they have an excessive sense of self-importance and often display narcissistic or self-centered tendencies.
Evasive: Interactions with this character may involve evasive responses; they avoid giving direct answers or are elusive when questioned, often to conceal information or intentions.
Evil: This character is known for their evil nature; they are morally wicked and malevolent, often committing harmful or morally reprehensible acts.
Fanatical: This NPC is excessively enthusiastic and uncompromising in their beliefs, often to an extreme or irrational degree.
Flaky: Interacting with this character can be flaky; they are inconsistent or unreliable, often failing to follow through on commitments or plans.
Forgetful: This character is forgetful; they frequently fail to remember or recall important information or commitments.
Gossipy: Interactions with this character can get gossipy; they tend to spread rumors and engage in idle talk about other NPCs, often without factual basis.
Grumpy: This NPC often appears grumpy; they display irritability and are frequently in a bad mood, complaining and being discontented.
Gullible: This character is quite gullible; they are easily deceived and tend to be overly trusting, often falling for dishonest schemes or manipulation.
Hostile: This NPC is known for their hostile nature; they demonstrate antagonistic or aggressive behavior, often leading to conflicts with other characters.
Humorless: Interactions with this character can be humorless; they lack a sense of humor and are unable to appreciate and enjoy jokes or amusement.
Hypocritical: This character can be quite hypocritical; they engage in behavior that contradicts their stated beliefs or values.
Ignorant: This NPC may come across as ignorant; they lack knowledge, education, or awareness, often resulting in uninformed opinions.
Impatient: This character is impatient by nature; they are easily irritated by delays or a lack of promptness, often showing a lack of tolerance in their interactions.
Inconsiderate: This character tends to be inconsiderate; they fail to show regard or thoughtfulness for the feelings and needs of other NPCs.
Irrational: This character can be quite irrational; they lack logical or reasonable thinking, often being driven by emotions or beliefs that defy reason.
Jealous: This NPC is prone to jealousy; they are envious of other NPCs' possessions, achievements, or relationships, often leading to resentment or insecurity.
Judgmental: This character is quite judgmental; they frequently form critical or negative opinions about other NPCs, often without understanding or empathy.
Lazy: This character often appears lazy; they are inclined to avoid work or exertion and often show a lack of motivation or productivity.
Manipulative: Interactions with this character may involve manipulation; they are skilled at influencing or controlling other NPCs through cunning, deception, or subtle tactics.
Moody: This NPC exhibits moody behavior; they show unpredictable changes in mood and temperament, often without an apparent cause.
Nasty: This character behaves nastily; they are often hostile, unpleasant, or malicious in their interactions with other NPCs.
Nosy: This NPC tends to be nosy; they are prone to being overly curious and intrusive about the personal affairs of other characters.
Obsessive: This character often exhibits obsessive behavior; they engage in persistent, uncontrollable thoughts or behaviors related to a particular subject or idea.
Paranoid: This character is paranoid by nature; they exhibit extreme and irrational distrust or suspicion of other NPCs' motives or intentions.
Pessimistic: This NPC has a pessimistic outlook; they tend to expect the worst and see the negative aspects of situations, often lacking hope or optimism.
Petty: This character can be quite petty; they focus on minor or insignificant details, often displaying a narrow-minded or spiteful attitude.
Possessive: This NPC tends to be possessive; they demonstrate an excessive need to control or own other NPCs or things, often leading to jealousy or clinginess.
Prejudiced: This character is prejudiced; they hold biased or unfair opinions and attitudes towards certain groups or other NPCs.
Pretentious: This NPC often displays pretentious behavior; they exaggerate their sense of importance, often by pretending to be more cultured, educated, or sophisticated than they actually are.
Reckless: This character is known for their reckless behavior; they often act without concern for the potential consequences, taking risks without adequate consideration.
Resentful: This character holds deep-seated feelings of resentment; they frequently experience anger or bitterness towards other NPCs, often due to perceived injustices.
Rotten: This NPC has a rotten character; they are morally corrupt or reprehensible in a severe or extreme way.
Rude: This character can be quite rude; they often lack politeness or consideration for other NPCs, displaying disrespectful behavior.
Selfish: This NPC is selfish by nature; they prioritize their own needs and desires over those of others, often without regard for the well-being of other NPCs.
Sleazy: This character has a sleazy character; they have a disreputable or immoral nature, often associated with questionable or unethical behavior.
Spoiled: This character is often seen as spoiled; they are accustomed to having their desires or wishes indulged, often leading to a sense of entitlement.
Stingy: This NPC tends to be stingy; they are unwilling to spend or share money or resources and often exhibit excessive frugality or miserliness.
Unlucky: This character is frequently associated with bad luck; they often experience adverse events or unfortunate circumstances.
Unmotivated: This character lacks motivation; they often appear unmotivated and uninspired, lacking the drive to pursue goals or tasks.
Unreliable: This NPC is known for their unreliability; they are not dependable or trustworthy, often failing to meet commitments or obligations.
 */

public sealed class Abrasive : PersonalityTraitBase {
	public override string Name => "Abrasive";
	public override string Description => "This NPC is known for their abrasive personality; they tend to irritate or wear down others with their harsh and unpleasant manner.";
}

public sealed class Aggressive : PersonalityTraitBase {
	public override string Name => "Aggressive";
	public override string Description => "This NPC exhibits an aggressive nature; they are inclined to be forceful and competitive, often initiating confrontations with others.";
}

public sealed class Apathetic : PersonalityTraitBase {
	public override string Name => "Apathetic";
	public override string Description => "This NPC has an apathetic demeanor; they lack enthusiasm and tend to show indifference or emotional detachment.";
}

public sealed class Argumentative : PersonalityTraitBase {
	public override string Name => "Argumentative";
	public override string Description => "This NPC can be quite argumentative; they often engage in disagreements and debates, which can lead to confrontations with other characters.";
}

public sealed class Boring : PersonalityTraitBase {
	public override string Name => "Boring";
	public override string Description => "Interacting with this NPC can be a bit dull; they lack excitement and stimulation, making their conversations and actions monotonous.";
}

public sealed class Cruel : PersonalityTraitBase {
	public override string Name => "Cruel";
	public override string Description => "This NPC can be quite cruel; they demonstrate a lack of empathy and are willing to inflict suffering or harm on other characters.";
}

public sealed class Cynical : PersonalityTraitBase {
	public override string Name => "Cynical";
	public override string Description => "This NPC has a cynical outlook; they tend to distrust the motives and sincerity of others and often have a pessimistic view of human nature.";
}

public sealed class CowardlyTrait : PersonalityTraitBase {
	public override string Name => "Cowardly";
	public override string Description => "This NPC displays cowardly traits; they often show a lack of courage and fearfulness when faced with challenges or danger.";
}

public sealed class DisloyalTrait : PersonalityTraitBase {
	public override string Name => "Disloyal";
	public override string Description => "This NPC can be disloyal; they are unfaithful and lack loyalty to commitments, responsibilities, or relationships.";
}

public sealed class DisorganizedTrait : PersonalityTraitBase {
	public override string Name => "Disorganized";
	public override string Description => "This NPC tends to be disorganized; they lack order and structure in their actions, often leading to confusion and chaos.";
}

public sealed class DisrespectfulTrait : PersonalityTraitBase {
	public override string Name => "Disrespectful";
	public override string Description => "Interactions with this NPC can be disrespectful; they often fail to show proper respect or courtesy towards other NPCs, displaying a rude or offensive manner.";
}

public sealed class DisruptiveTrait : PersonalityTraitBase {
	public override string Name => "Disruptive";
	public override string Description => "This NPC can be disruptive; they tend to interrupt and disturb, often causing disorder or confusion in interactions with others.";
}

public sealed class EgotisticalTrait : PersonalityTraitBase {
	public override string Name => "Egotistical";
	public override string Description => "This NPC is known for their egotistical behavior; they have an excessive sense of self-importance and often display narcissistic or self-centered tendencies.";
}

public sealed class EvasiveTrait : PersonalityTraitBase {
	public override string Name => "Evasive";
	public override string Description => "Interactions with this NPC may involve evasive responses; they avoid giving direct answers or are elusive when questioned, often to conceal information or intentions.";
}

public sealed class EvilTrait : PersonalityTraitBase {
	public override string Name => "Evil";
	public override string Description => "This NPC is known for their evil nature; they are morally wicked and malevolent, often committing harmful or morally reprehensible acts.";
}

public sealed class FanaticalTrait : PersonalityTraitBase {
	public override string Name => "Fanatical";
	public override string Description => "This NPC is excessively enthusiastic and uncompromising in their beliefs, often to an extreme or irrational degree.";
}

public sealed class FlakyTrait : PersonalityTraitBase {
	public override string Name => "Flaky";
	public override string Description => "Interacting with this NPC can be flaky; they are inconsistent or unreliable, often failing to follow through on commitments or plans.";
}

public sealed class ForgetfulTrait : PersonalityTraitBase {
	public override string Name => "Forgetful";
	public override string Description => "This NPC is forgetful; they frequently fail to remember or recall important information or commitments.";
}

public sealed class GossipyTrait : PersonalityTraitBase {
	public override string Name => "Gossipy";
	public override string Description => "Interactions with this NPC can get gossipy; they tend to spread rumors and engage in idle talk about other NPCs, often without factual basis.";
}

public sealed class GrumpyTrait : PersonalityTraitBase {
	public override string Name => "Grumpy";
	public override string Description => "This NPC often appears grumpy; they display irritability and are frequently in a bad mood, complaining and being discontented.";
}

public sealed class GullibleTrait : PersonalityTraitBase {
	public override string Name => "Gullible";
	public override string Description => "This NPC is quite gullible; they are easily deceived and tend to be overly trusting, often falling for dishonest schemes or manipulation.";
}

public sealed class HostileTrait : PersonalityTraitBase {
	public override string Name => "Hostile";
	public override string Description => "This NPC is known for their hostile nature; they demonstrate antagonistic or aggressive behavior, often leading to conflicts with other characters.";
}

public sealed class HumorlessTrait : PersonalityTraitBase {
	public override string Name => "Humorless";
	public override string Description => "Interactions with this NPC can be humorless; they lack a sense of humor and are unable to appreciate and enjoy jokes or amusement.";
}

public sealed class HypocriticalTrait : PersonalityTraitBase {
	public override string Name => "Hypocritical";
	public override string Description => "This NPC can be quite hypocritical; they engage in behavior that contradicts their stated beliefs or values.";
}

public sealed class IgnorantTrait : PersonalityTraitBase {
	public override string Name => "Ignorant";
	public override string Description => "This NPC may come across as ignorant; they lack knowledge, education, or awareness, often resulting in uninformed opinions.";
}

public sealed class ImpatientTrait : PersonalityTraitBase {
	public override string Name => "Impatient";
	public override string Description => "This NPC is impatient by nature; they are easily irritated by delays or a lack of promptness, often showing a lack of tolerance in their interactions.";
}

public sealed class InconsiderateTrait : PersonalityTraitBase {
	public override string Name => "Inconsiderate";
	public override string Description => "This NPC tends to be inconsiderate; they fail to show regard or thoughtfulness for the feelings and needs of other NPCs.";
}

public sealed class IrrationalTrait : PersonalityTraitBase {
	public override string Name => "Irrational";
	public override string Description => "This NPC can be quite irrational; they lack logical or reasonable thinking, often being driven by emotions or beliefs that defy reason.";
}

public sealed class JealousTrait : PersonalityTraitBase {
	public override string Name => "Jealous";
	public override string Description => "This NPC is prone to jealousy; they are envious of other NPCs' possessions, achievements, or relationships, often leading to resentment or insecurity.";
}

public sealed class JudgmentalTrait : PersonalityTraitBase {
	public override string Name => "Judgmental";
	public override string Description => "This NPC is quite judgmental; they frequently form critical or negative opinions about other NPCs, often without understanding or empathy.";
}

public sealed class LazyTrait : PersonalityTraitBase {
	public override string Name => "Lazy";
	public override string Description => "This NPC often appears lazy; they are inclined to avoid work or exertion and often show a lack of motivation or productivity.";
}

public sealed class ManipulativeTrait : PersonalityTraitBase {
	public override string Name => "Manipulative";
	public override string Description => "Interactions with this NPC may involve manipulation; they are skilled at influencing or controlling other NPCs through cunning, deception, or subtle tactics.";
}

public sealed class MoodyTrait : PersonalityTraitBase {
	public override string Name => "Moody";
	public override string Description => "This NPC exhibits moody behavior; they show unpredictable changes in mood and temperament, often without an apparent cause.";
}

public sealed class NastyTrait : PersonalityTraitBase {
	public override string Name => "Nasty";
	public override string Description => "This NPC behaves nastily; they are often hostile, unpleasant, or malicious in their interactions with other NPCs.";
}

public sealed class NosyTrait : PersonalityTraitBase {
	public override string Name => "Nosy";
	public override string Description => "This NPC tends to be nosy; they are prone to being overly curious and intrusive about the personal affairs of other characters.";
}

public sealed class ObsessiveTrait : PersonalityTraitBase {
	public override string Name => "Obsessive";
	public override string Description => "This NPC often exhibits obsessive behavior; they engage in persistent, uncontrollable thoughts or behaviors related to a particular subject or idea.";
}

public sealed class ParanoidTrait : PersonalityTraitBase {
	public override string Name => "Paranoid";
	public override string Description => "This NPC is paranoid by nature; they exhibit extreme and irrational distrust or suspicion of other NPCs' motives or intentions.";
}

public sealed class PessimisticTrait : PersonalityTraitBase {
	public override string Name => "Pessimistic";
	public override string Description => "This NPC has a pessimistic outlook; they tend to expect the worst and see the negative aspects of situations, often lacking hope or optimism.";
}

public sealed class PettyTrait : PersonalityTraitBase {
	public override string Name => "Petty";
	public override string Description => "This NPC can be quite petty; they focus on minor or insignificant details, often displaying a narrow-minded or spiteful attitude.";
}

public sealed class PossessiveTrait : PersonalityTraitBase {
	public override string Name => "Possessive";
	public override string Description => "This NPC tends to be possessive; they demonstrate an excessive need to control or own other NPCs or things, often leading to jealousy or clinginess.";
}

public sealed class PrejudicedTrait : PersonalityTraitBase {
	public override string Name => "Prejudiced";
	public override string Description => "This NPC is prejudiced; they hold biased or unfair opinions and attitudes towards certain groups or other NPCs.";
}

public sealed class PretentiousTrait : PersonalityTraitBase {
	public override string Name => "Pretentious";
	public override string Description => "This NPC often displays pretentious behavior; they exaggerate their sense of importance, often by pretending to be more cultured, educated, or sophisticated than they actually are.";
}

public sealed class RecklessTrait : PersonalityTraitBase {
	public override string Name => "Reckless";
	public override string Description => "This NPC is known for their reckless behavior; they often act without concern for the potential consequences, taking risks without adequate consideration.";
}

public sealed class ResentfulTrait : PersonalityTraitBase {
	public override string Name => "Resentful";
	public override string Description => "This NPC holds deep-seated feelings of resentment; they frequently experience anger or bitterness towards other NPCs, often due to perceived injustices.";
}

public sealed class RottenTrait : PersonalityTraitBase {
	public override string Name => "Rotten";
	public override string Description => "This NPC has a rotten character; they are morally corrupt or reprehensible in a severe or extreme way.";
}

public sealed class RudeTrait : PersonalityTraitBase {
	public override string Name => "Rude";
	public override string Description => "This NPC can be quite rude; they often lack politeness or consideration for other NPCs, displaying disrespectful behavior.";
}

public sealed class SelfishTrait : PersonalityTraitBase {
	public override string Name => "Selfish";
	public override string Description => "This NPC is selfish by nature; they prioritize their own needs and desires over those of others, often without regard for the well-being of other NPCs.";
}

public sealed class SleazyTrait : PersonalityTraitBase {
	public override string Name => "Sleazy";
	public override string Description => "This NPC has a sleazy character; they have a disreputable or immoral nature, often associated with questionable or unethical behavior.";
}

public sealed class SpoiledTrait : PersonalityTraitBase {
	public override string Name => "Spoiled";
	public override string Description => "This NPC is often seen as spoiled; they are accustomed to having their desires or wishes indulged, often leading to a sense of entitlement.";
}

public sealed class StingyTrait : PersonalityTraitBase {
	public override string Name => "Stingy";
	public override string Description => "This NPC tends to be stingy; they are unwilling to spend or share money or resources and often exhibit excessive frugality or miserliness.";
}

public sealed class UnluckyTrait : PersonalityTraitBase {
	public override string Name => "Unlucky";
	public override string Description => "This NPC is frequently associated with bad luck; they often experience adverse events or unfortunate circumstances.";
}

public sealed class UnmotivatedTrait : PersonalityTraitBase {
	public override string Name => "Unmotivated";
	public override string Description => "This NPC lacks motivation; they often appear unmotivated and uninspired, lacking the drive to pursue goals or tasks.";
}

public sealed class UnreliableTrait : PersonalityTraitBase {
	public override string Name => "Unreliable";
	public override string Description => "This NPC is known for their unreliability; they are not dependable or trustworthy, often failing to meet commitments or obligations.";
}