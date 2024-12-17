using UnityEngine.EventSystems;
using UnityEngine;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Inventory, Weapon, Nothing }
    public SlotType slotType;

    [SerializeField] public int slotIndex;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem != null)
        {
            if (transform.childCount > 0)
            {
                var existingItem = transform.GetChild(0).GetComponent<InventoryItem>();
                SwapItems(draggedItem, existingItem);
            }
            else
            {
                PlaceItemInSlot(draggedItem);
            }
        }
    }

    // Place the item in the current slot, ensuring it's valid for the slot type
    private void PlaceItemInSlot(InventoryItem draggedItem)
    {
        // Check if the item is compatible with the slot type
        if (!IsItemCompatibleWithSlot(draggedItem))
        {
            ReturnItemToPreviousParent(draggedItem);
            return;
        }

        // Place the item in the slot and update the inventory list
        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;
        draggedItem.previousParent = transform;

        UpdateInventoryList(draggedItem);

        // Equip the item if it's a weapon
        EquipWeaponIfNeeded(draggedItem);
    }

    // Check if the item is compatible with the slot type
    private bool IsItemCompatibleWithSlot(InventoryItem draggedItem)
    {
        return !(slotType == SlotType.Weapon && !(draggedItem.item is Weapon));
    }

    // Equip the weapon if the item is a weapon
    private void EquipWeaponIfNeeded(InventoryItem draggedItem)
    {
        if (slotType == SlotType.Weapon && draggedItem.item is Weapon weapon)
        {
            EquipWeapon(weapon);
        }
    }

    // Swap the items between the current slot and the dragged item
    private void SwapItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        if (!IsItemCompatibleWithSlot(draggedItem))
        {
            ReturnItemToPreviousParent(draggedItem);
            return;
        }

        Transform draggedPreviousParent = draggedItem.previousParent;
        Transform existingPreviousParent = existingItem.previousParent;

        // Swap the positions of the items
        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;

        existingItem.transform.SetParent(draggedPreviousParent);
        existingItem.transform.localPosition = Vector3.zero;

        // Update the previous parents
        draggedItem.previousParent = transform;
        existingItem.previousParent = draggedPreviousParent;

        // Update the inventory list
        UpdateInventoryList(draggedItem);
        UpdateInventoryList(existingItem);

        // Equip the new weapon if needed
        EquipWeaponIfNeeded(draggedItem);
    }

    // Remove the item from the current slot
    public void RemoveItemFromSlot()
    {
        if (transform.childCount > 0)
        {
            var itemToRemove = transform.GetChild(0).GetComponent<InventoryItem>();
            if (itemToRemove != null)
            {
                itemToRemove.transform.SetParent(null);
                itemToRemove.transform.localPosition = Vector3.zero;
                UpdateInventoryList(itemToRemove);
            }
        }
    }

    // Equip a weapon from the inventory
    private void EquipWeapon(Weapon weapon)
    {
        InventoryManager.Instance.EquipWeaponFromInventory(weapon);
        RefreshHandsPOV();
    }

    // Handle the refresh of the hands POV after equipping a weapon
    private void RefreshHandsPOV()
    {
        InventoryManager.Instance.handsPOV.SetActive(false);
        InventoryManager.Instance.handsPOV.SetActive(true);
    }

    // Return the dragged item to its previous parent if it is invalid
    private void ReturnItemToPreviousParent(InventoryItem draggedItem)
    {
        draggedItem.transform.SetParent(draggedItem.previousParent);
        draggedItem.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        UpdateItemIndex();
        HandleEmptyWeaponSlot();
    }

    // Update the inventory list with the item from the current slot
    private void UpdateItemIndex()
    {
        if (slotType == SlotType.Inventory)
        {
            InventoryItem childItem = GetComponentInChildren<InventoryItem>();
            InventoryManager.Instance.inventoryItems[slotIndex] = childItem?.item;
        }
    }

    // Handle the case when the weapon slot is empty
    private void HandleEmptyWeaponSlot()
    {
        if (slotType == SlotType.Weapon && transform.childCount == 0)
        {
            InventoryManager.Instance.EquipWeaponFromInventory(null);
        }
    }

    // Update the inventory list when an item is placed in the slot
    private void UpdateInventoryList(InventoryItem draggedItem)
    {
        var inventoryList = InventoryManager.Instance.inventoryItems;

        if (slotIndex >= 0 && slotIndex < inventoryList.Count)
        {
            var previousIndex = inventoryList.IndexOf(draggedItem.item);
            if (previousIndex != -1 && previousIndex != slotIndex)
            {
                inventoryList[previousIndex] = null;
            }

            inventoryList[slotIndex] = draggedItem.item;
        }
        else if(slotType == SlotType.Inventory)
        {
            Debug.LogWarning($"Índice {slotIndex} fora dos limites da lista de inventário.");
        }
    }
}


