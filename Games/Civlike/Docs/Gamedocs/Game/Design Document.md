# 4× RT GS — Unified Game Design Document (v0.1, 28 Jul 2025)
> **Status:** _Draft consolidation of existing pillar‑docs plus newly identified gaps._ **Scope:** All systems up to the end‑game loop covered by the v0.1 concept set.
---
## Table of Contents
1. Vision & Core Pillars
2. World Model & Simulation Framework
3. Pillar Specifications
    - 3.1 Culture
    - 3.2 Trade & Market
    - 3.3 Technology
    - 3.4 Resources & Surveying
    - 3.5 Buildings & Recipes
    - 3.6 Population
    - 3.7 Disease & Health
    - 3.8 Law & Governance
    - 3.9 Warfare
    - 3.10 Diplomacy
    - 3.11 Environment & Climate
    - 3.12 Endgame & Legacy
4. Cross‑Pillar Integration
5. Outstanding Design Work
6. Appendices
---
## 1  Vision & Core Pillars
A globe‑spanning **4× real‑time grand‑strategy** where **Culture** and **Trade** drive history from 10 000 BC through the near future. Secondary systems (Technology, Warfare, etc.) intertwine to amplify or constrain the two primaries. All mechanics are authored in **pure C# (.NET 9)** with strict data‑driven tunables.
### Core Pillars

|Pillar|Owners|Status (2025‑07‑28)|
|---|---|---|
|Culture|Design ❯ v0.1 doc|Functional; awaiting Scholar POP emergence rule & polarity examples|
|Trade & Market|Design ❯ v0.1 doc|Functional; route capacity & Black‑Market risk missing|
|Technology|Design ❯ v0.1 doc|Functional; activity catalogue & decay fail‑states TBD|
|...|...|...|

---
## 2  World Model & Simulation Framework
### 2.1 Map Geometry
- **Icosphere** subdivided 7× ⇒ **327 680 triangular faces** (Using Earths surface area gives ~1 560 km² for each face).
- Each face = _Tile_. Tiles hold **Resources, Buildings, Armies**.
### 2.2 Time & Speed Model
- **Engine tick**: `10 ticks · sec⁻¹` (never changes).
- **Game speed slider** stretches years by multiplying ticks / year (see table).

| $\texttt{Days} \cdot \texttt{Tick}^{-1}$ | $\lfloor \texttt{Days} \cdot \texttt{Year}^{-1} \rfloor \cdot \texttt{Epoch}^{-1}$ | $\texttt{Days} \cdot \texttt{Epoch}^{-1}$ | $\texttt{Ticks} \cdot \texttt{Epoch}^{-1}$ | $\texttt{Seconds}_\texttt{Real} \cdot \texttt{Epoch}^{-1}$ | $\texttt{Hours}_\texttt{Real} \cdot \texttt{Epoch}^{-1}$ | $\texttt{Ticks} \cdot \texttt{Year}^{-1}$ | $\texttt{Years} \cdot \texttt{Epoch}^{-1}$ |
| ---------------------------------------- | ---------------------------------------------------------------------------------- | ----------------------------------------- | ------------------------------------------ | ---------------------------------------------------------- | -------------------------------------------------------- | ----------------------------------------- | ------------------------------------------ |
| 30                                       | 6000                                                                               | 2190000                                   | 73000,00                                   | 7300                                                       | 2,03                                                     | 12,17                                     | 5996,02                                    |
| 15                                       | 3000                                                                               | 1095000                                   | 73000,00                                   | 7300                                                       | 2,03                                                     | 24,35                                     | 2998,01                                    |
| 7,5                                      | 1500                                                                               | 547500                                    | 73000,00                                   | 7300                                                       | 2,03                                                     | 48,70                                     | 1499,01                                    |
| 2                                        | 500                                                                                | 182500                                    | 91250,00                                   | 9125                                                       | 2,53                                                     | 182,62                                    | 499,67                                     |
| 1                                        | 400                                                                                | 146000                                    | 146000,00                                  | 14600                                                      | 4,06                                                     | 365,24                                    | 399,73                                     |
| $\frac{1}{2}$                            | 300                                                                                | 109500                                    | 219000,00                                  | 21900                                                      | 6,08                                                     | 730,48                                    | 299,80                                     |
| $\frac{1}{6}$                            | 150                                                                                | 54750                                     | 328500,00                                  | 32850                                                      | 9,13                                                     | 2191,45                                   | 149,90                                     |
| $\frac{1}{12}$                           | 100                                                                                | 36500                                     | 438000,00                                  | 43800                                                      | 12,17                                                    | 4382,91                                   | 99,93                                      |
| $\frac{1}{24}$                           | 100                                                                                | 36500                                     | 876000,00                                  | 87600                                                      | 24,33                                                    | 8765,81                                   | 99,93                                      |
All systems depend on this tick cadence. If a system is said to trigger weekly, then it looks at this cadence and asks if a week within the world has passed. Sometimes a week passes each tick, other times the time between weeks in real world time is very long. The systems in game should not depend on the amount of time passing per tick, but instead trigger upon time passing regardless of how many ticks that take.

---
## 3  Pillar Specifications
The culture and economy pillars are the main pillars of the game. All other systems are supporting this or helping create a well rounded gameplay experience.
### 3.1 Culture
#### 3.1.1 Philosphy
The culture system is meant to be heavily data-driven with no established cultures present. This means a player should be able to replicate an existing or historical culture, but the culture itself is not present in the game besides the properties of a culture making it emerge. Cultures are supposed to be everchanging, whereby introduction of new foods, technologies, cultures, etc. shape the current cultures. They as a result drift, split and fuse organically. The rulers of nations can't in any peaceful way directly influence a culture, but they can indirectly nudge cultures by utilizing the environment, materials, wars, etc...
#### 3.1.2 Cultural Aspects
All initial, and potential future, tribes start with their own blank slate culture. Over time cultures develop as new ideas rise. From events causing the birth of a religion to people enjoying foodstuffs. Cultures have a set of everchanging aspects defining the culture as a whole. An aspect has 3 values related to it: **Salience**, **Rigidity** and **Polarity**:

| Field          | Range / Type  | Meaning                                        |
| -------------- | ------------- | ---------------------------------------------- |
| **Salience S** | 0‑100 %       | Share of the culture that _values_ an Aspect.  |
| **Rigidity R** | 0‑100 %       | How strongly the culture _defends_ the Aspect. |
| **Polarity P** | ‑100% … +100% | Positive vs negative expression of the Aspect. |
#### 3.1.3 Culture tree
Over time as cultures develop and people expand they may face different challenges and cultures may diverge. Usually cultures retain and recognize a common heritage, such a Germans with wildly different customs but still identifying as a German. This is simulated by placing cultures in a tree structure where the Hamming Distance between leaves on the tree determine how related the cultures are. There are of course other factors such as how similar the cultures are and the physical distance between cultures.

Note: Cultures are usually local to each population center. This means each population center has it's own version of a common culture where they may diverge a little bit, but mostly stay the same if national laws and interest cause them to. This also means the physical distance between cultures are easy to calculate as the physical location of a culture is the capital of a population center.

#### 3.1.x Feats and permanent modifiers
Cultures can achieve **Feats**. Feats are permanent achievements which may affect all cultures in a nation. Feats serve as a way to preserve cultural aspects passively after an initial investment. Later in the game older feats serve as ways to attract tourism and may also serve as a source of influence in cases such as renaissances.
Below is a list of feats:

| Feat             | featBonus | Notes                                                               |
| ---------------- | --------- | ------------------------------------------------------------------- |
| Wonder           | +10 %     | Snapshot promoter; multiple wonders stack multiplicatively (1.10ⁿ). |
| Epic Poem / Saga | +3 %      | Decays 1 % per year once Salience < 20 %.                           |


- **Overview:** Culture represents ideology, artistic output & soft‑power.
- **Key Data:** `CultureAspectId`, `Polarity`, `Scholar` POPs.
- **Gameplay Loop:** Monuments & festivals generate Culture Points → aspects unlock → affect diplomacy, morale, tech.
- **Missing clarifications**
    - Scholar POP emergence rule.
    - Concrete polarity (+1/‑1) gameplay effects & UI chips.
    - Link to Religion building costs (not in Buildings doc).
### 3.2 Economy
- …
- **Missing clarifications**
    - Route capacity algorithm across multi‑edge paths.
    - `RiskPremium%` formula on Black Market trades.
    - Pre‑credit currency conversion rules.
### 3.3 Technology (Technology2507041317)
- …
- **Missing clarifications**
    - Activity catalogue mapping building recipes → **Progress Points**.
    - Worker‑cycle pacing per vocation tier (real‑time anchor).
    - Discovery vs. Research UI milestones.
    - Tech decay fail‑states & UI behaviour.
### 3.4 Resources & Surveying (Resources2507041320)

- …
    
- **Missing clarifications**
    
    - Node exhaustion curve (`sparsityK`).
        
    - Cross‑tile orebody continuity ownership.
        
    - Surveyor labour supply per education tier.
        

### 3.5 Buildings & Recipes (BuildingsAndRecipes2507041609)

- …
    
- **Missing clarifications**
    
    - `SynergyTags` behaviour.
        
    - Justification for crowding exponent λ=2.
        
    - Horizontal vs. vertical upgrade stacking order.
        
    - Demolition cost ownership & wartime overrides.
        

### 3.6 Population (Population2507050216)

- …
    
- **Missing clarifications**
    
    - Health units mapping 0–1 ↔ fatality multipliers.
        
    - Cohort splitting timing under migration/education.
        
    - Conscription age limits.
        

### 3.7 Disease & Health (DiseaseAndHealth2507060316)

- …
    
- **Missing clarifications**
    
    - Hospital quarantine capacity formula.
        
    - Zoonotic host resource list & spill‑over rates.
        
    - Trade demand‑collapse curve post 5 % deaths.
        

### 3.8 Law & Governance (Laws2507042124)

- …
    
- **Missing clarifications**
    
    - Meta‑laws gating `Nullify`/`Supplement` catalogue.
        
    - Unrest pool saturation ceiling & riot/civil‑war triggers.
        
    - Event‑store pruning / savegame repair policy.
        

### 3.9 Warfare (Warfare2507061401)

- …
    
- **Missing clarifications**
    
    - Combat‑tick ↔ sim‑day mapping.
        
    - `MoraleCap` scaling via Culture aspects.
        
    - Supply edge constants **N** & **T**.
        

### 3.10 Diplomacy (Diplomacy2507061942)

- …
    
- **Missing clarifications**
    
    - Trust matrix initialisation baseline.
        
    - Secret clause leak chance equation **FN‑E**.
        
    - Favour token decay rules.
        

### 3.11 Environment & Climate (Environment2507082201)

- …
    
- **Missing clarifications**
    
    - Crop model hook points for agriculture subsystem.
        
    - Sea‑level inundation logic (ownership, infra loss, rerouting).
        
    - Disaster relief / insurance system linkage.
        

### 3.12 Endgame & Legacy (Endgame2507271955)

- …
    
- **Missing clarifications**
    
    - AI reaction protocol to Legacy Provinces.
        
    - Score & victory condition hooks for Legacy Seat.
        
    - Save‑game keys for dynasty vs. culture vs. province ownership.
        

---

## 4  Cross‑Pillar Integration

### 4.1 Global Timing & Scheduling **(TODO)**

|   |   |   |   |
|---|---|---|---|
|Subsystem|Internal step|Conversion to engine ticks|Owner|
|Labour Cycle|_TBD_|_TBD_|Economy|
|Disease daily|_TBD_|_TBD_|Health|
|Climate weekly|_TBD_|_TBD_|Environment|
|Combat|_TBD_|_TBD_|Warfare|

### 4.2 Shared Enumeration Registry **(TODO)**

- `vectorTag`, `SynergyTag`, `CultureAspectId`, `ActivityTag`, …
    
- Proposal: central `Enums.cs` with automated collision tests.
    

### 4.3 UI & Player‑Feedback Hooks

- Every mechanic that changes state must surface a notification → UI backlog.
    
- Each pillar section must add a **UI Sub‑section** in later drafts.
    

### 4.4 Save‑Game, Versioning & Error‑Handling

- Decide: hard‑fail vs. soft‑skip for JSON mismatches.
    
- Version stamps per subsystem payload.
    

---

## 5  Outstanding Design Work (snapshot v0.1)

1. **Fill the Global Timing table (§ 4.1)**.
    
2. Draft the **Shared Enumeration Registry (§ 4.2)**.
    
3. Resolve every bullet in _Missing clarifications_ lists (↑).
    
4. Append UI sub‑sections to each pillar.
    
5. Author balancing rationales where placeholder constants exist.
    

---

## 6  Appendices

### A. Simulation‑Speed Cheat‑Sheet (full)

_(copied from Environment concept doc – keep single source‐of‑truth)_

### B. Source File Index

|   |   |
|---|---|
|Doc|Version (YYMMDDHHmm)|
|Technology|2507041317|
|Resources|2507041320|
|…|…|

### C. Glossary (living)

|   |   |
|---|---|
|Term|Short definition|
|Tile|One face of 7‑subdivided icosphere (~1 560 km²).|
|POP|Cohort of people with shared vocation & age profile.|
|…|…|

---

### Next Revision

- Tag: **v0.2** — integrate resolved clarifications & first UI mock‑ups.