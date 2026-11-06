# Meta shop

Open `Scenes/MetaShop.unity` and press Play. The scene shows points, three upgrade buttons, and Main Menu.

- `MetaShopUI.cs` draws the basic UI with OnGUI, like the project's other demos. There are no UI prefabs or setup tools.
- `EconomyManager.cs` uses the existing transactions and save data. Its Inspector fields control starting points, upgrade costs, level cap, and each stat's bonus.
- `GetLevel(stat)` and `GetBonus(stat)` expose upgrades for future gameplay integration. `CreditRunEnd(pointsEarned, bossKilled, timeAlive)` adds the run payout.
- Purchases save to `economy.json` in `Application.persistentDataPath`. Managers reload the saved state in each scene.

This is a standalone scene intended for the future player-death flow. Automatic death routing is not connected. When that flow is ready, register MetaShop in Build Settings and load it after death. Main Menu currently loads the existing MainMenu scene; there is no restart/continue button.

All project changes are within TL4_Getty folders. The existing Getty tests cover transactions and dynamic binding.

## Orbital console presentation

The shop uses Resources/TL4/OrbitalShopSkin.png as its pixel art console and environment. MetaShopUI draws live labels, balance, level text, original Minecraft icons, and interactive purchase/menu controls over the skin in a 560x315 coordinate space. The complete scene fits the screen with letterboxing at other aspect ratios. The artwork contains no baked-in labels or item icons. Purchase prices and availability still come from EconomyManager. MinecraftArtworkSources.md records the icon sources. There are no level indicator bars.
