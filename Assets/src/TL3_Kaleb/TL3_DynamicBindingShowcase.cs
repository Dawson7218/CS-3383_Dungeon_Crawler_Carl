using UnityEngine;

public class TL3_DynamicBindingShowcase : MonoBehaviour
{
    Item current = new RelicItem();

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 400, 30), current.ItemId());
        if(GUI.Button(new Rect(20, 60, 160, 30), "Swap"))
            current = (current is RelicItem) ? new Item() : new RelicItem();
    }
}
