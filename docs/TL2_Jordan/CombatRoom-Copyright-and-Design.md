> Historical design: the Death Star poster has been removed from CombatRoomPrefab and replaced with space-themed pillars. See [CombatRoom-SpacePillars.md](CombatRoom-SpacePillars.md) for the current asset, integration, and copyright analysis.

# Combat room: Reactor Safety parody

## What was reused?

The Death Star image published on the official Star Wars Databank, credited by that site to Lucasfilm. No permission or reuse license was obtained. The image is copyrighted; appearing on a public website does not make it public domain.

- Source page: https://www.starwars.com/databank/Death-Star
- Image URL: https://lumiere-a.akamaihd.net/v1/images/Death-Star-I-copy_36ad2500.jpeg?region=0%2C0%2C1600%2C900
- Retrieved: November 5, 2026.
- Original reference: `DeathStar-reference.jpg` beside this document, for comparison in the course submission. This original is outside Unity's Assets folder and is not included as a game asset. Sharing the reference is a separate use requiring its own justification; this document does not grant permission to redistribute it.
- Adapted texture: `Assets/src/TL2_Jordan/Art/ReactorSafetyPoster.png`.
- Integration: `Assets/prefabs/TL2_Jordan/CombatRoomPrefab.prefab`, under `CombatDecorations/ReactorSafetyParodyPoster`.

## How was it transformed?

The built-in image-generation/editing tool used the downloaded image as an edit target. It produced a cropped, cyan blueprint/halftone interpretation with original warning text, review annotations, and a schematic inset. This is AI-assisted image editing, not a claim that the source was independently drawn or that no protected expression remains. The added text specifically mocks the Death Star's fictional exhaust-port vulnerability: "YOUR EXHAUST PORT SHOULD NOT DOUBLE AS A SELF-DESTRUCT BUTTON."

The large superlaser dish is explicitly distinguished from the small exhaust port. The inset is an illustrative joke, not a canon-accurate engineering drawing. The footer identifies the source and describes the poster as unofficial parody/classroom study. Attribution and that label do not themselves establish fair use.

## What was needed to integrate it with the code?

The texture is imported as one Unity 2D sprite, with bilinear filtering, mipmaps disabled, and 313.5 pixels per world unit for the 1254-pixel image. Its prefab scale is 1.1, giving a 4.4 by 4.4 world-unit display. A SpriteRenderer on the combat-room prefab draws it at sorting order 4, beneath the existing player (5) and items (6). The existing DungeonRoomVisualizer instantiates the decorated prefab for logical CombatRoom objects. No additional runtime script, external dependency, scene edit, or teammate file change was necessary.

The rest of the bay uses editable square sprites: metal floor seams, red training marks, cyan doorway guides, amber hazard stripes, and decorative equipment. Decorations have no colliders. Existing doorway blockers remain inherited from BaseRoom.

## Why could the reuse infringe?

The issue is copying and adapting protected visual expression, and potentially distributing/displaying it in the game, without authorization. Those acts implicate the copyright owner's exclusive rights under 17 U.S.C. section 106. If fair use or another applicable exception covers the particular use, it is not infringement. Thus the marking key is best answered as an example of unlicensed reuse that would need permission or an exception, rather than a definitive admission of unlawful conduct.

## Fair-use argument and its limitations

1. **Purpose and character.** The poster comments on a specific design flaw in the fictional station, converting a dramatic promotional image into a mocking workplace safety review. That supports a transformative/parodic purpose. Classroom analysis also helps. Against that, it still serves as attractive decoration in a game, and a classroom assignment is not automatic permission. An AI style change alone would be a weaker argument than the specific commentary.
2. **Nature.** The source is published, but it is highly creative fictional imagery. Its creative nature weighs against the argument.
3. **Amount and substantiality.** The game uses one small display with a cropped, reinterpreted station rather than a collection of film images. Nevertheless, the Death Star is the main, recognizable expressive subject of the original image. Cropping, reducing resolution, or using a small on-screen area does not automatically mean the amount is legally small. The argument is that this recognizable portion is needed to identify the subject being criticized; that remains contestable.
4. **Market effect.** The commentary poster is unlikely to substitute for watching Star Wars or viewing the source image. However, licensed Star Wars artwork and game imagery have markets, and broader distribution could invite a market-harm argument. The assessment must consider this particular use and whether the claimed harm is legally cognizable; it cannot be settled merely by saying the project is free.

These considerations support a plausible argument, not a guaranteed legal conclusion. They apply to the use as a commentary poster; they do not establish that every reuse of the image, including distributing the unmodified reference, is fair.

## What if the game were sold?

Selling the game would require reassessing the actual commercial distribution and the continued commentary purpose. Commercial use does not automatically defeat fair use, but the classroom context would no longer describe all uses. If a use were found infringing, potential civil consequences include an injunction and monetary remedies, subject to the statutory requirements. For a commercial release, obtaining permission or replacing this poster with wholly original art would avoid relying on this unresolved fair-use argument. This document makes no representation that the asset is cleared for sale.

## Authorities

- Exclusive rights, section 106: https://www.copyright.gov/title17/92chap1.html#106
- Fair use, section 107: https://www.copyright.gov/title17/92chap1.html#107
- Copyright Office explanation: https://www.copyright.gov/fair-use/more-info.html
- Infringement and civil remedies: https://www.copyright.gov/title17/92chap5.html

## Image-editing method and prompt

Tool: built-in image generation/editing tool; no API/CLI fallback. The official downloaded JPEG was supplied as the edit target. The final image was copied into the Unity project; its original generated copy remains outside the project.

Prompt:

> Edit the supplied official Death Star image into a finished, square 1024x1024 science-fiction workplace safety parody poster texture for a top-down Unity student game. Input is the edit target: retain a recognizable cropped Death Star based on the supplied image, including its equatorial trench and large dish, but transform the photograph into cyan blueprint/halftone technical illustration, with amber and red handwritten engineering review marks and arrows. The parody specifically critiques the Death Star's catastrophic exhaust-port/reactor vulnerability. Do NOT mistake the large superlaser dish for the exhaust port: label dish 'SUPERLASER - NOT THE VENT', show a separate schematic inset of a small thermal exhaust shaft leading to a reactor, caption 'ONE TORPEDO. TOTAL LOSS.' Across the top exact very large text 'REACTOR SAFETY'. Main joke in large readable type across lower third exact text 'YOUR EXHAUST PORT SHOULD NOT DOUBLE AS A SELF-DESTRUCT BUTTON.' Small label 'DEATH STAR DESIGN REVIEW'. Small footer 'UNOFFICIAL PARODY / CLASSROOM STUDY' and 'Source image: StarWars.com / Lucasfilm'. Straight-on flat graphic texture edge-to-edge, no mockup, no perspective, no surrounding room, no frame outside canvas. Dark navy blueprint background, clean large readable block typography with strong visual hierarchy, amber hazard bands on top and bottom, cyan diagram lines, red FAIL stamp. Preserve only enough of original subject for recognition, entirely remove photographic starfield. Keep it restrained and readable when displayed small in a room. No logos or claims of affiliation.

The returned image is 1254 by 1254 rather than the requested 1024 by 1024; the sprite import size was set from the actual output. Not every requested graphical detail appears (for example, a FAIL stamp); the inspected final poster is the authoritative artifact.

## Verify in Unity

1. Open CombatRoomPrefab in Prefab Mode and inspect CombatDecorations.
2. Confirm the poster texture imports without missing-sprite errors. Zoom in to read it; at the full-room camera distance its headline will be clearer than its annotations.
3. Run the current game scene and navigate to a generated combat room. Confirm the new appearance, inherited door openings, and player/enemy visibility.
4. The equipment is currently decorative. It does not provide collision, cover, damage, or interactive controls.

Prefab structure, local references, source paths, and whitespace were checked outside Unity. Unity import, rendering, and gameplay still require an in-editor check.