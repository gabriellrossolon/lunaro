using UnityEngine.EventSystems;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Inventory, Weapon, Nothing, Secondary, Ring, Necklace, Amulet , Potion, Stash}
    public SlotType slotType;

    public int slotIndex;
    private int stashIndexStartAt = 33;
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
        else if (DraggedItemIsTwoHanded(draggedItem))
        {
            if (IsWithSecondaryEquipped() && slotType == SlotType.Weapon)
            {
                ReturnItemToPreviousParent(draggedItem);
                return;
            }
        }
        else if (DraggedItemIsSecondary(draggedItem))
        {
            if (isWithTwoHandEquipped() && slotType == SlotType.Secondary)
            {
                ReturnItemToPreviousParent(draggedItem);
                return;
            }
        }

        // Place the item in the slot and update the inventory list
        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;
        draggedItem.previousParent = transform;

        UpdateInventoryList(draggedItem);
        UpdateStashList(draggedItem);

        // Equip the item if it's a weapon
        //EquipItemIfNeeded(draggedItem);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
    }

    // VERIFICAÇÕES SE O ITEM PODE OU NÃO ENTRAR NO SLOT
    private bool IsItemCompatibleWithSlot(InventoryItem draggedItem)
    {
        if (draggedItem == null || draggedItem.item == null)
            return false;

        switch (slotType)
        {
            case SlotType.Inventory:
                return true;

            case SlotType.Stash:
                return true;

            case SlotType.Weapon:
                return draggedItem.item is Weapon;

            case SlotType.Secondary:
                return draggedItem.item is Secondary;

            case SlotType.Ring:
            case SlotType.Necklace:
            case SlotType.Amulet:
                if (draggedItem.item is Jewel jewel)
                {
                    return (slotType == SlotType.Ring && jewel.jewelType == Jewel.JewelType.Ring) ||
                           (slotType == SlotType.Necklace && jewel.jewelType == Jewel.JewelType.Necklace) ||
                           (slotType == SlotType.Amulet && jewel.jewelType == Jewel.JewelType.Amulet);
                }
                return false;

            case SlotType.Potion:
                return draggedItem.item is Potion;

            case SlotType.Nothing:
                return false;

            default:
                return false;
        }
    }

    private bool DraggedItemIsTwoHanded(InventoryItem draggedItem)
    {
        if (draggedItem.item is Weapon weapon && 
            (weapon.weaponType == Weapon.WeaponType.Bow || weapon.weaponType == Weapon.WeaponType.GreatHammer || weapon.weaponType == Weapon.WeaponType.GreatSword))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool DraggedItemIsSecondary(InventoryItem draggedItem)
    {
        if (draggedItem.item is Secondary secondary)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool isWithTwoHandEquipped()
    {
        if (InventoryManager.Instance.equippedWeapon != null &&
           InventoryManager.Instance.equippedWeapon is Weapon weapon &&
           (weapon.weaponType == Weapon.WeaponType.Bow || weapon.weaponType == Weapon.WeaponType.GreatHammer || weapon.weaponType == Weapon.WeaponType.GreatSword))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool IsWithSecondaryEquipped()
    {
        if (InventoryManager.Instance.equippedSecondary != null && 
            InventoryManager.Instance.equippedSecondary is Secondary secondary)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
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
                if (draggedItem.item is Secondary secondary)
                {
                    EquipSecondary(secondary);
                }
                break;

            case SlotType.Ring:
            case SlotType.Necklace:
            case SlotType.Amulet:
                if (draggedItem.item is Jewel jewel)
                {
                    EquipJewel(jewel);
                }
                break;

            case SlotType.Potion:
                if (draggedItem.item is Potion potion)
                {
                    EquipPotion(potion);
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
        else if (DraggedItemIsTwoHanded(draggedItem))
        {
            if (IsWithSecondaryEquipped() && slotType == SlotType.Weapon)
            {
                ReturnItemToPreviousParent(draggedItem);
                return;
            }
        }
        else if (DraggedItemIsSecondary(draggedItem))
        {
            if (isWithTwoHandEquipped() && slotType == SlotType.Secondary)
            {
                ReturnItemToPreviousParent(draggedItem);
                return;
            }
        }
        else if (IsWithSecondaryEquipped())
        {
            if (existingItem.item is Weapon weapon &&
                (weapon.weaponType == Weapon.WeaponType.Bow || weapon.weaponType == Weapon.WeaponType.GreatHammer || weapon.weaponType == Weapon.WeaponType.GreatSword))
            {
                ReturnItemToPreviousParent(draggedItem);
                return;
            }
        }
        else if (isWithTwoHandEquipped())
        {
            if (existingItem.item is Secondary secondary)
            {
                ReturnItemToPreviousParent(draggedItem);
                return;
            }
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
        if(slotType == SlotType.Inventory)
            UpdateInventoryList(draggedItem);

        if (slotType == SlotType.Stash)
            UpdateStashList(existingItem);

        // Equip the new item if needed
        //EquipItemIfNeeded(draggedItem);

        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
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
    private void EquipWeapon(Weapon weapon)
    {
        InventoryManager.Instance.EquipWeaponFromInventory(weapon);
        RefreshHandsPOV();
    }

    private void EquipSecondary(Secondary secondary)
    {
        InventoryManager.Instance.EquipSecondaryFromInventory(secondary);
        RefreshHandsPOV(); // Atualiza a visão das mãos, se necessário
    }

    private void EquipJewel(Jewel jewel)
    {
        InventoryManager.Instance.EquipJewelFromInventory(jewel, jewel.jewelType);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
    }

    private void EquipPotion(Potion potion)
    {
        InventoryManager.Instance.EquipPotionFromInventory(potion);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
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
        
    }

    private void OnTransformChildrenChanged()
    {
        if(slotType != SlotType.Inventory || slotType != SlotType.Nothing)
        {
            EquipItemIfNeeded(GetComponentInChildren<InventoryItem>());
        }
        HandleEmptyEquipSlot();
    }

    // Update the inventory list with the item from the current slot
    private void UpdateItemIndex()
    {
        if (slotType == SlotType.Inventory)
        {
            InventoryItem childItem = GetComponentInChildren<InventoryItem>();
            InventoryManager.Instance.inventoryItems[slotIndex] = childItem?.item;
        }

        if(slotType == SlotType.Stash)
        {
            InventoryItem childItem = GetComponentInChildren<InventoryItem>();
            InventoryManager.Instance.stashItems[slotIndex - stashIndexStartAt] = childItem?.item;
        }
    }

    // Handle the case when the item slot is empty
    private void HandleEmptyEquipSlot()
    {
        if (transform.childCount == 0)
        {
            switch (slotType)
            {
                case SlotType.Weapon:
                    InventoryManager.Instance.EquipWeaponFromInventory(null);
                    break;

                case SlotType.Secondary:
                    InventoryManager.Instance.EquipSecondaryFromInventory(null);
                    break;

                case SlotType.Ring:
                    InventoryManager.Instance.EquipJewelFromInventory(null, Jewel.JewelType.Ring);
                    break;

                case SlotType.Necklace:
                    InventoryManager.Instance.EquipJewelFromInventory(null, Jewel.JewelType.Necklace);
                    break;

                case SlotType.Amulet:
                    InventoryManager.Instance.EquipJewelFromInventory(null, Jewel.JewelType.Amulet);
                    break;

                case SlotType.Potion:
                    InventoryManager.Instance.EquipPotionFromInventory(null);
                    break;

                default:
                    break; // Caso não seja nenhum dos tipos tratados
            }
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


    private void UpdateStashList(InventoryItem draggedItem)
    {
        var stashList = InventoryManager.Instance.stashItems;

        if (slotIndex >= stashIndexStartAt && slotIndex - stashIndexStartAt < stashList.Count) //Aqui começa os Index do StashSlot
        {
            var previousIndex = stashList.IndexOf(draggedItem.item);
            if (previousIndex != -1 && previousIndex != slotIndex)
            {
                stashList[previousIndex] = null;
            }

            stashList[slotIndex - stashIndexStartAt] = draggedItem.item; //Subtrai a quantia de Index de Stash Slot, EX: Se é o Index 33, vai pra stashIndex 0
        }
        else if (slotType == SlotType.Stash)
        {
            Debug.LogWarning($"Índice {slotIndex} fora dos limites da lista de stash (deve ser 1 ou maior).");
        }
    }

}


