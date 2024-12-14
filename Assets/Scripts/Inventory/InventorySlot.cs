using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public enum SlotType { Inventory, Weapon }
    public SlotType slotType;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag?.GetComponent<InventoryItem>();

        if (draggedItem != null)
        {
            // Verifica se o slot já contém um item
            if (transform.childCount > 0)
            {
                var existingItem = transform.GetChild(0).GetComponent<InventoryItem>();
                HandleItemDrop(draggedItem, existingItem);
            }
            else
            {
                // Slot vazio: coloca o item diretamente
                PlaceItemInSlot(draggedItem);
            }
        }
    }

    private void HandleItemDrop(InventoryItem draggedItem, InventoryItem existingItem)
    {
        // Determina a ação com base no tipo do slot
        switch (slotType)
        {
            case SlotType.Inventory:
                SwapItems(draggedItem, existingItem);
                break;

            case SlotType.Weapon:
                // Verifica se o item sendo arrastado é uma arma
                if (draggedItem.item is Weapon)
                {
                    EquipWeapon(draggedItem.item as Weapon);
                    SwapItems(draggedItem, existingItem);
                }
                else
                {
                    ReturnItemToPreviousParent(draggedItem);
                }
                break;

            default:
                ReturnItemsToPreviousParents(draggedItem, existingItem);
                break;
        }
    }

    private void EquipWeapon(Weapon weapon)
    {
        // Equipar a arma via InventoryManager
        InventoryManager.Instance.EquipWeaponFromInventory(weapon);
        InventoryManager.Instance.handsPOV.SetActive(false);
        InventoryManager.Instance.handsPOV.SetActive(true);
    }

    private void SwapItems(InventoryItem draggedItem, InventoryItem existingItem)
    {
        // Guarda os pais anteriores
        Transform draggedPreviousParent = draggedItem.previousParent;
        Transform existingPreviousParent = existingItem.previousParent;

        // Troca os itens de lugar
        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;

        existingItem.transform.SetParent(draggedPreviousParent);
        existingItem.transform.localPosition = Vector3.zero;

        // Atualiza os pais registrados
        draggedItem.previousParent = transform;
        existingItem.previousParent = draggedPreviousParent;
    }

    private void PlaceItemInSlot(InventoryItem draggedItem)
    {
        // Verifica compatibilidade do tipo antes de adicionar
        if (slotType == SlotType.Weapon && !(draggedItem.item is Weapon))
        {
            Debug.LogWarning($"Item '{draggedItem.item.name}' não pode ser colocado em um slot do tipo Weapon.");
            ReturnItemToPreviousParent(draggedItem);
            return;
        }

        // Coloca o item no slot
        draggedItem.transform.SetParent(transform);
        draggedItem.transform.localPosition = Vector3.zero;
        draggedItem.previousParent = transform;

        // Equipar se for uma arma e o slot for de arma
        if (slotType == SlotType.Weapon && draggedItem.item is Weapon weapon)
        {
            EquipWeapon(weapon);
        }
    }

    private void ReturnItemToPreviousParent(InventoryItem draggedItem)
    {
        // Retorna o item ao seu local original
        draggedItem.transform.SetParent(draggedItem.previousParent);
        draggedItem.transform.localPosition = Vector3.zero;
    }

    private void ReturnItemsToPreviousParents(InventoryItem draggedItem, InventoryItem existingItem)
    {
        // Retorna ambos os itens para seus locais originais
        ReturnItemToPreviousParent(draggedItem);
        existingItem.transform.SetParent(existingItem.previousParent);
        existingItem.transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        // Garante que se o slot de Weapon ficar vazio, a arma equipada seja desativada
        if (slotType == SlotType.Weapon && transform.childCount == 0)
        {
            InventoryManager.Instance.EquipWeaponFromInventory(null);
        }
    }
}
