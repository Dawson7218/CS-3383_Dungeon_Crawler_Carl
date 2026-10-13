using UnityEngine;

public class InWorldItem : MonoBehaviour
{

    private Item item;
    

    // Update is called once per frame
    void Update()
    {
        // this will trigger applying the contained item when player is within this hit box
    }

    public Item Item()
    {
        return item;
    }
}
