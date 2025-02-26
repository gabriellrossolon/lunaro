using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections;
using System.Linq;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Inventory, Weapon, Nothing, Secondary, Ring, Necklace, Amulet , Potion, Stash}
    public SlotType slotType;

    public int slotIndex;
    private int stashIndexStartAt = 49;
    private void Update()
    {
        UpdateItemIndex();  
    }

    private void Start()
    {
        StartCoroutine(LateStart(0.1f));
    }

    private IEnumerator LateStart(float time)
    {
        yield return new WaitForSeconds(time);
        if (slotType != SlotType.Inventory || slotType != SlotType.Stash || slotType != SlotType.Nothing)
        {
            EquipItemIfNeeded(GetComponentInChildren<InventoryItem>());
        }
    }

    private void OnTransformChildrenChanged()
    {
        if (slotType != SlotType.Inventory || slotType != SlotType.Stash || slotType != SlotType.Nothing)
        {
            EquipItemIfNeeded(GetComponentInChildren<InventoryItem>());
        }
        HandleEmptyEquipSlot();
    }


    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem != null)
        {
            if (transform.childCount > 0)
            {
                var existingItem = transform.GetChild(0).GetComponent<InventoryItem>();
                StartCoroutine(SwapItems(draggedItem, existingItem));
            }
            else
            {
                PlaceItemInSlot(draggedItem);
            }
        }
    }

    private void PlaceItemInSlot(InventoryItem draggedItem)
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

        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;
        draggedItem.previousParent = transform;

        UpdateInventoryList(draggedItem);
        UpdateStashList(draggedItem);

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
            case SlotType.Stash:
                return true;

            case SlotType.Weapon:
                return draggedItem.item is Weapon;

            case SlotType.Secondary:
                return draggedItem.item is Secondary;

            case SlotType.Ring:
                return draggedItem.item is Ring;

            case SlotType.Necklace:
                return draggedItem.item is Necklace;

            case SlotType.Amulet:
                return draggedItem.item is Amulet;

            case SlotType.Potion:
                return draggedItem.item is Potion;

            case SlotType.Nothing:
            default:
                return false;
        }
    }
    private bool DraggedItemIsTwoHanded(InventoryItem draggedItem)
    {
        return draggedItem.item is Bow || draggedItem.item is GreatHammer || draggedItem.item is GreatSword || draggedItem.item is GreatAxe;
    }
    private bool DraggedItemIsSecondary(InventoryItem draggedItem)
    {
        if (draggedItem.item is Secondary secondary)
            return true;
        else
            return false;
    }
    private bool isWithTwoHandEquipped()
    {
        var equippedWeapon = InventoryManager.Instance.equippedWeapon;
        return equippedWeapon is Bow || equippedWeapon is GreatHammer || equippedWeapon is GreatSword || equippedWeapon is GreatAxe;
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
                break;
        }
    }

    private IEnumerator SwapItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        if (!IsItemCompatibleWithSlot(draggedItem))
        {
            ReturnItemToPreviousParent(draggedItem);
            yield break;
        }
        else if (DraggedItemIsTwoHanded(draggedItem))
        {
            if (IsWithSecondaryEquipped() && slotType == SlotType.Weapon)
            {
                ReturnItemToPreviousParent(draggedItem);
                yield break;
            }
        }
        else if (DraggedItemIsSecondary(draggedItem))
        {
            if (isWithTwoHandEquipped() && slotType == SlotType.Secondary)
            {
                ReturnItemToPreviousParent(draggedItem);
                yield break;
            }
        }
        else if (isWithTwoHandEquipped())
        {
            if (existingItem.item is Secondary secondary)
            {
                ReturnItemToPreviousParent(draggedItem);
                yield break;
            }
        }
        else if (IsWithSecondaryEquipped())
        {
            if (existingItem.item is Bow || existingItem.item is GreatHammer || existingItem.item is GreatSword || existingItem.item is GreatAxe)
            {
                ReturnItemToPreviousParent(draggedItem);
                yield break;
            }
        }
        
        Transform draggedPreviousParent = draggedItem.previousParent;
        Transform existingPreviousParent = existingItem.previousParent;

        existingItem.transform.SetParent(draggedPreviousParent);
        existingItem.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(0.05f); //Pequeno delay para que não ocorra a troca de ambos no mesmo Frame

        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;
        

        draggedItem.previousParent = transform;
        existingItem.previousParent = draggedPreviousParent;

        if(slotType == SlotType.Inventory)
            UpdateInventoryList(draggedItem);

        if (slotType == SlotType.Stash)
            UpdateStashList(existingItem);

        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
    }
    private void EquipWeapon(Weapon weapon)
    {
        InventoryManager.Instance.EquipWeaponFromInventory(weapon);
        RefreshHandsPOV();
    }

    private void EquipSecondary(Secondary secondary)
    {
        InventoryManager.Instance.EquipSecondaryFromInventory(secondary);
        RefreshHandsPOV();
    }

    private void EquipJewel(Jewel jewel)
    {
        switch (jewel)
        {
            case Ring:
                InventoryManager.Instance.EquipJewelFromInventory(jewel, "Ring");
                break;
            case Necklace:
                InventoryManager.Instance.EquipJewelFromInventory(jewel, "Necklace");
                break;
            case Amulet:
                InventoryManager.Instance.EquipJewelFromInventory(jewel, "Amulet");
                break;
        }
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
    }

    private void EquipPotion(Potion potion)
    {
        InventoryManager.Instance.EquipPotionFromInventory(potion);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
    }

    private void RefreshHandsPOV()
    {
        InventoryManager.Instance.handsPOV.SetActive(false);
        InventoryManager.Instance.handsPOV.SetActive(true);
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.inventoryEquipSound);
    }

    private void ReturnItemToPreviousParent(InventoryItem draggedItem)
    {
        draggedItem.transform.SetParent(draggedItem.previousParent);
        draggedItem.transform.localPosition = Vector3.zero;
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

    private void HandleEmptyEquipSlot() //PASSA VALOR NULL PARA O SLOT CASO NÃO TENHA NADA NELE (OU SEJA PASSA QUE O SLOT VAI EQUIPAR NADA)
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
                    InventoryManager.Instance.EquipJewelFromInventory(null, "Ring");
                    break;

                case SlotType.Necklace:
                    InventoryManager.Instance.EquipJewelFromInventory(null, "Necklace");
                    break;

                case SlotType.Amulet:
                    InventoryManager.Instance.EquipJewelFromInventory(null, "Amulet");
                    break;

                case SlotType.Potion:
                    InventoryManager.Instance.EquipPotionFromInventory(null);
                    break;

                default:
                    break;
            }
        }
    }

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

        if (slotIndex >= stashIndexStartAt && slotIndex - stashIndexStartAt < stashList.Count) 
        {
            var previousIndex = stashList.IndexOf(draggedItem.item);
            if (previousIndex != -1 && previousIndex != slotIndex)
            {
                stashList[previousIndex] = null;
            }

            stashList[slotIndex - stashIndexStartAt] = draggedItem.item;
        }
        else if (slotType == SlotType.Stash)
        {
            Debug.LogWarning($"Índice {slotIndex} fora dos limites da lista de stash (deve ser 1 ou maior).");
        }
    }
}