using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
// This script is attached to an inventory item in a Unity game.

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [Header("UI")]
    public Image image;

    [HideInInspector] public Item item; // This will hold the item data, which should be set in the inspector or through code
    [HideInInspector] public Transform parentAfterDrag; // This will hold the parent transform after dragging

    public void InitialiseItem(Item newItem)
    {
        item = newItem; // Set the item data
        image.sprite = newItem.image;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Logic for when the drag starts
        image.raycastTarget = false; // Disable raycast target to allow dragging through UI
        parentAfterDrag = transform.parent; // Store the original parent
        transform.SetParent(transform.root); // Move the item to the root of the canvas for easier dragging
    }
    public void OnDrag(PointerEventData eventData)
    {
        // Logic for while the item is being dragged
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        // Logic for when the drag ends
        image.raycastTarget = true; // Re-enable raycast target after dragging
        transform.SetParent(parentAfterDrag); // Return the item to its original parent
    }
}

