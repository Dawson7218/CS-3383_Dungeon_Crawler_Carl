# Loot System


<table>
<tr>
<td width="25%">

★★★★★ (3 reviews)

Author: Kaleb Dillon

Price: $1.99

Version: 1.2

</td>
<td width="50%">

[![LootSytem Demo Video](https://youtu.be/fn6T4-NDFUU)](https://youtu.be/fn6T4-NDFUU)

</td>
</tr>
</table>

### Description
This LootSystem Prefab is a level wide system prefab that can spawn an item anywhere it is told to in 2d space. The items are based on a Base Item prefab that can be changed out and or modified privided it follows a basic format. The system is set to clear any duplicates of itself within a given scene.

### LootSystem Components
- the lootSystem Component
  - holds:
  - a reference to the Base Item Prefab
  - the chance per game room type that an item will spawn
  - the weights for each item to be selected to spawn in based on room type

### Base Item Prefab Component requirements
- In world Item Component
  - holds the code for player pickup and despawn
- a collider set to be a trigger
- a sprite renderer

The item held in the In World Item component gets swapped out by the loot system on spawn, along side the sprite being rendered

### Setup Instructions
Pull the prefab into your scene you need items to spawn in. Pull in your Base Item Prefab that follows the requirements set above. edit the weights and spawn chances in the provided component menu shown in the video. Write a script that calls `LootSytem.Instance.GenerateLootAt(RoomType, Vector2)` to spawn items based on room type at a location you decide.

Note: if you want different items in each weight table you can look at the other tables to know what spawnable item ids exist, or you can look inside `LootTable.prototypes` for a more complete list.

### Requirements
Unity: 6000.5.9f1 or later