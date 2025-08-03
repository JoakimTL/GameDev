# Environment & Climate Pillar – Brainstorm Log (250708‑1425, updated 250708‑1510)

> **Purpose** — Living record of every agreed‑upon design choice for the Environment & Climate pillar so far. Revisit once we draft the formal spec.

---

## 1  Vision & Scope

- **Believable planet‑scale climate** simulated during world‑gen at high fidelity; runtime model updates only when large forcings breach thresholds.
    
- **Natural disasters** (hurricanes, volcanoes, solar storms, etc.) generate tangible strategic risk.
    
- **Climate change** (player‑driven or natural) can alter biomes, sea‑level and disaster frequency.
    
- **Performance mantra** — heavy computation up front; lean during gameplay.
    

---

## 2  Simulation Cadence & Data Hierarchy

|Layer|Resolution|Runtime cadence|Notes|
|---|---|---|---|
|**Face** (single triangle, 1 560 km²)|Static geometry + BaseBiome + StatusFlags|Updated only when its **SuperCell** is marked dirty|All other climate vars live one level up.|
|**SuperCell** (64 faces ≈ 100 000 km²)|Stores weekly means for 6 vars: `Temp`, `Precip`, `PET`, `SoilMoisture`, `SST`, `Albedo`|**Weekly** interpolation cache (52 steps yr⁻¹)|Async solver rebuilds this cache whenever thresholds are exceeded.|
|**Global**|CO₂e ppm, ocean volumes, solar‑cycle index|Continuous|Drives forcing for next solver run.|

> **Why Weekly?** >80 % RAM/CPU saving vs daily while crop‑yield accuracy loss is negligible; hurricane/ tornado micro‑loop handles intra‑week hazards.

---

## 3  Thresholds that Trigger a Climate Re‑solve

|   |   |   |
|---|---|---|
|Signal (since last solve)|Default breach value|Rationale|
|ΔCO₂e|**+5 ppm** (≈ +0.07 °C ECS)|Greenhouse forcing meaningful but not noisy.|
|Ice‑sheet Δvolume|**±1 %** global equivalent (≈ 35 mm SLR)|Albedo + sea‑level feedback.|
|Major volcanic eruption|**VEI ≥ 6**|Stratospheric aerosols & CO₂ pulse.|
|Other|Designer / modder hook|e.g., orbital shade activation|

Async solver starts on breach; its fresh weekly cache swaps in on the next January 1 tick.

---

## 4  Terrain Classification – Hybrid "Biomes + StatusFlags"

### 4.1  BaseBiome table (evaluated at world‑gen & on annual dirty update)

|   |   |   |
|---|---|---|
|ID|Thresholds (all must pass)|Key sources|
|**IceSheet**|Warmest‑month T < 0 °C|Köppen EF|
|**Tundra**|Warmest‑month T 0–10 °C **AND** Annual P < 350 mm|Köppen ET|
|**Alpine**|Elev > 3000 m **OR** lapse‑corrected warmest‑month T < 10 °C|Global alpine climatology|
|**Taiga**|MAT 0–5 °C **AND** P 250–1000 mm|Boreal biome surveys|
|**TemperateDeciduousForest**|MAT 5–15 °C **AND** P 750–1500 mm **AND** ≥4 months T > 10 °C|Köppen Cfa/Cfb, USDA|
|**TemperateGrassland / Steppe**|P 250–500 mm **AND** AI 0.05–0.20|UNEP aridity index|
|**MediterraneanShrubland**|MAT 10–20 °C **AND** P 350–900 mm **AND** summer driest < 30 mm **AND** <⅓ of wettest winter|Köppen Cs|
|**HotDesert**|P < 0.5 × Pₜₕᵣ (Köppen) **OR** AI < 0.05|Köppen BW|
|**ColdDesert**|As above, but MAT < 10 °C||
|**TropicalRainforest**|MAT ≥ 18 °C **AND** P ≥ 1 800 mm **AND** no month < 60 mm|Köppen Af|
|**TropicalSeasonalForest**|MAT ≥ 18 °C **AND** P 1 200–1 800 mm **AND** ≤3 months < 60 mm|Köppen Am|
|**Savanna**|MAT ≥ 18 °C **AND** P 800–1 500 mm **AND** at least 1 month < 60 mm|Köppen Aw|
|**Wetland**|Water‑table ≤ 0 m (≥50 % year saturated)|US EPA|
|**VolcanicPlain**|Inside `HotspotRadius`; climate agnostic|Game design tag|

> **AI = P / PET** (UNEP).  
> **Pₜₕᵣ(Köppen) = 2 × MAT** (±14 or 28 modifier for seasonality).

### 4.2  StatusFlags (byte‑packed, fast‑changing)

`SnowCover`, `Flooded`, `BurnScar`, `Drought`, `AshFall`, `LavaFlow`, … (extensible)

---

## 5  Hotspot & Eruption Model

- **World‑gen** seeds ≈20 `HotspotSource` faces; each keeps an `EruptionClock` with VEI distribution.
    
- Each eruption lays **Basaltic Flow** rings; affected faces multiply existing ore deposit quantities by **1 + veiMod × 0.15–0.40** (caps at VEI 5 for ore boost).
    
- **VEI ≥ 6** events inject aerosols/CO₂ to global forcing; **VEI 8** ultra‑rare (1 × 10⁻⁴ per hotspot per millennium).
    
- Mid‑tech **Magma Tap** building yields geothermal power + rare‑earth slag when adjacent to hotspot.
    

---

## 6  Disaster Catalogue & Mechanics

|   |   |   |   |
|---|---|---|---|
|Type|Spawn rule|Movement|Severity mapping|
|**Hurricane**|SST ≥ 26 °C, shear low, ≥5° lat|Path = prevailing wind + Coriolis; steps daily inside weekly window|Cat 1–7 damage table (TBD)|
|**Tornado Swarm**|Spring CAPE & shear thresholds in Steppe/Savanna cells|Randomised cluster within cell|EF 1–5|
|**Volcano**|`HotspotSource` or Fault VEI draw|Static|VEI 1–8|
|**Earthquake**|Fault stress RNG on high SeismicActivity faces|Static|Mercalli I–XII|
|**Limnic**|Gas‑charged lake + trigger quake/slide|Static flood cloud|casualty burst|
|**Solar Storm**|11‑yr solar RNG + super‑CME tail|Global|Kp 5–9; cripples power grid|
|**Bolide Impact**|Poisson; size ~1 km “city‑killer” to 10 km “playthrough‑ender”|Static crater|Mass extinction if defence fails|

Planetary‑defence success chance set by aggregate funding slider once **Global Defence Accord** ratified via standard diplomacy vote.

---

## 7  Climate‑Field Pruning (runtime state)

|   |   |
|---|---|
|Retained per SuperCell|Purpose|
|Weekly means: `Temp`, `Precip`, `PET`, `SoilMoisture`, `SST`, `Albedo`|Boundary for solver; read by crops & disasters|
|CO₂e ppm|Greenhouse forcing|
|Ocean volumes & basin sea‑level|Flood & coastline shift|
|Ice volume per sheet|Albedo & SLR feedback|
|Hotspot/Fault meta|Disaster RNG seed|

All other world‑gen climate variables are discarded after first solve.

---

## 8  Open To‑Dos & Footnotes

- Benchmark weekly vs daily crop yield once agronomy recipes exist (expect negligible diff).
    
- Finalise Cat/EF/VEI damage tables.
    
- Implement `StatusFlags` interactions (BurnScar lowers albedo, etc.).
    
- Profile memory; if tight, evaluate 10‑day cadence fallback.
    
- Diplomacy overhaul: incorporate Global Defence Accord bargaining logic.
    
- Seasonal snow overlay & river freeze visual pass.
    
- Megaproject ideas list (orbital shades, equatorial scrubber ring, geo‑engineering) — separate brainstorm.
    

---

_(End of log – update 250708‑1510)_