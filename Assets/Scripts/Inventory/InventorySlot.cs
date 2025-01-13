using UnityEngine.EventSystems;
using UnityEngine;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Inventory, Weapon, Nothing, Secondary }
    public SlotType slotType;

    public int slotIndex;

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
        //EquipItemIfNeeded(draggedItem);
    }

    // Check if the item is compatible with the slot type
    private bool IsItemCompatibleWithSlot(InventoryItem draggedItem)
    {
        if (draggedItem == null || draggedItem.item == null)
            return false;

        switch (slotType)
        {
            case SlotType.Inventory:
                // Aceita qualquer tipo de item no inventário
                return true;

            case SlotType.Weapon:
                // Aceita apenas itens do tipo Weapon
                return draggedItem.item is Weapon;

            case SlotType.Secondary:
                // Aceita apenas itens secundários (você pode adicionar uma verificação específica para isso)
                return draggedItem.item is Secondary; // Assumindo que você tenha uma classe ou interface chamada `SecondaryItem`

            case SlotType.Nothing:
                // Não aceita nenhum item
                return false;

            default:
                // Caso o tipo não seja tratado, retorna falso por segurança
                return false;
        }
    }

    // Equip the weapon if the item is a weapon
    private void EquipItemIfNeeded(InventoryItem draggedItem)
    {
        if (draggedItem == null || draggedItem.item == null)
            return;

        switch (slotType)
        {
            case SlotType.Weapon:
                if (draggedItem.item is Weapon weapon)
                {
                    EquipWeapon(weapon);
                }
                break;

            case SlotType.Secondary:
                if (draggedItem.item is Secondary secondaryItem)
                {
                    EquipSecondary(secondaryItem);
                }
                break;

            default:
                // Nenhuma ação necessária para outros tipos de slots
                break;
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

        // Equip the new item if needed
        //EquipItemIfNeeded(draggedItem);
    }

    // Remove the item from the current slot
    //public void RemoveItemFromSlot()
    //{
    //    if (transform.childCount > 0)
    //    {
    //        var itemToRemove = transform.GetChild(0).GetComponent<InventoryItem>();
    //        if (itemToRemove != null)
    //        {
    //            itemToRemove.transform.SetParent(null);
    //            itemToRemove.transform.localPosition = Vector3.zero;
    //            UpdateInventoryList(itemToRemove);
    //        }
    //    }
    //}

    // Equip a weapon from the inventory
    private void EquipWeapon(Weapon weapon)
    {
        InventoryManager.Instance.EquipWeaponFromInventory(weapon);
        RefreshHandsPOV();
    }

    private void EquipSecondary(Secondary secondaryItem)
    {
        InventoryManager.Instance.EquipSecondaryFromInventory(secondaryItem);
        RefreshHandsPOV(); // Atualiza a visão das mãos, se necessário
    }

    // Handle the refresh of the hands POV after equipping a weapon
    private void RefreshHandsPOV()
    {
        InventoryManager.Instance.handsPOV.SetActive(false);
        InventoryManager.Instance.handsPOV.SetActive(true);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
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
        HandleEmptyEquipSlot();
    }

    private void OnTransformChildrenChanged()
    {
        if(slotType == SlotType.Weapon || slotType == SlotType.Secondary)
        {
            EquipItemIfNeeded(GetComponentInChildren<InventoryItem>());
        }
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

    // Handle the case when the item slot is empty
    private void HandleEmptyEquipSlot()
    {
        if (slotType == SlotType.Weapon && transform.childCount == 0)
        {
            InventoryManager.Instance.EquipWeaponFromInventory(null);
        }

        if (slotType == SlotType.Secondary && transform.childCount == 0)
        {
            InventoryManager.Instance.EquipSecondaryFromInventory(null);
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


