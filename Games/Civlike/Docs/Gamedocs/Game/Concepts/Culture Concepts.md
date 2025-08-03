Cultures are the backbone of the game. They are part of all the dynamic systems in the game and incentivizes behaviors from nations and leaders. They create situations, tension and an organic structure to the events of the game.
Cultures are modular in nature. Hopefully it will be possible to replicate any historical or modern culture using the modular system. The modules in a culture are called aspects and are, as the name implies, aspects of the culture which interact with other parts of the game.
Every nation, when a tribe, starts with a simple egalitarian culture for all it's people in the tribe. There are mild gender roles, but mostly because of the biological nature of the sexes. 
## Aspects
A culture can have many aspects, but some more important than others. Some of the same aspects are turned on the head for some cultures.
In a culture an aspect has a *saliency* value, a *rigidity* value and a *polarity* value. 
	- Saliency is how important this aspect is to the culture.
	- Rigidity is how compromising the culture is to different views on the aspect.
	- Polarity is which way the culture leans on the aspect.
Let's use some real world examples:
Let us use 2 aspects from Islam. While Islam is a religion you could argue, and at least for the sake of the game systems, that it is part of the believer's culture. Muslims can't eat port nor can they depict the prophet Mohammed. These are both highly salient aspects of the religion. Eating pork can be considered a foodstuffs aspect, while not being allowed to depict a historic figure could be an arts aspect. Let's say both aspects are negative in polarity, meaning disallowed foodstuffs and disallowed arts. For Muslims however it doesn't matter that much if others are eating pork, but they do care when other proclaim to be depicting their prophet. This is the difference between low and high rigidity.
## Schisms and Splits
NB: Schisms can happen over multiple aspects at once!
### Cultural Pressure
In a monocultural isolated nation the culture drifts as it adapts to new technology and the environment. Most nations won't be isolated however, and cultural pressures from internal and external sources will cause "unwanted" cultural drift in existing cultures. A trade route might introduce cultural elements which might oppose rigid aspects of the recipient city's cultures. The cultural pressures are simple cultural differences causing shifts in aspects on both cultures. Most cultural pressure is unwanted for both cultures, but most likely if the aspects are non-rigid the effects from the pressure is minimal.

Each city tracks the pressure on each cultural aspect in the city. This pressure is applied to all cultures, but how some cultures react differ based on the rigidity of the culture.

Cultural pressure can never be negative. If negative pressure is applied then it can only zero out any positive pressures.
### Cultural Tension
From unwanted pressure shifting the existing culture comes cultural tension within the culture of a city. Cultural pressure on highly rigid aspects of a culture cause a lot more tension than pressure on a low rigidity aspect. High cultural tension leads to city-wide unrest, and the percentage of population in the city belonging to that culture determines the amount of unrest in the city from the tension. A minority culture experiencing a high amount of tension will probably not cause a large amount of unrest, a majority culture will however cause a lot of unrest.
### Schisms
When a high tension culture reaches a breaking point the culture most likely splits. The split will be quite charged, and can lead to internal strife hurting the city it happens in a lot. There most likely won't be any revolts before the schism, as it will most likely climax as a cultural schism instead of a revolt.
### Resolution
Now that the culture has split there are now two almost identical cultures with a significant difference in one aspect. The schism most likely happened because of a high rigidity aspect was being pressured, and now the city has two sibling cultures where one might hate the other for their transgressions of that specific aspect. Both cultures also now gains a negative schism opinion equilibrium event, the magnitude of which is determined by the saliency on the cultural aspect in question.
## Naming Convention
The first cultures are based on the tribe it originates from. The name may stick through to modern ages, but when schisms happen the new culture needs a name which may need to be generated.


The game starts off with one culture per nation. This monoculturalism doesn't really last that long once nations meet and start exchanging goods and culture. A culture is never static, as the importance of aspects may change as priorities change. Another culture may have completely different views on an aspect, but they seems like nice people so we want to replicate that.

Schisms and splits of cultures occur when saliency is 





$influence = \frac{(followers \cdot \alpha + feats \cdot \beta) \cdot speed}{distance}$

















Cultures are ways of living.

In code a culture can have multiple aspects to them, which are then tied to other aspects of the game. These cultural aspects are predefined in code, but not in their ties to other gameplay aspects. If a culture worships chocolate, then the aspect is worshipping, but the chocolate is a dynamic aspect of the game.

So cultures have cultural aspects, but the way cultures slowly change over time is how strongly an aspect is adhered to in the culture. If you have a holiday during mid summer, but people don't really celebrate it it can be thought to not be a strictly followed aspect of the culture, but still a significant date for people of that culture. Strictness goes from -100% to 100%, where the negative strictness is a way to indicate the cultures aversion to that aspect.

Aspects of a culture in code is derived from a CultureAspectBase class.

Aspects have a Salience and a Polarity value. Salience is how important this aspect is in the culture while Polarity is how the people feel about it. A positive polarity means this aspect is embraced, while a negative polarity means the aspect is shunned.

---
# Unrest-Driven Culture Splitting

A model for emergent culture schisms driven by sustained unrest rather than instantaneous belief shifts.

---

## Data Structures

```csharp
public abstract class CulturalAspectBase {
    public AspectType Type { get; }
    public string TargetKey { get; } //I don't want this to be a string. I want the target to be per derived class in a way that directly links to the thing it's refering to.

    /// 0 (dormant) … 1 (core to identity)
    public float Salience { get; protected set; }

    /// +1 = embraced, –1 = avoided, 0 = neutral
    public int Polarity { get; protected set; }

    /// inertia controls speed of Salience change
    public float Inertia { get; }

    /// last change applied in Update
    public float LastDeltaSalience { get; protected set; }

    protected CulturalAspectBase(AspectType type, string key, float salience, int polarity, float inertia) {
        Type      = type;
        TargetKey = key;
        Salience  = Mathf.Clamp01(salience);
        Polarity  = Math.Sign(polarity);
        Inertia   = inertia;
    }

    public virtual void Update(float pressure) {
        // Compute allowed delta this tick
        float maxDelta = (1f - Salience) / Inertia;
        float delta    = Mathf.Clamp(pressure, -maxDelta, +maxDelta);
        LastDeltaSalience = delta;
        float signed    = Salience * Polarity + delta;
        signed          = Mathf.Clamp(signed, -1f, +1f);
        Polarity        = Math.Sign(signed);
        Salience        = Math.Abs(signed);
    }

    public abstract void ApplyEffects(City city, GameContext ctx);
}

public class Culture {
    public string Name { get; }
    public List<CulturalAspectBase> Aspects { get; } = new();
    public Queue<float> UnrestHistory { get; } = new();  // last N ticks

    /// Total normalized unrest this tick
    public float TotalUnrest { get; set; }
    public float TotalPopulation { get; set; }

    public float SplitUnrestThreshold = 0.3f; // e.g. 30% sustained unrest

    public void TickUpdate(GameContext ctx) {
        TotalUnrest = 0f;

        // 1) Update aspects
        foreach (var a in Aspects) {
            float pressure = ctx.CalculatePressure(a, this);
            a.Update(pressure);

            // 2) Compute tension/unrest
            if (a.Polarity > 0 && a.Salience > 0f) {
                float sentimentGap = Math.Max(0f, pressure - a.Salience);
                float tension      = sentimentGap * a.Salience;
                TotalUnrest       += tension * ctx.GetCulturePopulation(this) * ctx.CulturalToleranceModifier;
            }
        }

        // 3) Record normalized unrest
        float normUnrest = TotalUnrest / TotalPopulation;
        UnrestHistory.Enqueue(normUnrest);
        if (UnrestHistory.Count > 12) UnrestHistory.Dequeue();

        // 4) Check split condition
        float avgUnrest = UnrestHistory.Average();
        if (avgUnrest > SplitUnrestThreshold) {
            var worstAspect = Aspects
                .Where(a => a.Polarity > 0)
                .OrderByDescending(a => (ctx.CalculatePressure(a, this) - a.Salience) * a.Salience)
                .First();
            SplitOnAspect(worstAspect, ctx);
            UnrestHistory.Clear();
        }
    }

    private void SplitOnAspect(CulturalAspectBase aspect, GameContext ctx) {
        float caringFraction = aspect.Salience;
        // 1) Clone and name new culture
        Culture child = DeepClone();
        child.Name += " Faction";

        // 2) Parent drops aspect
        aspect.Salience = 0f;
        aspect.Polarity = 0;

        // 3) Child doubles down
        var childAspect = child.Aspects.First(a => a.Type == aspect.Type && a.TargetKey == aspect.TargetKey);
        childAspect.Salience = 1f;
        childAspect.Polarity = +1;

        // 4) Reallocate population shares
        foreach (var city in ctx.World.CitiesWithCulture(this)) {
            float parentShare = city.CultureShares[this];
            float factionPop  = parentShare * caringFraction;
            city.AdjustCultureShare(this, -factionPop);
            city.AdjustCultureShare(child,  factionPop);
        }

        ctx.World.AddCulture(child);
    }
}
```

# Modular, Emergent Culture System

This note describes a **data-driven**, **emergent** culture model for a 4X RTS game (10000 BC–modern). Cultures consist of predefined _aspects_ whose **Salience** and **Polarity** evolve under game pressures. When sustained unrest builds, a culture may split into factions along the most contentious aspect.

---
## 📚 Terminology
- **AspectType**: Category of cultural facet (e.g. `Religion`, `FoodPreference`, `Holiday`, `GrudgeAgainst`, `MoralValue`).
- **TargetKey**: Unique identifier for an aspect instance (e.g. `MidsummerFestival`, `Olive`, `ChocolateWorship`).
- **Salience** (0 … 1): Median importance of the aspect. 0 = dormant; 1 = core to identity.
- **Polarity** (–1, 0, +1): Attitude toward the aspect. +1 = embraced; –1 = avoided; 0 = neutral.
- **Inertia** (> 0): Resistance to change—higher = slower Salience shifts.
- **Pressure** (–1 … +1): External push (environment, policies, trade, war, events) per tick.
- **Tension**: Unmet Pressure weighted by Salience → unrest generation.
- **UnrestHistory**: Sliding window of normalized unrest levels (e.g. last 12 ticks).
- **SplitUnrestThreshold**: Average unrest > threshold triggers a faction split.
---
## 🗂️ Data Structures

```csharp
public enum AspectType {
    Religion,
    FoodPreference,
    Holiday,
    GrudgeAgainst,
    MoralValue,
    GovernancePreference,
    // …
}

public abstract class CulturalAspectBase {
    public AspectType Type { get; }
    public string TargetKey { get; }
    public float Salience { get; protected set; }    // 0…1
    public int Polarity { get; protected set; }      // –1, 0, +1
    public float Inertia { get; }                    // >0
    public float LastDeltaSalience { get; protected set; }

    protected CulturalAspectBase(
        AspectType type,
        string key,
        float initialSalience,
        int initialPolarity,
        float inertia)
    {
        Type      = type;
        TargetKey = key;
        Salience  = Mathf.Clamp01(initialSalience);
        Polarity  = Math.Sign(initialPolarity);
        Inertia   = inertia;
    }

    public virtual void Update(float pressure) {
        float maxDelta = (1f - Salience) / Inertia;
        float delta    = Mathf.Clamp(pressure, -maxDelta, +maxDelta);
        LastDeltaSalience = delta;

        float signed = Salience * Polarity + delta;
        signed = Mathf.Clamp(signed, -1f, +1f);

        Polarity = Math.Sign(signed);
        Salience = Math.Abs(signed);
    }

    public abstract void ApplyEffects(City city, GameContext ctx);
}

public class Culture {
    public string Name { get; }
    public List<CulturalAspectBase> Aspects { get; } = new();
    public Queue<float> UnrestHistory { get; } = new();  // last N ticks
    public float TotalUnrest { get; set; }
    public float TotalPopulation { get; set; }
    public float SplitUnrestThreshold = 0.3f;

    public void TickUpdate(GameContext ctx) {
        TotalUnrest = 0f;

        // 1) Update aspects & compute unrest
        foreach (var a in Aspects) {
            float pressure = ctx.CalculatePressure(a, this);
            a.Update(pressure);

            if (a.Polarity > 0 && a.Salience > 0f) {
                float gap    = Math.Max(0f, pressure - a.Salience);
                float tension = gap * a.Salience;
                TotalUnrest += tension * ctx.GetCulturePopulation(this) * ctx.CulturalToleranceModifier;
            }
        }

        // 2) Record normalized unrest
        float norm = TotalUnrest / TotalPopulation;
        UnrestHistory.Enqueue(norm);
        if (UnrestHistory.Count > 12) UnrestHistory.Dequeue();

        // 3) Check sustained unrest → split
        float avg = UnrestHistory.Average();
        if (avg > SplitUnrestThreshold) {
            var aspect = Aspects
                .Where(a => a.Polarity > 0)
                .OrderByDescending(a => (ctx.CalculatePressure(a, this) - a.Salience) * a.Salience)
                .First();
            SplitOnAspect(aspect, ctx);
            UnrestHistory.Clear();
        }
    }

    void SplitOnAspect(CulturalAspectBase aspect, GameContext ctx) {
        float fraction = aspect.Salience;
        Culture child = DeepClone();
        child.Name += " Faction";

        // Parent loses aspect
        aspect.Salience = 0f;
        aspect.Polarity = 0;

        // Child doubles down
        var ca = child.Aspects.First(a => a.Type == aspect.Type && a.TargetKey == aspect.TargetKey);
        ca.Salience = 1f;
        ca.Polarity = +1;

        foreach (var city in ctx.World.CitiesWithCulture(this)) {
            float share = city.CultureShares[this];
            float pop   = share * fraction;
            city.AdjustCultureShare(this, -pop);
            city.AdjustCultureShare(child,  pop);
        }

        ctx.World.AddCulture(child);
    }
}
```

---

## 🔄 Pressure & Tension

- **Pressure**: external force per aspect per tick; range –1…+1.
    
    - _Examples:_ olive groves (+food), festival ban (–holiday), war atrocities (+grudge).
        
- **Tension formula**:
    
    ```
    float gap     = Max(0, pressure - Salience);
    float tension = gap * Salience;
    ```
    
    Higher Salience & unmet Pressure → more unrest.
    

---

## ⚔️ Splitting Mechanics

1. **Trigger**: avg(normUnrest) over last N ticks > `SplitUnrestThreshold`.
    
2. **Select** aspect with highest `(pressure - Salience) * Salience`.
    
3. **Split**:
    
    - Clone parent → child faction.
        
    - Parent: `Salience=0`, `Polarity=0`.
        
    - Child: `Salience=1`, `Polarity=+1`.
        
    - Move `Salience` fraction of population shares to child.
        

_Splits model real-world schisms: rare, driven by sustained cultural friction._

---

## ⚙️ Parameters & Tuning

|Parameter|Description|Example Range|
|---|---|---|
|Inertia|Slow vs. fast Salience shifts|`50`–`200`|
|Unrest window length|Number of ticks to average unrest|`6`–`24`|
|SplitUnrestThreshold|Avg unrest to trigger split|`0.3`–`0.5`|
|Pressure magnitude|Scale of environmental/policy inputs|`0.005`–`0.05`|

---

> **Note:** All culture changes are emergent—no leader decrees or scripted events. Culture drifts under pressures; only sustained, unresolved tension births new factions.

# Culture Groups

Cultures are grouped by their ancestry and their likeness. If two cultures are essentially the same, but has wildly different ancestries they will be placed into different groups. Two cultures sharing ancestry will be closer in grouping, but their likeness will determine how closely grouped they are.