# Item Memento

Checkpoint for Feature 4’s equipped loadout. `Loadout` writes the snapshot, `ItemMemento` stores it, and `LootSystem` holds the reference.

## What the code does

A save copies the loadout as it is at that moment. A restore puts that same loadout back. Three classes split the work the way the class diagram does.

| Role | Class | Job |
|---|---|---|
| Originator | `Loadout` | `CreateMemento()` takes the snapshot. `SetMemento()` writes it back. |
| Memento | `ItemMemento` | Lock-box. No public fields or getters. |
| Caretaker | `LootSystem` | `SaveState()` asks for a snapshot and keeps it. `RestoreState()` hands it back. It does not read the lists or the stats. |

`ItemMemento` stores the Champion snapshot:

- `equippedItemIds` — one prototype id per equipped item, in order
- `consumableTimers` — `(consumable id, seconds remaining)` for each consumable, in the same order those consumables were equipped
- `statBaseline` — a copy of the `PlayerStats` object last passed to `ApplyItem`
- `schemaVersion` — `1`

The copy is separate from the live objects. `PlayerStats` and the timer list keep changing after a save (damage, heals, consumable ticks). The checkpoint keeps the numbers from the moment `SaveState` ran.

`CopyFrom` and `RestoreInto` are `internal`. Only `Loadout` calls them. The constructor is `internal` too, so the caretaker does not build an empty box.

## Use

Call this from gameplay code, usually through the `LootSystem` in the scene.

```csharp
ItemMemento checkpoint = lootSystem.SaveState();

lootSystem.ApplyItem(pickedUpItem, playerStats);

// Later, put the loadout and those stats back.
lootSystem.RestoreState(checkpoint);
```

`SaveAState()` is the older name. It calls `SaveState()` and returns the same snapshot.

What you can do with the returned object is hold it and pass it to `RestoreState`. The equipped ids, remaining times, and stat numbers are not on the public surface.

`Awake` binds the scene’s `LootTable` onto the loadout. `RestoreState` binds it again, then tells the loadout to apply the snapshot.

On restore, `Loadout`:

1. Rejects a snapshot whose `schemaVersion` is not `1`, and leaves the current loadout in place.
2. Clears the equipped list and clones each saved id from the loot-table prototypes (`Item.Clone()`).
3. Writes each saved remaining time onto the consumable clones, in order.
4. Rebuilds the live timer list from those clones. A clone gets a new consumable id; the saved id stays inside the memento.
5. Writes `maxHP`, `currentHP`, `moveSpeed`, `attackPower`, `defense`, `critChance`, and `critMulti` onto the same `PlayerStats` instance that `ApplyItem` last used.

Restore does not call `ApplyEffect`. The saved stat numbers already include every item that was equipped at the checkpoint. Calling `ApplyEffect` again would apply those deltas a second time.

`ApplyItem` still equips the item, applies its effect, and records a consumable timer. It also remembers the `PlayerStats` reference so a later restore has somewhere to write the baseline.

The saved ids have to exist on the loot table (`armor_plating`, `grenade_upgrade_1`, `barrel_extension`, and the rest of the prototype list). An id that is not in that list is skipped. Consumable times line up only when every saved consumable id resolves.

## AI interaction log

### Entry 1 — Memento mockup

| | |
|---|---|
| **Date** | 29 October 2026 |
| **Tool** | Cursor agent |
| **Prompt** | ItemMemento needs to follow the Memento GoF pattern. Mock up what that looks like for Kaleb’s feature. |
| **Source** | `docs/TL3_Kaleb` class diagram and AI log; `ItemMemento.cs`, `Loadout.cs`, `LootSystem.cs` |
| **Output** | No code changes. A sequence of Caretaker → Originator → lock-box, and a sketch of `SaveState` / `CreateMemento` / `CopyFrom` / `RestoreInto`. |

The sketch used the diagram’s three roles and the four snapshot fields. It kept `CreateMemento()` and `SetMemento(m)` parameterless, and it deep-copied timers and stats so later ticks would not change the checkpoint.

### Entry 2 — Implement the snapshot

| | |
|---|---|
| **Date** | 3 November 2026 |
| **Tool** | Cursor agent |
| **Prompt** | Implement the memento code. Add a markdown file for what it does, how to use it, and a brief AI interaction log. |
| **Output** | `Assets/src/TL3_Kaleb/ItemMemento.cs`, `Loadout.cs`, `LootSystem.cs`, and this file. |

What changed from the empty stubs:

- `CreateMemento` fills an `ItemMemento` instead of returning a blank one.
- `SetMemento` rebuilds items from prototype ids, restores consumable time, and writes the stat copy back.
- `LootSystem.SaveState` stores whatever `CreateMemento` returns. `RestoreState` passes that object to `SetMemento` and does not read it.
- `SaveAState` stays as a wrapper around `SaveState`.

Checked against the class diagram: the caretaker has no path to the snapshot fields, restore does not reapply item effects, and `schemaVersion` stays 1.

### Entry 3 — Plan the tests

| | |
|---|---|
| **Date** | 4 November 2026 |
| **Tool** | Cursor agent |
| **Prompt** | Plan tests for the item memento functionality. |
| **Source** | This file; `Loadout.cs`, `LootSystem.cs`, `LootTable.cs`; `Kaleb_Tests` (`BindingTests.cs`, `Kaleb_Tests.asmdef`) |
| **Output** | A test plan. No code changes. |

The plan keeps the tests on the public API (`ApplyItem`, `CreateMemento`, `SetMemento`, `SaveState`, `SaveAState`, `RestoreState`). `CopyFrom`, `RestoreInto`, and the `ItemMemento` constructor stay `internal`, with no `InternalsVisibleTo`. `schemaVersion` stays untested, because nothing public can build a snapshot whose version is not 1. The cases are the round trip, a deep copy, two checkpoints, no second `ApplyEffect`, a null restore, an unknown id, the caretaker, an empty save before any `ApplyItem`, and a reflection check that `ItemMemento` has no public snapshot surface.

### Entry 4 — Edit Mode tests

| | |
|---|---|
| **Date** | 4 November 2026 |
| **Tool** | Same Cursor conversation |
| **Prompt** | Implement the Item Memento test plan. |
| **Output** | `Assets/tst/TL3_Kaleb/Kaleb_Tests/ItemMementoTests.cs` |

Nine Edit Mode tests, all passed in the open Unity editor: round trip, snapshot independence, two checkpoints, no reapplied effects, null restore, unknown id, caretaker `SaveState` / `SaveAState`, empty save before `ApplyItem`, and no public snapshot surface. `Kaleb_Tests.asmdef` was unchanged.
