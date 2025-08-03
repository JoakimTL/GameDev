# 4× RT GS – Building & Recipe System (v0.5, consolidated)
This document merges every agreed‑upon element from the building brainstorm (up to **July 4 2025**) and keeps the remaining open design questions for future reference.

---
## 1  Building JSON – illustrative stub
```
{
  "id": "quarry",
  "displayName": "Quarry",
  "categories": ["Extraction"],
  "capitalOnly": false,
  "recipeIds": ["manual-quarrying", "mechanised-quarrying"],

  "baseFootprintSqm": 400,
  "upgradeFootprintSqm": {
    "horizontal": 400,
    "vertical": 0
  },
  "maxUpgradeLevels": 5,

  "upgradeCost": {
    "horizontal": { "stone": 50, "timber": 30 },
    "vertical":   { "steel": 20, "concrete": 60 }
  },

  "corpOwnable": true,
  "requiresCorpHQ": true,

  "construction": {
    "stackCost": { "timber": 50, "rope": 20 },
    "durationDays": 120
  },
  "demolitionCostPerSqm": { "labour": 0.05, "currency": 0.2 },

  "salvageOutput": {
    "base": {
      "reinforced-concrete": 30,
      "scrap-steel": 5
    },
    "horizontal": {
      "reinforced-concrete": 10,
      "scrap-steel": 2
    },
    "vertical": {
      "reinforced-concrete": 20,
      "scrap-steel": 8
    }
  },

  "resilience": {
    "wartimeDamageMod": 1.5,
    "decayWithoutUpkeep": 0.01
  },

  "synergyTags": ["Extraction"]
}
```
---
## 2  Recipe JSON – pattern
```
{
  "id": "manual-quarrying",
  "displayName": "Manual Quarrying",
  "labour": { "Labourer": 20 },
  "inputs": {},
  "outputs": { "stone": 6 },
  "unlockTechPrereqs": ["stone-tools"],
  "providesTechProgress": ["architecture"]
}
```
---
## 3  Tile Buildability & Ruggedness
- **buildableArea = baseArea × terrainBuildabilityMod × techMultiplier**
- Terrain guidelines: plains 1.0, hills 0.7, mountains 0.2, swamp 0.5, desert 0.8.
- Engineering techs increment _techMultiplier_ to open more buildable land.
---
## 4  Verticality, Crowding & Exponential Cost
- **CrowdingMultiplier** – `M = 1 / (1 - u)^λ` with `u = usedArea / buildableArea`, `λ = 2` (static).
- Vertical upgrades bypass area usage but pay _upgradeCost.vertical_.
---
## 5  Corporations
### 5.1  Ownership Modes

|Mode|Unlock Condition|Share Liquidity|Typical Era|
|---|---|---|---|
|**Guild** (private)|Default|None – family/partners|Ancient → Medieval|
|**Joint‑Stock Corporation**|**Stock Exchange** building|Publicly traded|Early‑Industrial onward|

_Guilds_ behave like single‑owner entities; _Joint‑Stock Corps_ issue shares, pay dividends, and can be traded by players, AIs, or states.
### 5.2  Corporate HQ Requirement
- Corp owning **≥ 2 different building types** in a Population Centre must erect a standalone _Corporate HQ_ (prefers capital tile).
- Single‑type chains embed a back‑office in one of their sites.
- HQ provides management jobs, unlocks advanced finance UI, and is capturable in war.
### 5.3  Profit & Pricing Logic
- **Operating Profit** = revenue – input‑cost – wages – upkeep.
- **Local Competition** lowers margins when multiple corps supply the same good in one market.
- **Dividends** – payout ratio configurable; retained earnings fund upgrades & expansion.
### 5.4  Stock Price Drivers (see §11)
- **Value Component** – `P_value = EPS × PE_baseline`.
- **Growth Premium** – `+ g × β` where _g_ = QoQ revenue growth.
- **Event Volatility** – wars, laws, scandals add ±σ noise.
### 5.5  Patriotism & Warfare (placeholder)
- Hidden **patriotism/loyalty** metric: high → sabotage; low → compliance after HQ capture.
### 5.6  State Investment & Regulation
- Governments and sovereign funds may hold shares, receive dividends, and vote; policy sliders cap or encourage public ownership.
- Laws can enforce golden shares, anti‑trust break‑ups, or restrict hostile takeovers.
---
## 6  Terrain & Construction Tech Unlocks

|   |   |   |   |
|---|---|---|---|
|Terrain|Base Buildability|Unlock Tech → Multiplier|Other Effects|
|Plains|1.0|—|River adjacency grants irrigation bonus|
|Hills|0.7|Steel Reinforcement +0.2|Earthworks cost ↑; +10 % mining yield|
|Mountains|0.2|Deep Pilings ×2; Geotechnical Anchors ×2|Vertical‑only builds; transport cost ↑|
|Swamp|0.5|Drainage Canals +0.3|High disease risk until Sanitation tech|
|Desert|0.8|Solar Shielding +0.1|Extra heat upkeep (+energy demand)|

---
## 7  Infrastructure & Transport‑Cost System
### 7.1  Edge‑Cost Formula
`EdgeCost = (terrainFactor × baseCost) – infraBonus` (clamped ≥ 0)
- **terrainFactor** – plains 1.0, hills 1.2, mountains 1.5, swamp 1.3, desert 1.1.
- **infraBonus** – summed from active modes on the edge.
- Same‑tile adjacency cost is zero; neighbouring‑tile baseCost ≈ 10 % of far‑field tariff.
- Path‑finding sums EdgeCost over cheapest path; path cache invalidated when infrastructure changes.
### 7.2  Infrastructure Buildings & Edge Bonuses

|   |   |   |   |   |
|---|---|---|---|---|
|Tier|Road|Rail|Waterway|infraBonus|
|0|Trail|—|Dug‑out Canoe|0.05 – 0.10|
|1|Dirt Road|Wooden Rail|Lock Canal|0.20 – 0.30|
|2|Paved Road|Steel Rail|Steam Barge|0.35 – 0.60|
|3|Highway|Mag‑Lev|Container Port|0.50 – 0.90|

---
## 8  Warfare Damage, Repair & Salvage
- **Repair** ≤ original cost; automatic if labour & inputs available.
- **Demolition** – `totalSalvage = (base + horiz×builtHoriz + vert×builtVert) × integrity%`.
- Salvage appears on local market; **Recycling Plants** turn salvage into primary inputs.
---
## 9  Housing Zoning & Rent Model (draft)
- Player zones X km² to a housing _Tier_ (1 → high‑density/low‑quality … 5 → low‑density/high‑quality).
- Governor AI constructs residential mix; **rent = (landCost + buildCost × amortisation) × margin**.
- LandCost responds to scarcity (CrowdingMultiplier), events, and zoning laws.
---
## 10  Stock‑Pricing Model (initial draft)
- `P_value = EPS × PE_baseline` (value).  
    `P_growth = P_value × (1 + g × β)` (growth premium).  
    Market price oscillates around _P_growth_ via AI trading.
---
## 11  AI & Autonomy (prototype placeholder)
- **Layers**: Centre Governor → Corporate AI → National Planner.
- Simple heuristics for prototype; advanced planners and RL considered future work.
---
## 12  Law‑System Footnotes
- Housing zoning & rent‑control.
- Land tax & public‑land leases.
- Corporate charters, ownership caps, hostile‑takeover rules.
- Stock‑market regulation & disclosure.
- Labour laws (wages, safety).
- War conduct (civilians, pillage).
---
## 13  Open Design Questions (v6)
1. **Housing‑Law Mechanics** – zoning UI flow, automated tier mix, rent/tax policy.
2. **Stock‑Pricing Calibration** – PE baseline by tech era; growth β per sector.
3. **Recycling Chain** – salvage → recycle recipes & energy demands.
4. **Corp HQ Balance** – upkeep vs benefits; patriotic–loyalty mechanic, capture penalties.
---

_End of consolidated v0.5 – ready for the_ **_Law Brainstorm_** _phase._