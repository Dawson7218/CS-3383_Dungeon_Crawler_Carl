# Combat room: space support pillars

## Current design

CombatRoomPrefab now replaces the Death Star poster, frame, and indicator with four space-themed support columns. The metal floor, red training area, cyan doorway guides, consoles, and hazard stripes remain. Pillars sit at local positions (+/-5.1, +/-2.2), clear of the four central entrance routes. They are decorative sprites without colliders or interactive behavior. Sorting order 4 keeps them below the current player (5) and items (6).

Only files under TL2_Jordan were changed. No GameScene edits or teammate implementation changes were needed. The poster files and its prior notes remain as unused design history, not as objects in the current combat prefab.

## Source and rights

Game: The Legend of Zelda: Link's Awakening DX (Game Boy Color).
Original game graphics: Nintendo. No license or permission was obtained.
Reference map: Eagle's Tower, assembled/ripped by Revned, hosted by VGMaps.

- Index and attribution: https://www.vgmaps.com/atlas/GB-GBC/index.htm
- Download: https://www.vgmaps.com/atlas/GB-GBC/LegendOfZelda-Link%27sAwakeningDX-Eagle%27sTower.png
- Retrieved: November 5, 2026.
- Local reference: `LinksAwakening-EagleTower-reference.png`, beside this document, outside Unity's Assets directory.
- Selected feature: the long purple stacked pillar near the upper left, approximately x240-255, y112-191 in the 1504x1056 map.
- Adapted sprite: `Assets/src/TL2_Jordan/Art/SpaceSupportColumn.png`.
- Integration: `Assets/prefabs/TL2_Jordan/CombatRoomPrefab.prefab`, four named Support objects beneath CombatDecorations.

The Spriters Resource also lists an Eagle Tower sheet, but its image download was blocked; that sheet was not used as the edit input. Hosting a ripped map does not grant rights to Nintendo's underlying artwork. Credit Revned for assembling the map, not for ownership of the original pillar art. Keeping or sharing the full reference map is a separate use, not automatically justified by an argument about the adapted pillar.

## Transformation and technical integration

The built-in image-editing tool was given the VGMaps image and explicit coordinates identifying the original pillar. It produced an AI-assisted redrawing rather than a deterministic pixel-for-pixel recolor. It retains the tall cap/shaft/base structure while replacing purple stone with graphite and steel plating, cyan conduits, bolts, vents, and amber warning bands. The design is more compact than the source. It is a derivative adaptation, not a claim of independently creating the source design.

The result has a transparent background. Its Unity .meta file imports it as a single 2D sprite with point filtering and no mipmaps. Pixels per unit are calculated from the visible alpha bounds so the visible column is approximately 2.8 world units tall. Four SpriteRenderers reference the same image, so there is one shared texture rather than four image copies. No new runtime scripts are required: DungeonRoomVisualizer already instantiates CombatRoomPrefab.

## Answering the marking key accurately

**What item raises the copyright issue?** The support-column adaptation derived from Nintendo's Link's Awakening DX pillar.

**How could it infringe?** Copying and adapting protected artwork and distributing/displaying it with a game can implicate the owner's reproduction, adaptation, distribution, and display rights. Permission or an applicable exception is needed. If fair use applies, the use is not infringement; the assignment's wording should not be read as a requirement to falsely assert both conclusions at once.

**What was needed to integrate it?** Locate and identify the source, use it as an image-editing input, redesign it for the game's space setting, import the transparent PNG as a Unity sprite, configure its scale/filtering/layering, and reference it from four objects in the existing combat prefab.

## U.S. fair-use argument, including weaknesses

1. **Purpose and character:** The redesign changes the aesthetic expression and integrates it into a student project for study and discussion. These points can be offered in support. However, unlike the previous poster, it does not criticize or parody Zelda. Both versions function as game-environment pillars. Re-theming a fantasy asset as science fiction, changing its colors, or using AI to redraw it does not by itself establish a different legal purpose. This is a significant weakness.
2. **Nature:** The source is published but creative fictional game art. Its creativity weighs against the argument.
3. **Amount and substantiality:** One small environmental design was selected rather than importing an entire tileset or map into the game. However, the relevant work and the expressive importance of the portion must be assessed; using the complete recognizable pillar design may still be substantial. The large map used as an editing input and retained as documentation also needs separate consideration. There is no safe pixel count or percentage.
4. **Market effect:** This student adaptation would not replace playing Link's Awakening. That supports a limited argument, but both assets serve game-scenery functions, and the existence or potential for licensing markets cannot simply be dismissed. Lack of a sale or attribution is not a complete defense.

**Overall:** This is an example for analyzing unlicensed reuse, not a cleared asset or a strong assurance of fair use. Its fair-use argument is weaker than the previous commentary poster because its gameplay/decorative purpose remains similar to the source. The course context is relevant, but not decisive.

## What if the game were sold?

Selling the game would require reassessing the commercial use, distribution, and market context. Commerciality is not an automatic failure of fair use, but the educational rationale would not cover every later use. If an exception did not apply, infringement could lead to an injunction and monetary remedies, subject to statutory requirements. Permission or replacement with independently created art would avoid depending on this uncertain argument. Changing the asset's theme does not automatically clear it for commercial release.

## Authorities

- Exclusive rights and fair use (sections 106 and 107): https://www.copyright.gov/title17/92chap1.html
- Four-factor explanation: https://www.copyright.gov/fair-use/more-info.html
- Infringement and remedies: https://www.copyright.gov/title17/92chap5.html

## Generation prompt and tool

Built-in image-generation/editing tool, with the downloaded VGMaps map supplied as the edit target; no CLI/API fallback.

> Use case: sprite image editing. The supplied image is an edit target, an Eagle's Tower map from Link's Awakening DX. Extract and adapt specifically ONE of the long purple cylindrical pillars near the UPPER LEFT of this map: the first pillar occupies approximately x240..255, y112..191 in the 1504x1056 input. It has a small oval top cap, repeated rounded shaft segments, and a flared base. Do not use a pot, wall, enemy or chest. Transform that pillar into a SPACE-STATION SUPPORT COLUMN for a top-down 2D game. Keep its recognizable tall stacked-column silhouette, flattened oval cap, dark pixel outlines and top-down/slightly front-view projection, but substantially redraw the stonework as dark graphite and steel plating, add two bright cyan vertical power conduits, silver rim at top and base, small amber hazard bands at the base, and tiny bolts. A deliberate pixel-art sprite with approximately 32x64 logical pixels, hard-edged block pixels, limited palette, no antialiasing, no soft gradients or bloom, not a realistic illustration. Make the shaft less repetitive by adding inset metal panel shapes. Show exactly one isolated full column centered, all of its cap and base intact. A compact column with height approximately twice width, not a tall thin rod. Output a square transparent canvas, column centered occupying about 75% of canvas height and 38% width, substantial transparent margins, no background, no floor tile, no lettering, no labels, no extra objects, no cast shadow. This is an edited adaptation of that specific pillar, not just generic original sci-fi art.

The output is stylized pixel-like artwork, not a verified 32x64 pixel-grid sprite. Its final silhouette is taller and slimmer than the requested 2:1 ratio. The final PNG, rather than every requested prompt detail, is the authoritative result.

## Check in Unity

Open CombatRoomPrefab and inspect CombatDecorations. Confirm four column sprites appear, the poster is absent, and all entrances remain clear. During Play, check player/enemy visibility. These are visual props, not physical cover or destructible pillars. Prefab references, alpha transparency, and diff formatting were checked outside Unity; in-editor rendering and gameplay still need confirmation.