using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryElement : HudElement
{
    [SerializeField] private GameObject itemPrefabBase;
    public override bool Refresh(GameState gameState, GameObject hudObject)
    {
        try
        {
            /*
                     SpriteRenderer renderer = itemspawned.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = Resources.Load<Sprite>("TL3_Kaleb/" + itemToSpawn.getTexture());
             */

            LootSystem lootSystem = LootSystem.Instance;

            var items = lootSystem.Loadout().Items();

            for (int i = 1; i <= 7; i++)
            {
                var itemSprite = hudObject.transform.Find($"Items/Item{i}").GetComponent<Image>();

                try
                {
                    itemSprite.sprite = Resources.Load<Sprite>("TL3_Kaleb/" + items[i-1].getTexture());
                }
                catch
                {
                    itemSprite.sprite = Resources.Load<Sprite>("TL1_Dawson/BlankSprite");
                }
                
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.Log($"Inventory Refresh Failed: " + ex);
            return false;
        }
    }
}
