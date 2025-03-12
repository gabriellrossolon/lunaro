using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Item References")]
    public ItemBase item;
    private Image iconImage;

    [Header("Drop References")]
    [SerializeField] private GameObject dropBox;

    [Header("Canvas References")]
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    [HideInInspector] public Transform previousParent;
    private Canvas originalCanvas;
    private bool isDragging = false;

    [Header("Options References")]
    [SerializeField] private GameObject itemOptions;
    [SerializeField] private GameObject equipBtn;
    [SerializeField] private GameObject dropBtn;
    private float clickTimeThreshold = 0.2f;
    private float clickStartTime;
    private Transform optionsOriginalParent;


    private void Awake()
    {
        InitializeComponents();
        previousParent = transform.parent;
        originalCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (itemOptions.activeSelf)
            if(!HudManager.Instance.openedInventory)
                itemOptions.SetActive(false);
    }

    private void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        iconImage = GetComponent<Image>();
    }

    //Funções de Dragging

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        if (itemOptions != null && itemOptions.activeSelf)
            ReturnItemOptionsToOriginalParent();

        StoreOriginalPosition();
        SetDraggableState();
        BringToTop();
        MoveItemToTopCanvas();
        HudManager.Instance.itemShow.SetActive(false);
    }

    private void StoreOriginalPosition()
    {
        originalPosition = rectTransform.position;
        previousParent = transform.parent;
    }

    private void SetDraggableState()
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    private void BringToTop()
    {
        transform.SetAsLastSibling();
    }

    private void MoveItemToTopCanvas()
    {
        transform.SetParent(originalCanvas.transform, true);
        rectTransform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateItemPosition(eventData.position);
    }

    private void UpdateItemPosition(Vector3 newPosition)
    {
        rectTransform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetDraggableState();

        if (ShouldReturnItem(eventData))
            ReturnItemToOriginalPosition();

        transform.SetParent(previousParent);
        rectTransform.localPosition = Vector3.zero;
    }

    private void ResetDraggableState()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private bool ShouldReturnItem(PointerEventData eventData)
    {
        return eventData.pointerEnter == null ||
            eventData.pointerEnter.GetComponent<InventorySlot>() == null ||
            eventData.pointerEnter.transform == previousParent;
    }

    private void ReturnItemToOriginalPosition()
    {
        transform.SetParent(previousParent);
        transform.localPosition = Vector3.zero;
    }

    //Funções de Pointer (Mouse) no Item do Slot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HudManager.Instance.RefreshItemShow(item);
        HudManager.Instance.itemShow.SetActive(true);
        HudManager.Instance.itemShow.transform.position = eventData.position;
        HudManager.Instance.SetCursor(HudManager.Instance.cursorClicker);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HudManager.Instance.itemShow.SetActive(false);
        HudManager.Instance.SetCursor(HudManager.Instance.cursorPointer);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickStartTime = Time.time;
        isDragging = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float clickDuration = Time.time - clickStartTime;

        if (clickDuration <= clickTimeThreshold && !isDragging)
            ToggleItemOptions();
    }

    private void ToggleItemOptions()
    {
        if (itemOptions == null) return;

        BehaviorForEquippeds();

        bool isActive = itemOptions.activeSelf;
        itemOptions.SetActive(!isActive);

        if (!isActive)
        {
            optionsOriginalParent = itemOptions.transform.parent;
            itemOptions.transform.SetParent(originalCanvas.transform, true);
        }
        else
        {
            ReturnItemOptionsToOriginalParent();
        }
    }

    private void ReturnItemOptionsToOriginalParent()
    {
        if (itemOptions != null && optionsOriginalParent != null)
        {
            itemOptions.transform.SetParent(optionsOriginalParent, true);
            itemOptions.transform.localPosition = Vector3.zero;
            itemOptions.SetActive(false);
        }
    }

    //Funções dos Options Buttons

    public void DropItem()
    {
        Vector3 spawnPosition = PlayerManager.Instance.transform.position + PlayerManager.Instance.transform.forward * 1.5f;
        GameObject newDropBox = Instantiate(dropBox, spawnPosition, Quaternion.identity);

        DroppedItem droppedItem = newDropBox.GetComponent<DroppedItem>();
        droppedItem.canThrow = false;

        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.dropSound);

        if (droppedItem != null)
            droppedItem.item = item;

        Destroy(itemOptions);
        InventoryManager.Instance.RemoveItem(item);
    }

    public void CallItemEquip()
    {
        ReturnItemOptionsToOriginalParent();
        BehaviorForEquippeds();
        InventorySlot invSlot = GetComponentInParent<InventorySlot>();

        if(invSlot.slotType != InventorySlot.SlotType.Inventory &&
            invSlot.slotType != InventorySlot.SlotType.Stash &&
            invSlot.slotType != InventorySlot.SlotType.Nothing)
        {
            CallItemUnequip();
            return;
        }

        switch (item)
        {
            case Weapon weapon:
                if (InventoryManager.Instance.weaponSlot.transform.childCount > 0)
                {
                    var existingItem = InventoryManager.Instance.weaponSlot.transform.GetChild(0).GetComponent<InventoryItem>();

                    existingItem.transform.SetParent(invSlot.transform);
                    existingItem.rectTransform.localPosition = Vector3.zero;

                    Invoke("MoveWeaponToSlot", 0.01f);
                }
                else
                {
                    MoveWeaponToSlot();
                }
                break;

            default:
                Debug.Log("Item desconhecido");
                break;
        }
    }

    private void MoveWeaponToSlot()
    {
        transform.SetParent(InventoryManager.Instance.weaponSlot.transform);
        rectTransform.localPosition = Vector3.zero;
    }

    private void CallItemUnequip()
    {
        List<InventorySlot> inventorySlots = new List<InventorySlot>(FindObjectsOfType<InventorySlot>());
        int nullIndex = InventoryManager.Instance.inventoryItems.FindIndex(item => item == null);

        if (nullIndex == -1)
            return;

        InventorySlot slot = inventorySlots.Find(s => s.slotIndex == nullIndex);

        if (slot == null)
            return;

        Transform slotTransform = slot.transform;
        transform.SetParent(slotTransform);
        rectTransform.localPosition = Vector3.zero;
    }


    private void BehaviorForEquippeds()
    {
        InventorySlot invSlot = GetComponentInParent<InventorySlot>();

        if (invSlot.slotType == InventorySlot.SlotType.Inventory ||
            invSlot.slotType == InventorySlot.SlotType.Stash ||
            invSlot.slotType == InventorySlot.SlotType.Nothing)
        {
            dropBtn.SetActive(true);
            equipBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
        }
        else
        {
            dropBtn.SetActive(false);
            equipBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
        }
    }
}
