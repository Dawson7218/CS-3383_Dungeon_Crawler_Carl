using UnityEngine;
using TMPro;

public class ItemSceenCordinator : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnItem()
    {
        int selection = dropdown.value;
        RoomType roomType = RoomType.None;
        switch (selection)
        {
            case 1:
                roomType = RoomType.CombatRoom;
                break;
            case 2:
                roomType = RoomType.TreasureRoom;
                break;
            case 3:
                roomType = RoomType.CorridorRoom;
                break;
            case 4:
                roomType = RoomType.BossRoom;
                break;
            default:
                roomType = RoomType.None;
                break;
        }

        float range = 4;
        float randX = Random.Range(-range,range);
        float randY = Random.Range(-range,range);
        LootSystem.Instance.GenerateLootAt(roomType, new Vector2(randX, randY));
    }
}
