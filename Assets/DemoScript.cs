using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;

    public void PickupItem(int id)
    {
        Debug.Log("Spawning an Item");
        bool result = inventoryManager.AddItem(itemsToPickup[id]);
        if (result==true)
        {
            Debug.Log("Item added");
        } else
        {
            Debug.Log("Item NOT added");
        }
    }

    public void GetSelectedItem()
    {
        Item recievedItem = inventoryManager.GetSelectedItem(false);
        if (recievedItem != null)
        {
            Debug.Log("Item recieved");
        }
        else
        {
            Debug.Log("Can't recieve item");
        }
    }

    public void UseSelectedItem()
    {
        Item recievedItem = inventoryManager.GetSelectedItem(true);
        if (recievedItem != null)
        {
            Debug.Log("Item used");
        }
        else
        {
            Debug.Log("Can't use item");
        }
    }
}
