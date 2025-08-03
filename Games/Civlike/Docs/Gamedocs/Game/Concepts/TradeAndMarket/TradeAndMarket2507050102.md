# Trade & Market Pillar – v0.1 Concept Spec
_Last updated: 5 Jul 2025_

---
## 1 Purpose & Scope
This document defines every core mechanic, data structure and tuning dial required to simulate trade, markets and currency from 10 000 BC through the credit era. It integrates with Population, Culture, Law, Technology, Environment and Warfare pillars.

---
## 2 Population Containers & Early Exchange

|Object|Head‑count|Key fields|Exchange mode|
|---|---|---|---|
|**BandBuilding**|25–50|`Clans[2–8]`, `commonStock`, `prestige[]`|gift‑economy, depth‑3 barter|
|**ResidencyBlock**|120–400|`Households`, `inventory`, `prestige[]`|communal pool + barter|
|**TradeBuilding**|0|`orderBook`, `inventory`|clears multi‑party swaps once money standardises|
### 2.1 Gift → Prestige → Leadership
- Seasonal surplus donations award **Prestige = surplus / dailyBundle**.
- Top‑quartile Clans form _Elite_ stratum; highest‑prestige Clan is **Chief**.
- Prestige is **spent** on large works, diplomacy, tariffs—no passive decay.
### 2.2 Depth‑3 Barter Loop
Weekly solver closes 3‑way chains among Bands/Blocks inside travel radius, eliminating most unmet wants at negligible CPU cost.

---
## 3 Goods, Prices & Baskets
### 3.1 Data schema additions
- `demandElasticity` (float) – per good, with class defaults.
- `spoilageProfile` – id key into _Spoilage.json_.
### 3.2 Local price pipeline
1. **Cost event** – Building recalculates **ask** when any cost term changes ≥ **2 %** or markup policy toggles.
2. **Market clear** – local price = volume‑weighted mean of asks.
3. **Propagation** – clearing a TradeRoute nudges destination price toward `originPrice + transportCost` if |Δ| ≥ 2 %.
4. **Negative prices** allowed (waste, dumping, toll rebates).
### 3.3 Goods Baskets
_Merge rule_: If all goods in a class differ < **5 %** for **12 price‑ticks**, they merge into a `…Basket`; split when spread widens.

---
## 4 Currency Evolution & Money Supply

|   |   |   |   |
|---|---|---|---|
|Phase|Money creation|Inflation driver|Unlocks|
|**Commodity Pivot**|harvest/mining of the pivot good|bumper crops|depth‑3 barter bias, sacks/strings UI|
|**Coinage**|ruler‑controlled mint/debasement|mint rate – bullion inflow|Tariff laws, TradeBuilding|
|**Credit**|bank loans create deposits|loan growth – real output|Central Bank office, interest‑rate lever|
Hyper‑inflation event: global CPI ≥ +50 % within 12 days.

---
## 5 Trade Routes & Logistics
- `TradeRoute` object holds path edges, capacity (tons/tick), current **RouteController** (null, state, pirate, escort).
- **Raid** → loot 30 % cargo/day. **Toll** → skim 10 %/day. Route controller chosen by stationed unit or treaty.
### 5.1 Transport Tech Ladder

|   |   |   |   |   |
|---|---|---|---|---|
|Tech|Mode|Capacity|Speed ×|Infra prereq|
|Pack Animals|trail|1|1.0|none|
|Wheel & Cart|road|4|1.2|Road edge|
|River Raft|river|6|1.5|River node|
|Coastal Sail|sea|12|1.8|Port|
|Caravansary|caravan|8|1.4|Way‑station|
|Steam Rail|rail|50|4.0|Rail‑track|
|Container Ship|blue‑water|200|5.0|Port + Crane|

---
## 6 Storage & Spoilage
_Every Building_ owns `inventory{ goodId → qty }`. _Seasonal job_ applies:
```
lossRate = GoodsSpoilage[g].baseLoss;
for tag in building.StorageTags:
    lossRate += GoodsSpoilage[g].GetBonus(tag);
stack -= floor(stack * lossRate);
```
- Defaults: grain 20 %/season; **Granary** tag −18 % → net 2 %.
---
## 7 Actors & AI
- **MerchantGuild** – spawns with TradeBuilding; bids with +μ markup, owns caravans (abstracted).
- **Trade League** – diplomatic bloc auto‑offers if ≥5 routes raided continuously.
---
## 8 Policy Levers
- **Tariffs** – slider per goods‑class; advanced per‑good matrix behind “Complex Economics” UI toggle.
- **Embargo** – on/off per nation (+ strategic‑only flag).
- **Interest rate & reserve ratio** – Central Bank office (Credit phase).
---
## 9 Events & Shocks

|   |   |   |
|---|---|---|
|Tag|Trigger|Effect|
|Crop Blight|poor harvest RNG|staple output −X %|
|Piracy Surge|coastal unrest|route risk +Y %|
|Mint Debasement|edict|coin content −Z %|
|Credit Panic|bad‑loan > reserves|loan freeze, rates +100 %|
|Gold Strike|bullion deposit found|CPI −n %, wage lag|

---
## 10 Inflation & CPI
- Per‑settlement **CPI basket**: Staples 40 % / Manufactures 30 % / Services 20 % / Luxuries 10 %.
- Wage offers rise **only** through vacancy competition; no CPI tether.
- Price‑expectations kicker: when CPI ≥ +20 % in 12 days, firms auto‑raise mark‑ups & banks hike rates.
---
## 11 Parameter Reference

|   |   |   |
|---|---|---|
|Code key|Value|Comment|
|`PRICE_DELTA_TRIGGER`|0.02|2 % cost change to refresh ask|
|`ROUTE_LOOT_SHARE`|0.30|cargo value/day|
|`ROUTE_TOLL_SHARE`|0.10|cargo value/day|
|`GRAIN_SPOIL_PROPER`|0.02/season|with Granary tag|
|`GRAIN_SPOIL_LOOSE`|0.20/season|no storage tag|
|`BASKET_SPREAD`|0.05|5 % window|
|`BASKET_TICKS`|12|≈ 12 days early cadence|
|`HYPER_INFLATION_THRESHOLD`|0.50 / 12 days|

---
## 12 Inter‑pillar Hooks (footnotes)
- **Culture** – Hospitality, Taboos modify Trust, route risk; sumptuary laws skew Demand.
- **Law/Gov** – Property tiers unlock private inventories; tariffs & embargoes via edicts.
- **Military** – Escorts reduce route risk; raids raise WarScore.
- **Environment** – Spoilage & waste feed Health/Pollution models.
- **Technology** – Production methods fire cost events; infra upgrades open new transport modes.
---
### End of v0.1 Spec