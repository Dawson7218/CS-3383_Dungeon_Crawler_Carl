using UnityEngine;

public class TL3_DynamicBindingShowcase : MonoBehaviour
{
    Item current = new RelicUpgradeItem("relicItem", 0.15f, 10, "coolRelic");
 
    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), current.ApplyEffect());
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is RelicUpgradeItem) ? new Item("baseItem", ItemKind.None) : new RelicUpgradeItem("relicItem", 0.15f, 10, "coolRelic");
    }
}
