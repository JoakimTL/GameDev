Technologies drive a civilization’s evolution, everything from simple farming to cutting-edge microprocessors. In the game, researching tech is a cornerstone mechanic that keeps nation-states in constant strategic flux.
# Progress
Each technology tracks a **Progress** value through three stages, undiscovered, discovered, and researched. As you move through three broad stages, the civilization’s understanding deepens and you unlock practical benefits.

| State        | Progress percentage range           | Gameplay effect                                                                                                                |
| ------------ | ------------------------ | ------------------------------------------------------------------------------------------------------------------------------ |
| Undiscovered | $0\% \rightarrow 25\%$   | Technology remains invisible: no hints in the UI, no ability to fund research.                                                 |
| Discovered   | $25\% \rightarrow 100\%$ | You know it exists. Research funding is now an option, but practical applications remain locked                                |
| Researched   | $100\% \le$              | Unlocks all recipes and bonuses. Further progress represents refinement, adding efficiency bonuses to uses of this technology. |

Each technology has different progress requirements, which represents the technological complexity needed. 
# Mechanics
## Advancing Progress
Gaining progress in a technology can only happen when all prerequisites are discovered, but not necessarily completely researched.
Progressing in technology research happens in two ways: Passive and Active progression. Passive progression is always happening while active progression is intentional.
Progress is tracked in the form of output cycles with research efficiency per output cycle. A researcher provides more progress per output cycle than a farmer, but they both provide progress depending on the number of output cycles they deliver.
### Passive progression
Byproducts of your nation’s ongoing activities nudge every known technology forward:
	- Utilizing any prerequisite.
	- Researching a direct prerequisite.
	- Researching another related technology. A related technology does not necessarily mean a prerequisite.
	- Trading with a nation which has researched the technology.
	- Exchanging culture with another nation.
	- Using materials whose most primitive recipe require this technology.
### Active progression
Active progression comes from intentional funding of the technology research. This means having explicit laboratories set up for discovering new technologies and researching existing ones:
	- Active discovery is unpredictable use of funds at best.
	- Active research however speeds up research after discovery considerably. It must be noted that research is still determined by output cycles from workers, just this time the workers are researchers instead of manufacturing or other service oriented workers.

## Progress Decay
When a technology falls out of use the knowledge of it can decay. This is a very slow and gradual affair, so slow in fact that the player might not even notice until their tech tree suddenly looks slimmer because all the stone age technologies disappeared.
Decay of technology progress can be slowed and even completely stopped by having specialized buildings working on preserving the knowledge. This can be used for museums or something like that? Preserved medieval technologies captivating the minds of modern people?

### Progress Points (PP)
- **Definition**: 1 PP represents the progress delivered by one baseline worker completing a single output cycle. 2 200 PP ≈ 1 person‑year (6 output cycles / day × 365 days).
- **Education curve**

| Years schooled | $\frac{PP}{OC}$ |
| -------------- | --------------- |
| 0              | 0.10            |
| 4              | 0.23            |
| 8              | 0.52            |
| 12+            | 1.20            |
Schooling beyond 12 years does not raise PP further; specialisation bonuses can.   
- **Vocation multipliers** (≥ 1.0): Scholar × 2.0 · Engineer × 1.5 · Artisan × 1.3 · Merchant × 1.1 · Farmer × 1.0 · Labourer × 1.0.
- **Building multipliers**: Basic Workshop × 1.0 · Library × 1.1 · Laboratory × 1.3 · Advanced Research Institute × 1.5.
- **Single‑stack rule**: Each PoP contributes PP from **one** qualifying activity per output cycle—no double‑dipping.
### Output Cycles → PP Formula
where $C(t,f_i)$ is the cycles‑per‑day from the exhaustion curve, _V_ and _B_ are the vocation and building multipliers.
### Progress Decay
- Begins only when **no PoP** supplies PP to the tech.
- Half‑life 60 years → 300 years if at least one Library is active.
- Once global‑communication techs (Printing Press, Internet, …) are researched, decay can be set to **0 PP/year**.
### Tag Hierarchy
| Group           | Example tags                                                       |
| --------------- | ------------------------------------------------------------------ |
| **Science**     | Physics · Chemistry · Biology · Astronomy · Mathematics · Medicine |
| **Engineering** | Metallurgy · Construction · Mechanics · Electricity                |
| **Economics**   | Coinage · Banking · Trade Logistics                                |
| **Military**    | Cavalry · Siegecraft · Ballistics                                  |
| **Culture**     | Music · Literature · Religion                                      |