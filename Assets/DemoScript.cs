using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;

    public void PickupItem(int id)
    {
        Debug.Log("Spawning an Item");
        inventoryManager.AddItem(itemsToPickup[id]);
    }
}
