using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Inventory, Weapon }
    public SlotType slotType;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (draggedItem != null)
        {
            if (transform.childCount > 0)
            {
                var existingItem = transform.GetChild(0).GetComponent<InventoryItem>();
                HandleItemDrop(draggedItem, existingItem);
            }
            else
            {
                PlaceItemInSlot(draggedItem);
            }
        }
    }

    private void HandleItemDrop(InventoryItem draggedItem, InventoryItem existingItem)
    {
        switch (slotType)
        {
            case SlotType.Inventory:
                SwapItems(draggedItem, existingItem);
                break;

            case SlotType.Weapon:
                HandleWeaponSlot(draggedItem, existingItem);
                break;

            default:
                ReturnItemsToPreviousParents(draggedItem, existingItem);
                break;
        }
    }

    private void HandleWeaponSlot(InventoryItem draggedItem, InventoryItem existingItem)
    {
        var weapon = draggedItem.item as Weapon;
        if (weapon != null)
        {
            EquipWeapon(weapon);
            SwapItems(draggedItem, existingItem);
        }
        else
        {
            ReturnItemsToPreviousParents(draggedItem, existingItem);
        }
    }

    private void EquipWeapon(Weapon weapon)
    {
        if (weapon != null)
        {
            InventoryManager.Instance.EquipWeaponFromInventory(weapon);
        }
    }

    private void SwapItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        Transform originalParent = draggedItem.previousParent;
        Transform destinationParent = existingItem.previousParent;

        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;

        existingItem.transform.SetParent(originalParent);
        existingItem.transform.localPosition = Vector3.zero;

        draggedItem.previousParent = destinationParent;
        existingItem.previousParent = originalParent;
    }

    private void PlaceItemInSlot(InventoryItem draggedItem)
    {
        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;
        draggedItem.previousParent = transform;

        if (slotType == SlotType.Weapon && draggedItem.item is Weapon weapon)
        {
            EquipWeapon(weapon);
        }
    }

    private void ReturnItemsToPreviousParents(InventoryItem draggedItem, InventoryItem existingItem)
    {
        draggedItem.transform.SetParent(draggedItem.previousParent);
        draggedItem.transform.localPosition = Vector3.zero;

        existingItem.transform.SetParent(existingItem.previousParent);
        existingItem.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (slotType == SlotType.Weapon && transform.childCount == 0)
        {
            InventoryManager.Instance.EquipWeaponFromInventory(null);
        }
    }
}
