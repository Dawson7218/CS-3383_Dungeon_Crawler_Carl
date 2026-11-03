# Run Point Calculation

## Purpose

`EconomyManager.calculateRunPoints(int enemiesKilled, bool bossKilled, float timeAlive)` returns the points earned during one run. It calculates a score without changing the player's balance, upgrade levels, or save file. The existing `CreditRunEnd` method and `RunEndPayoutTransaction` handle payment and persistence.

Source: `Assets/src/TL4_Getty/EconomyManager.cs`.
Tests: `Assets/tst/TL4_Getty/Getty_Tests/MetaShopEconomyTests.cs`, class `RunPointsTests`.

## Scoring rules

These values were proposed during this interaction and approved by the user:

| Run result | Points |
| --- | ---: |
| Each enemy killed | 10 |
| Boss defeated | 100 bonus |
| Each completed second alive | 1 |

```text
points = enemiesKilled * 10 + (bossKilled ? 100 : 0) + floor(timeAlive)
```

`timeAlive` is measured in seconds. Partial seconds are discarded: 65.9 seconds earns 65 time points. For example, 12 enemies, a defeated boss, and 65.9 seconds alive earns **285 points**.

The boss flag adds its bonus once per calculation. It does not change the enemy count. If the caller includes the boss in `enemiesKilled`, that kill also earns the ordinary 10 points. Use the same counting convention throughout the game.

The three constants at the start of the method control the rewards. Edit them to adjust the scoring rules.

## Declaration and class placement

```csharp
public int calculateRunPoints(int enemiesKilled, bool bossKilled, float timeAlive)
```

The return type changes from the originally proposed `void` to `int`, as approved by the user. Returning the score lets a run summary display the result before the existing payout method saves it. The requested method name is preserved; `CalculateRunPoints` would follow normal C# naming conventions if the team later chooses to rename it.

`EconomyManager` is a practical home for this small economy rule because it already exposes the payout entry point. The transaction continues to apply a final amount; the save data continues to store the balance and levels. If scoring later gains combos, difficulty settings, or multiple modes, a separate `RunPointCalculator` could own those rules.

The repository's current `docs/TL4_Getty/Class-Diagram.png` shows `EconomyManager` and the transaction hierarchy but does not show this method. The older Champion PDF places run scoring outside Feature 5. This requested feature extends that boundary; add the declaration above to `EconomyManager` in the shared diagram if that is now the team's intended design. Neither diagram nor Champion was edited.

## Use

Call this from run-end handling after collecting the final run statistics:

```csharp
EconomyManager economy = EconomyManager.GetInstance();
if (economy != null)
{
    int pointsEarned = economy.calculateRunPoints(12, true, 65.9f);
    // Display pointsEarned in the run summary if needed.
    economy.CreditRunEnd(pointsEarned, true, 65.9f);
    if (!string.IsNullOrEmpty(economy.LastError))
        UnityEngine.Debug.LogWarning(economy.LastError);
}
```

Use the actual run statistics in place of the example values. `CreditRunEnd` accepts the already calculated total; it does not award the boss or survival rewards again. It uses the existing save-before-accepting-state transaction flow. Calling `calculateRunPoints` repeatedly is safe, but the run-end controller must call `CreditRunEnd` only once per completed run because repeated credits would pay again. Automatic death/run-end wiring is outside this addition.

## Input validation and limits

- Negative `enemiesKilled` throws `ArgumentOutOfRangeException` naming `enemiesKilled`.
- Negative, NaN, or infinite `timeAlive` throws `ArgumentOutOfRangeException` naming `timeAlive`.
- Zero kills and zero time are valid. With no boss, the result is zero.
- The calculation uses `double` before multiplication and addition, then caps the total at `int.MaxValue` to prevent integer wraparound.
- That cap applies to the returned score. The existing payout transaction still rejects a credit if the current balance plus the score would exceed `int.MaxValue`; it leaves the balance unchanged and reports the error through `LastError` when called by the manager.

## Verification

`RunPointsTests` covers individual rewards, combined scoring, fractional seconds, invalid inputs, very large inputs, integer-limit boundaries, and applying a calculated score through the existing payout transaction while preserving upgrade levels. Its inactive Unity object prevents `Awake` from loading a player save. It does not call `CreditRunEnd` or write `economy.json`.

Validation result: all **30 tests passed** (18 calculator tests and 12 existing economy tests) in Unity 6000.5.9f1 EditMode. The main repository was open in another Unity instance, so the run used an isolated temporary project containing exact copies of the six economy source files and the test file, with the locally cached Unity Test Framework. This validates the economy code in Unity; it is not a full-project gameplay or run-end integration test.

In Unity, open **Window > General > Test Runner**, select **EditMode**, and run `Getty_Tests` to include both the existing economy tests and the new calculator tests.

## Brief AI interaction log

Date: November 3, 2026. Tool: Codex.

1. The user requested the `calculateRunPoints` feature alongside the completed economy classes, plus Markdown explaining its behavior, use, and AI interaction.
2. Codex inspected the economy code, existing tests, class diagram, and Champion PDF. The repository did not define a scoring formula or contain the requested calculator.
3. Codex proposed 10 points per enemy, a 100-point boss bonus, and 1 point per completed second, with an `int` return value. The user replied: "Use those defaults and return int".
4. Codex added the calculation to `EconomyManager`, input validation, overflow protection, NUnit tests, and this document. Calculation and persistence remain separate operations.

5. Codex ran all 30 economy tests successfully in an isolated Unity project because the main project was already open. No gameplay wiring or actual player-save credit was exercised.
