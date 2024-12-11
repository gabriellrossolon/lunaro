using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool isEquipSlot = false;

    public void OnDrop(PointerEventData eventData)
    {
        var draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();

        if (isEquipSlot)
        {
            Weapon weapon = draggedItem.item as Weapon;
            if (weapon != null)
            {
                InventoryManager.Instance.EquipWeaponFromInventory(weapon);
            }
            else
            {
                Debug.Log("Este slot só aceita armas.");
            }
        }
    }
}
