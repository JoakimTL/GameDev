## 1. Cultural Building Blocks
### 1.1 Cultures & Cores
- Every pop belongs to exactly **one** culture at a time.
- The **core** culture is stored per civilization; local **flavor** deviations exist at city level.
### 1.2 Aspects – the Modular DNA
Each culture is a bundle of _aspects_. An aspect records three continuous properties

|Property|Meaning in Play|Typical Range|
|---|---|---|
|**Saliency**|How important this aspect feels to its followers. Drives unrest & naming.|0.0 – 1.0|
|**Polarity**|The current stance or direction (e.g. _meat‑eating allowed ➜ forbidden_).|‑1.0 – +1.0|
|**Rigidity**|How uncompromising the culture is when pressured on this aspect.|0.0 – 1.0|
> _Example_: In Sunni Islam, **foodstuffs****:pork** and **arts****:iconography** are both high‑saliency and negative‑polarity (forbidden). Pork has _lower rigidity_ (Muslims tolerate outsiders eating it) whereas iconography scores _high rigidity_ (depictions of the Prophet spark outrage).
A culture starts minimalist – a tribal egalitarian bundle with mild gender roles – and grows additional aspects organically.


## 1. Cultural Building Blocks

### 1.1 Cultures & Cores

- Every pop belongs to exactly **one** culture at a time.
    
- The **core** culture is stored per civilisation; local **flavour** deviations exist at city level (see §4).
    

### 1.2 Aspects – the Modular DNA

Each culture is a bundle of _aspects_. An aspect records three continuous properties:

|Property|Meaning in Play|Typical Range|
|---|---|---|
|**Saliency**|How important this aspect feels to its followers. Drives unrest & naming.|0.0 – 1.0|
|**Polarity**|The current stance or direction (e.g. _meat‑eating allowed ➜ forbidden_).|‑1.0 – +1.0|
|**Rigidity**|How uncompromising the culture is when pressured on this aspect.|0.0 – 1.0|

> _Example_: In Sunni Islam, **foodstuffs****:pork** and **arts****:iconography** are both high‑saliency and negative‑polarity (forbidden). Pork has _lower rigidity_ (Muslims tolerate outsiders eating it) whereas iconography scores _high rigidity_ (depictions of the Prophet spark outrage).

A culture starts minimalist – a tribal egalitarian bundle with mild gender roles – and grows additional aspects organically (§3).

---

## 2. Cultural Pressure, Tension & Unrest

### 2.1 Sources of Pressure

- **External trade routes**, migration, propaganda, open borders, neighbouring wars.
    
- **Internal divergence** driven by tech unlocks, environment shifts, class changes (§3).
    

Each _city_ keeps a _positive_ pressure bucket per aspect. Negative contributions only reduce the bucket back toward **0** (pressure never goes below zero).

### 2.2 Translating Pressure → Tension

```
Tension = Pressure × Rigidity
```

Tension is stored per _city × aspect × culture_. High‑rigidity aspects amplify tension sharply.

### 2.3 Unrest & Breaking Point

- A pop faction’s unrest contribution scales with its share of the city’s population.
    
- When one or more aspects in a **cluster** of connected cities push _Tension ≥ BreakThreshold_, they trigger a **Schism Wave** (§5).
    

There are usually **no classic revolts** leading up to the schism – the pressure vents through cultural fission instead.

---

## 3. Organic Internal Drift

Cultures evolve even in isolation, modelled by four _always‑on_ drivers:

1. **Environment** (biome & climate change)
    
2. **Technology unlocks**
    
3. **Economy / class composition**
    
4. **Generational turnover**
    

New aspects _seed_ when triggers fire (e.g. “Printing Press” creates **arts****:mass****‑media**).  
Old or low‑saliency aspects decay toward the baseline via **entropy / cultural vigour** half‑life presets.

---

## 4. Local Flavour vs. Core Identity

- **Flavor Δ** records the local deviation vector in each city.
    
- If ≥ 55 % of the parent culture’s pops adopt the same deviation, it _promotes_ into the core bundle.
    
- A single extremely rigid conflict (high Δ × high rigidity) can ignite a **micro‑schism** limited to one city.
    

Only _core cultures_ are tracked in the diplomacy & opinion systems to keep ledgers lean.

---

## 5. Schisms & Cascade Waves

### 5.1 Quarterly Check Loop

Every quarter the game:

1. Scans all cities for **hot aspects** (Tension ≥ BreakThreshold).
    
2. Bundles contiguous cities that share an identical exploding‑set.
    
3. Fires **one** Schism Wave per bundle:
    
    - Pops above the _JumpThreshold_ switch to a fresh **child culture**.
        
    - Remaining hot aspects may trigger _later_ waves → cascading splits.
        
4. Applies a **Wave Cool‑down** to avoid instant re‑checks.
    

### 5.2 Aftermath

- Both parent & child receive a negative **Schism‑Hate** opinion event (magnitude ∝ saliency).
    
- Diplomacy ledger stores _memory deltas_ which self‑delete once the opinion re‑aligns.
    

---

## 6. Opinion‑of‑Culture System

A hybrid three‑part ledger:

1. **Constant equilibrium** ➜ tree distance, live trade, ongoing wars.
    
2. **Event stack** ➜ war declarations, peace treaties, propaganda, schism hate.
    
3. **Monthly pressure bucket** ➜ player actions (infiltration, festivals, expos).
    

Aspects store only _delta_ memory; data cleans itself when numbers converge.

---

## 7. Naming Conventions

### 7.1 Tiers

1. **Literal Badge** – single‑issue sects (e.g. _Iconoclast Coastlanders_).
    
2. **Identity Template** – multi‑aspect splits (e.g. _Orthodox Low‑Tide Church_).
    
3. **Autonym** – when the child reaches ≥ 40 % of the parent’s pops _and_ spans multiple regions (e.g. _Astraic Communion_, _Novanism_).
    

### 7.2 Autonym Generator (C# Sketch)

```
string MakeName(Culture parent, HashSet<string> taken)
{
    var syllables = parent.LanguageFamily.SyllableBank;
    var root = PickRoot(syllables, 2, 3);
    var suffix = PickSuffix(parent.Theme); // -ism, -ite, Church, etc.
    var name = root + suffix;
    while(taken.Contains(name))
        name = root + Mutate(suffix);
    taken.Add(name);
    return name;
}
```

---

## 8. Player Tools & Designer Knobs

### 8.1 Active Tools

- **Codify Tradition** – boosts rigidity on selected aspects.
    
- **Festivals** – temporarily accelerate aspect decay.
    
- **Propaganda / Improve Relations** – injects opinion events or pressure.
    
- **Cultural Expo / Showcase** – edits Flavor Δ diffusion speed.
    

### 8.2 Balance Scalars

```
BreakThreshold   // tension needed to schism
BundlingTolerance// how similar exploding-sets must be
MinPopShare      // minimum faction size to jump
WaveCooldown     // turns until next wave check
JumpThreshold    // percentage of pops that flip during wave
RigidityCutoff   // below this, tension never stores
HalfLifePresets  // entropy & vigour decay speeds
```

---

## 9. End‑to‑End Flow (Lifecycle Recap)

```
Local Flavor Δ → may promote to Core   ┐
            or clash with rigid Core   │
                     ↓                │
                Tension stores        │
                     ↓                │
  Cluster of cities above threshold   │
                     ↓                │
         Schism Wave (multi‑aspect)   │
                     ↓                │
    Child Culture spawns & is named   │
                     ↓                │
      Opinion ledger updates hatred   │
                     ↓                │
    Drift + Flavor keep world alive  ┘
```