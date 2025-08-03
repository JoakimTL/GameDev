# Disease & Health – Design Notes

_(Version: Draft 3, 2025‑07‑06)_

---

## 1  Overview

A fully procedural disease framework that hooks into every core pillar of **4× RT‑GS** (Culture, Trade, Population, Technology). Each disease is composed of:

- **Symptoms** (with age‑band modifiers & detectability)
    
- **Transmission Modes** (one or more, copied from data‑driven templates)
    
- **Antibody Code** (128‑bit, mutates → waves & cross‑immunity)
    

The system supports mutation drift, zoonotic spill‑overs, vector ecology, medicine supply chains, policy levers, and climate‑driven range shifts—all without hard‑coding real‑world pathogens.

---

## 2  Data Primitives

|Object|Key fields|
|---|---|
|**AntibodyCode**|`long _value1`, `long _value2`; `Mutate(bits)` flips random bits; Hamming distance → cross‑immunity `e^(–α·d)`|
|**DiseaseSymptom**|`SymptomRef`, `Severity (0‑10)`, optional `Detectability`, optional `AgeProfile{<band>: multipliers}`|
|**TransmissionMode**|Runtime copy of a **TransmissionTemplate** (see §3) + per‑strain tweaks|
|**DiseaseStrain**|`id`, `displayName`, `parent`, `originFace`, `antibodyCode`, `symptoms[]`, `transmissionModes[]`, `LethalityBase`, `IncubationDays`, `InfectiousDays`, `YearDiscovered`|
|**TileEcosystem**|Lives _next to_ `Face`; holds: `VectorPools[]`, `EnvironmentalReservoirs[]`, `AnimalReservoirs[]`|
|**VectorPool**|`vectorTag`, `Density`, `DailyMortality`, `BitingRate`|
|**EnvironmentalReservoir**|`hostResourceId`, `hostQty`, `infectedFraction`, `decayRate`|
|**AnimalReservoir**|`speciesTag`, `Population`, `Prevalence`|

---

## 3  TransmissionTemplate (JSON Schema)

```
{
  "name": "string",                 // required – unique id
  "type": "WaterborneTransmission", // enum: Waterborne | Vector | Airborne | Contact | Soil | ZoonoticSpillover

  "hostResources": ["res_id"],      // 0‑N resource ids (empty ⇒ human‑only)
  "baseEffectiveness": 0.30,         // fallback β when host present OR pure human‑to‑human

  // Detection & crowding
  "severityDetectThreshold": 7,       // ≥7 ⇒ auto‑spotted regardless of tech
  "densityCurve": "logistic",        // default k = 3, x₀ = 1.2 ppl / m²

  // Water‑specific
  "halfLifeDays": 2.5,

  // Vector‑specific
  "vectorTag": "anopheles",
  "eipDays": { "min": 10, "max": 21 },

  // Optional age tweaks
  "ageProfile": { "<5": { "fatalityMult": 1.6 } },

  // Tech counters (per host or global "*")
  "techCounters": {
    "water_fresh": { "Well": 0.6, "SewageTreatment": 0.4 },
    "*":           { "MaskMandate": 0.7 }
  }
}
```

At game start the engine loads every template; the procedural generator then stitches 1‑N of them into a new strain, prunes hosts not present on the seed tile, and nudges numeric fields ±10 % for diversity.

**Example Templates**: `waterborne_basic`, `malaria_vector`, `airborne_generic`, `tick_livestock`, `bat_coronavirus_shift`.

---

## 4  Core Mechanics (Locked‑in)

1. **Climate resolution** – seasonal terrain bands only (no monthly R₀ recalcs).
    
2. **Virulence–transmission trade‑off** – logistic ceiling `MaxCFR = 1 / (1 + e^{a(R₀ − b)})`, default `a = 1.5`, `b = 2.5`.
    
3. **Population density effect** – logistic curve `k = 3`, `x₀ = 1.2 ppl / m²`.
    
4. **Detection rules**
    
    - Symptom severity ≥ 7 ⇒ always noticed.
        
    - Otherwise needs `Tech.Diagnostics.Basic` **or** building `Hospital Lvl 2` (skilled Healers).
        
5. **Host‑resource fallback** – If none of a transmission’s `hostResources` exist on a tile, it reverts to pure human‑to‑human spread at `baseEffectiveness`; spread never hard‑zeros unless Effective R₀ < 1.
    
6. **Trade impact** – V‑shaped demand collapse only if Deaths ≥ 5 % _or_ Quarantine ON.
    
7. **Policy levers** – available once relevant tech unlocked:
    
    - `Quarantine` (city toggle) –20 % Trade, −R₀_local.
        
    - `LimitMarkets` (nation) –10 % Trade, −R₀_city × 0.1.
        
    - `BanPilgrimage` (nation) –Culture Joy, −vector spill‑over.
        
8. **Quarantine vs siege** – harsher supply penalty applies (default cordon –20 %, siege –40 %).
    
9. **Medicine supply chain** –
    
    - `Apothecary` converts _herb_ → _medicine_.
        
    - `Clinic/Hospital` hire Healer vocation & consume `medicine` each tick to cut fatality.
        
10. **Cold‑chain vaccines** – doses/day limited by TradeRoute segments flagged `ColdStorage` (reefer tonnage metric).
    
11. **Bed‑net resistance** – annual +3 % rise in vector β due to insecticide resistance; reset by `Tech.InsecticideGen2`.
    
12. **Migration drain** – City Attractiveness minus (Deaths × 10).
    
13. **Malnutrition debuff** – `GlobalHealthMod = 1 + MalnutritionLevel × 0.25` multiplicative on fatality.
    
14. **Climate‑change vector drift** – every century climate bands slide 0.5° pole‑ward; vector spawn bands update, unlocking mosquito transmission in previously frost‑safe tiles.
    
15. **Extinct strain archiving** – strains with zero infections for 10 years move to the History ledger; archive pruned annually.
    

---

## 5  Defaults & Tunables

|   |   |   |
|---|---|---|
|Dial|Default|Notes|
|Detection severity threshold|**7**|tweak per strain|
|Crowding curve|Logistic k = 3, x₀ = 1.2|caps megacity blow‑ups|
|Bed‑net resistance creep|+3 % β/year|resets via new tech|
|Mutation wave trigger|≥ 3–4 bit flips|new wave unless `CrossImmunity < 0.5`|

---

## 6  Backlog / Footnotes

- Pathogen fame & history‑book chapters
    
- AI opportunistic warfare logic & morality slider
    
- Compound‑crisis generator (famine × disease, volcano ash cooling…)
    
- **Black‑market medicines & contraband trade** (ties to Unrest & Trade pillars)
    
- Beneficial / ecological‑control pathogens (myxomatosis‑style)
    
- Religious miracle‑cure events (Culture pillar)
    
- Trade‑route transport delays for vaccines & other perishables
    
- **Bioweapon sabotage quests** (late tech + espionage brainstorming)
    
- Achievement list (e.g., Iron Constitution, Laissez‑Faire)
    

---

## 7  Next Steps

1. **Play‑test strain generation** with the five template drafts.
    
2. Balance virulence‑ceiling constants `a`, `b` after collecting outbreak telemetry.
    
3. Expand symptom JSON library and hook `AgeProfile` look‑ups.
    
4. Implement techs: `Diagnostics.Basic`, `Diagnostics.Advanced`, `ColdChain.Logistics`, `InsecticideGen2`.
    
5. Prototype Culture miracle‑cure event chain (see backlog).
    
6. Implement black‑market medicine events and tie them into Trade & Unrest systems.
    
7. Add policy‑lever UI for `LimitMarkets` & `BanPilgrimage` once tech unlocked.
    

---

_End of document_