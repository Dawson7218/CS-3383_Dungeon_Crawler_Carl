# Meta shop

Open `Scenes/MetaShop.unity` and press Play. The scene shows points, three upgrade buttons, and Main Menu.

- `MetaShopUI.cs` draws the basic UI with OnGUI, like the project's other demos. There are no UI prefabs or setup tools.
- `EconomyManager.cs` uses the existing transactions and save data. Its Inspector fields control starting points, upgrade costs, level cap, and each stat's bonus.
- `GetLevel(stat)` and `GetBonus(stat)` expose upgrades for future gameplay integration. `CreditRunEnd(pointsEarned, bossKilled, timeAlive)` adds the run payout.
- Purchases save to `economy.json` in `Application.persistentDataPath`. Managers reload the saved state in each scene.

DeathScene already links to MetaShop, and both scenes are registered in Build Settings. Main Menu loads the existing MainMenu scene; there is no restart/continue button.

All project changes are within TL4_Getty folders. The existing Getty tests cover transactions and dynamic binding.

## Orbital console presentation

The shop uses Resources/TL4/OrbitalShopSkin.png as its pixel art console and environment. MetaShopUI draws live labels, balance, level text, original Minecraft icons, and interactive purchase/menu controls over the skin in a 560x315 coordinate space. The complete scene fits the screen with letterboxing at other aspect ratios. The artwork contains no baked-in labels or item icons. Purchase prices and availability still come from EconomyManager. MinecraftArtworkSources.md records the icon sources. There are no level indicator bars.

## Gameplay integration

`MetaRunIntegration` starts automatically; no scene edits or Inspector setup are needed.
In GameScene and MVPScene it applies saved health, attack, and defense bonuses once
and fills the player's upgraded health. It counts enemy defeats, records the boss
bonus, and measures scaled gameplay time (timeScale = 0 pauses the clock).
The boss contributes the 100-point bonus; ordinary enemy defeat events contribute
10 points each. Each completed second contributes one point.

Loading DeathScene calculates and saves the payout automatically. Another team's
run-ending code can instead call `MetaRunIntegration.Instance.EndRun()` before
changing scenes. Repeated completion calls do not duplicate a successful payout.
Boss defeat alone does not end the run: the 10-second countdown belongs to the
run-ending system, which must call EndRun or load DeathScene when it finishes.
Leaving for a menu without completion does not award points.

A failed payout is logged and retained in memory for another save attempt on the
next scene load. It does not survive closing the application. Until it is saved,
the integration does not start tracking another run. Existing saved upgrades and
unspent points still use economy.json. The TL1 health bar's fixed 100 maximum
has not been changed.

Playtest checks:
- Buy each upgrade, start GameScene, and check increased maximum/current HP,
  attack damage, and reduced incoming damage.
- Kill enemies, then die: verify the balance increases by 10 per kill plus whole
  survival seconds. Reload the shop and verify the balance remains saved.
- Defeat the boss, then end the run through EndRun or DeathScene: verify the
  additional 100 points. Repeated EndRun calls must not add another payout.
- Start another run: bonuses apply once, health is full, and kill/time tracking resets.
