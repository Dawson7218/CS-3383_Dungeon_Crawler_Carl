using UnityEngine;

public class TL3_DynamicBindingShowcase : MonoBehaviour
{
    Item current = new RelicUpgradeItem();
 
    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), current.GetItemId());
        if (GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is RelicUpgradeItem) ? new Item() : new RelicUpgradeItem();
    }
}
