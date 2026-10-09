using UnityEngine;

public class LootSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item RollDrop(RoomType roomType)
    {
        switch (roomType)
        {
            default:
                return new RelicUpgradeItem("steak", "steak1", 0.1f, 1);
        }
    }
}
