# Meta shop

Open `Scenes/MetaShop.unity` and press Play. The scene shows points, three upgrade buttons, and Main Menu.

- `MetaShopUI.cs` draws the basic UI with OnGUI, like the project's other demos. There are no UI prefabs or setup tools.
- `EconomyManager.cs` uses the existing transactions and save data. Its Inspector fields control starting points, upgrade costs, level cap, and each stat's bonus.
- `GetLevel(stat)` and `GetBonus(stat)` expose upgrades for future gameplay integration. `CreditRunEnd(pointsEarned, bossKilled, timeAlive)` adds the run payout.
- Purchases save to `economy.json` in `Application.persistentDataPath`. Managers reload the saved state in each scene.

This is a standalone scene intended for the future player-death flow. Automatic death routing is not connected. When that flow is ready, register MetaShop in Build Settings and load it after death. Main Menu currently loads the existing MainMenu scene; there is no restart/continue button.

All project changes are within TL4_Getty folders. The existing Getty tests cover transactions and dynamic binding.
