# AI Interaction Log — Feature 4 (Upgrade & Loot System)

Champion: `Champion_Upgrade_and_Loot_System.docx`  
This log is the full question record referenced by the Champion’s **AI Question Log — Summary**. Numbers in the Champion (stat deltas, drop rates, hour estimates, PERT values) were set from the Feature 4 specification and the sample’s grading notes, not accepted blindly from the model.

---

## Entry 1 — Champion document draft

| | |
|---|---|
| **Date** | 15 September 2026 |
| **Tool** | Cursor agent (chat over the Dungeon Crawler Carl repo) |
| **Source files given to the AI** | `/Users/jonathan/Downloads/Champion Sample Enemy Feature.docx` (structure + diagram types); Feature 4 text from `docs/TL5_KC/Feature Specifications.docx` |
| **Output** | `Champion_Upgrade_and_Loot_System.docx` plus Gane–Sarson / use-case / PERT / Gantt PNGs in `champion_diagrams/` |

### Prompt (verbatim summary)

Use the sample Champion as the structure to create a Champion for Feature 4 — Upgrade & Loot System (TL3). Include the same diagram types as the sample. Required parts:

1. Introduction — what the feature does, and what it does **not** do.
2. Use Case — actors + numbered basic sequence; exceptions branch from a step (`Step 3a`).
3. Interface Contract — inputs / outputs / events, each typed and testable.
4. DFD — Gane–Sarson, context → feature zoom; no black holes or miracles.
5. Timeline — work items + honest hour estimates (TL4 tracks against these).

Feature 4 responsibilities, public interface, internal classes, Prototype + Memento, GRASP, dependencies, testing strategy, completion criteria, and the 10 / 27 / 10 / 5 hour budget were pasted from the team specification.

### What the AI produced

- Five-section Champion in the sample’s Word styling (Cambria headings, Calibri body, navy header tables).
- Two use-case diagrams (Map Generator / Player) with `<<includes>>` and `<<extends>>`.
- Diagram 0 (game context, processes 3 and 4 highlighted), Process 4 zoom, and a 4.2 decision tree.
- Interface tables with typed inputs, outputs, events, and loot-table weights.
- Ten work items totaling 52 hours of work; PERT + Gantt with a 31-hour critical path.

### Substantive checks against the sample’s three failure modes

These are the human-owned edits called out in the Champion summary — not extra AI prose.

1. **Exceptions are branches, not extra stories.** Scenarios use `Step 2a` / `3a` / `4a` / `6a` tied to the basic sequence (failed drop chance, duplicate relic, consumable refresh, memento restore).
2. **DFDs use Gane–Sarson, not flowchart circles.** Processes are numbered rounded rectangles; data stores are `D1` Item Catalog and `D2` Player Loadout. Every process has at least one incoming and one outgoing flow.
3. **Timeline is the Feature 4 budget, not a collapsed “coding only” guess.** Design 10 + Coding 27 + Testing 10 + Documentation 5 = 52 hours of work; calendar time on the critical path is 31 hours (`1 → 3 → 4/5 → 7 → 8 → 9`).

Interface bounds used for TL4 scoring (owned; not invented by the model as “whatever looks fine”):

| Bound | Value |
|---|---|
| Weapon (common / rare) | Attack +5 / +8 |
| Armor (common / rare) | Defense +3, MaxHP +10 / Defense +5, MaxHP +20 |
| Consumable | Attack +8 for 10.0 s; no stack; refresh timer |
| Relic | MoveSpeed +0.15, CritChance +10; duplicate RelicId is a no-op |
| Combat / Corridor drop rates | 0.35 / 0.10 |
| Treasure / Boss drop rates | 1.00 (never NullItem) |

---

## Entry 2 — AI log file and README rename

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Prompt** | In the Champion folder, create an AI interaction log markdown file; convert `README.md.txt` to a real `.md` file. |
| **Output** | This file (`AI_Interaction_Log.md`) and `README.md` (replacing the empty `README.md.txt`). |

No Champion content was regenerated in this entry.

---

## Entry 3 — Full Feature Specifications check

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Source** | `docs/TL5_KC/Feature Specifications.docx` (all seven features) |
| **Prompt** | I am only in charge of Upgrade & Loot. Double-check previous work against the full spec. |
| **Output** | Regenerated Champion: RoomType names aligned with Feature 2; public API limited to the four spec methods; Feature 1/5/6/7 boundaries stated from *their* spec text, not guessed. |

Mismatches found and corrected:

| Issue | Spec | Old Champion | Fix |
|---|---|---|---|
| Public API | Four methods only; no events | Treated four events as public | Events demoted to test-only notifications. Feature 1 keeps Observer on `GameStateManager`. |
| RoomType | Feature 2 rooms: CombatRoom, TreasureRoom, CorridorRoom, BossRoom | Combat, Treasure, Corridor, Boss | Enum and loot tables renamed. |
| HUD | Feature 1: `Refresh(GameState)`, `OnStateChanged` | “Feature 1 reads GetStats()” | GetStats() is Feature 7. HUD updates via Feature 1’s Observer after our mutation. |
| Feature 5 | `TryPurchase` unlocks “reflected in Feature 4/6 content availability” | Only “does not award points” | Still do not implement economy. Unlock gating is not on Feature 4’s public interface; called out as out of scope. |
| Feature 6 | Boss phases / `OnBossDefeated` | Boss only as a drop table | BossRoom is a `RollDrop` key only. |
| Callers of RollDrop | Feature 4 “reads room type from Feature 2” | Actor = “Map Generator” | Actor = Feature 2 `MapManager`; Feature 3/6 may pass the current `RoomType` after a clear. |
| Internal classes | Item + four subtypes + LootTable + ItemMemento | Item types named; LootTable only implied | All seven internal classes listed in the introduction. |

What was already correct (left unchanged):

- Abstract `Item` + virtual `ApplyEffect`; visible no-op if `virtual` is removed.
- GoF Prototype + Memento; GRASP Information Expert + High Cohesion.
- Dependencies: mutate Feature 7 `PlayerStats`; read Feature 2 `RoomType`.
- Headless tests; 10 / 27 / 10 / 5 hour budget; four reversible item types.
- Feature 4 does not own combat, map generation, HUD, or points.

Note: Feature 1’s spec says it “reads player stats (Feature 2)”. Feature 2 is map generation; `GetStats()` lives on Feature 7. The Champion follows Feature 7’s interface, not that numbering slip.

---

## Entry 4 — UML line styles (Pearson)

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Prompt** | Dashed use-case lines were missing arrowheads; then: double-check every UML line for the right head and stroke; then a Pearson legend image (Communicates / Include / Extend / Generalizes). |
| **Output** | Regenerated `uc1_roll_drop.png` and `uc2_apply_item.png`. |

Pearson decisions that shipped:

- **Communicates:** solid line, no arrowheads (Map Manager — Roll Drop; Player — Collect Item).
- **`<< include >>`:** dashed shaft, filled triangular head pointing at the included use case (Roll Drop → Select Weighted Template / Clone Item Prototype; Collect Item → Apply Effect).
- **`<< extend >>`:** dashed shaft, filled head from the exception to the basic use case (Guarantee Required Drop → Roll Drop; Restore Loadout → Collect Item; Revert Consumable → Apply Effect).
- **Generalizes:** unused (no subtype actors or use cases). Labels are singular `<< include >>` / `<< extend >>`.

---

## Entry 5 — Diagram 0 and Diagram 4 arrow repair

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Prompt** | Diagram 0 had arrows that went nowhere or did not make sense; Diagram 4 had the same class of problems. |
| **Output** | Redrawn `dfd0_context.png` and `dfd4_zoom.png` so every flow has a box as source and destination. |

Broken flows that were rejected:

| Diagram | Broken flow | Fix |
|---|---|---|
| 0 (early) | `updated loadout` ended in empty space; `current loadout` missed process 4; HUD labels sat on store arrows | Rerouted so Player → 4 is pickup, D1 → 3 is templates, D2 ↔ 4 is loadout, 5 → Player is HUD |
| 4 (early) | `item / pickup` came from the left edge (miracle); `current stats` stopped between 4.2 and 4.3 | Pickup from Player/sibling; current stats end on 4.2 |

---

## Entry 6 — Diagram 0 = one process per spec feature

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Source** | `docs/TL5_KC/Feature Specifications.docx`; instructor Diagram 0 example (Pong-style numbered processes + open store) |
| **Prompt** | Diagram 0 must contain one box for each feature in the Feature Specifications and any data boxes needed for all features. The provided image is a correct Diagram 0. Follow-up: data stores must look like the example (`D2 \| Level Runtime Data`, open on the right). |
| **Output** | Regenerated Diagram 0 (and Diagram 4 so numbering still matches). Store helper changed to Gane–Sarson open-right. |

Diagram 0 processes (spec numbers, process 4 highlighted):

| # | Feature |
|---|---|
| 1 | HUD, UI & Game State |
| 2 | Room & Map Generation |
| 3 | Enemy AI & Encounter Spawning |
| 4 | Upgrade & Loot |
| 5 | Meta-Progression / Point Economy |
| 6 | Boss Encounter |
| 7 | Player Controller & Combat |

Stores: `D1` Item Catalog, `D2` Player Loadout, `D3` Player Stats, `D4` Dungeon Layout, `D5` Point Balance. Diagram 4 is the explosion of process 4 (`4.1` Roll Drop, `4.2` Apply Effect, `4.3` Revert, `4.4` Snapshot) with sibling processes 2, 3, and 7 as boundary sources/sinks.

---

## Entry 7 — Timeline is Feature 4 only

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Prompt** | The timeline and the Gantt should be for my individual feature, not the whole project; same with the work items. |
| **Output** | Section retitled “Timeline (Feature 4 only)”; work-item table, PERT, and Gantt labeled as Feature 4 tasks. Gantt axis is “Feature 4 hour.” PERT nodes have loot task names so they are not read as Diagram 0 feature numbers. |

Unchanged numbers (still the Feature 4 spec budget, not a seven-feature roll-up): Design 10 + Coding 27 + Testing 10 + Documentation 5 = **52 hours of Feature 4 work**; critical path **31 hours** (`1 → 3 → 4/5 → 7 → 8 → 9`).

---

## Entry 8 — Refresh this log

| | |
|---|---|
| **Date** | 17 September 2026 |
| **Tool** | Same Cursor conversation |
| **Prompt** | Update the AI interaction log. |
| **Output** | This file: Entries 4–8 and questions 18–24 added so the log matches the Champion that actually shipped. |

---

## Numbered question log

Questions asked of the model while drafting, with the decision that went into the Champion.

1. **Q:** Which section order — the sample (Intro, Use Case, DFD, Acceptance Tests, Timeline) or the five parts listed in the assignment (Intro, Use Case, Interface Contract, DFD, Timeline)?  
   **A:** Follow the assignment’s five parts. Keep the sample’s diagram set. Put numeric pass/fail bounds in the Interface Contract instead of a separate Acceptance Tests section.

2. **Q:** How many use cases?  
   **A:** Two, matching the sample. `LD1 Roll Drop for Room` (Feature 2 `MapManager`) and `CI1 Collect Item` (Player).

3. **Q:** Who is the actor for rolling loot — the player or the map?  
   **A:** Feature 2 `MapManager` is the primary actor (`RollDrop` reads `RoomType`). Feature 3 / Feature 6 may call the same method after a clear, passing that room’s type. The player does not choose the table.

4. **Q:** How should exceptions be written?  
   **A:** As `Step Na` branches from the numbered basic sequence, including the sample’s `<<extends>>` cases (required TreasureRoom/BossRoom drop; consumable refresh; loadout restore on death/checkpoint).

5. **Q:** What does the feature explicitly not do?  
   **A:** No combat / `PlayerStats` ownership (Feature 7); no `GenerateRun` / room templates (Feature 2); no `SpawnEncounter` (Feature 3); no HUD Observer (Feature 1); no `TryPurchase` / points (Feature 5); no boss phases (Feature 6).

6. **Q:** Gane–Sarson symbols — circles or rectangles?  
   **A:** Numbered process rectangles with a header band; square external entity (Player); data stores with a left-hand id (`D1`, `D2`). Not UML activity circles.

7. **Q:** Which processes are “this feature” on Diagram 0?  
   **A:** Originally process 3 Roll Loot Drop and process 4 Apply / Restore Loadout. **Superseded by Q19:** one process per spec feature; only process 4 (Upgrade & Loot) is this Champion’s box.

8. **Q:** How do we avoid black holes and miracles?  
   **A:** Every process has ≥1 incoming and ≥1 outgoing flow. `D1` is a read-mostly catalog (allowed). Duplicate-relic rejection is a real output of 4.2 (no-op), not a missing output.

9. **Q:** Prototype and Memento — where do they show up besides the intro?  
   **A:** Use case `LD1` Step 5 is `Item.Clone()`. `CI1` Step 6 / `Step 6a` are `SaveState` / `RestoreState` via `ItemMemento`. Interface Contract tests clone identity and snapshot round-trip.

10. **Q:** Are interface entries allowed to say “the item buffs the player”?  
    **A:** No. Each input/output has a type and a numeric pass/fail bound (see table in Entry 1).

11. **Q:** How is `RollDrop` tested without a scene?  
    **A:** Headless: 2,000 rolls per `RoomType` with a fixed seed; NullItem rates for CombatRoom/CorridorRoom; TreasureRoom/BossRoom never NullItem; returned `ItemId` must be in that room’s table.

12. **Q:** Do work-item hours have to sum to the spec budget?  
    **A:** Yes. Tasks 1–2 = 10 design; 3–8 = 27 coding; 9 = 10 testing; 10 = 5 documentation. Slack is computed, not guessed: task 2 has 10 h, task 6 has 6 h, task 10 has 5 h.

13. **Q:** What is the critical path?  
    **A:** `1 → 3 → 4 → 7 → 8 → 9` (or `1 → 3 → 5 → 7 → 8 → 9`), **31 hours**. Tasks 4 and 5 are both critical because `ApplyItem` cannot start until both upgrade families exist.

14. **Q:** May the AI’s first DFD routing (arrows through the Player entity, pickup into the wrong process) ship as-is?  
    **A:** No. Intermediate redraws fixed dead-end arrows. **Superseded by Q19–Q22:** Diagram 0 is one box per feature with open-right stores; no Player entity on Diagram 0.

15. **Q:** After reading all seven features, did we invent public events Feature 4 does not have?  
    **A:** Yes — the first draft did. Spec public interface is only `ApplyItem`, `RollDrop`, `SaveState`, `RestoreState`. Notifications are test-only. Feature 1 must not subscribe to them.

16. **Q:** Should RoomType be Combat/Treasure or CombatRoom/TreasureRoom?  
    **A:** Match Feature 2: `CombatRoom`, `TreasureRoom`, `CorridorRoom`, `BossRoom`.

17. **Q:** Does Feature 5’s unlock line pull TryPurchase into Feature 4?  
    **A:** No. Feature 4’s own Dependencies line is only Feature 7 stats + Feature 2 room type. Unlock gating is Feature 5’s job and is out of scope for this Champion’s API.

18. **Q:** Do dashed use-case lines need arrowheads?  
    **A:** Yes. Pearson: `<< include >>` dashed with a filled head pointing at the common/included use case; `<< extend >>` dashed with a filled head from the exception to the basic use case. Communicates stays a solid line with no heads.

19. **Q:** May Diagram 0 keep two loot processes (3 and 4) plus a Player entity?  
    **A:** No. After a correct Diagram 0 example was supplied, Diagram 0 must have **one process per Feature Specifications entry** (1 HUD, 2 map, 3 encounters, 4 loot, 5 economy, 6 boss, 7 player). Process 4 is the only highlighted box. The Player external entity was removed; inter-feature flows are process-to-process.

20. **Q:** Which data stores belong on Diagram 0?  
    **A:** Stores the features actually keep: `D1` Item Catalog and `D2` Player Loadout (Feature 4); `D3` Player Stats (Feature 7); `D4` Dungeon Layout (Feature 2); `D5` Point Balance (Feature 5).

21. **Q:** What does a Gane–Sarson data store look like?  
    **A:** Open on the right: top and bottom horizontals, left vertical, ID band (`D2 | Level Runtime Data` style). Not a closed rectangle.

22. **Q:** Are dead-end DFD arrows allowed?  
    **A:** No. Every flow starts on a process, store, or sibling process and ends on one. Diagram 0 and Diagram 4 were redrawn until pickup, current loadout, HUD, and `item / pickup` no longer terminated in empty space.

23. **Q:** What is Process 4 on the zoom after Diagram 0 became one-box-per-feature?  
    **A:** Process 4 is the whole Upgrade & Loot feature. Diagram 4 explodes it as `4.1` Roll Loot Drop, `4.2` Apply Effect (still the primitive with the decision tree), `4.3` Revert Consumable, `4.4` Snapshot Loadout. Sibling processes 2, 3, and 7 appear in white as sources/sinks.

24. **Q:** Are the PERT/Gantt/work items the seven-feature project schedule?  
    **A:** No. They are Feature 4 only: 10 loot tasks totaling Design 10 + Coding 27 + Testing 10 + Documentation 5 = 52 hours of my work, 31 hours on the critical path. Node numbers are those tasks, not Diagram 0 feature IDs. The Gantt axis is “Feature 4 hour,” not “Project hour.”

---

## What was not delegated

- Exact stat deltas, drop rates, and hour estimates (testable bounds; hours from Feature 4’s estimated effort).
- Which feature owns `PlayerStats` (7), `RoomType` (2), HUD (1), points (5), boss phases (6).
- Final yes/no on Gane–Sarson vs. flowchart symbols (course sample).
- That Feature 4’s published methods are only the four listed in the spec.
- That Diagram 0 is one process per spec feature (instructor example), not two loot subprocesses.
- That the timeline is Feature 4’s 52-hour budget, not the seven-feature project calendar.
