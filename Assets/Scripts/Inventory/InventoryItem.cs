using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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
    [HideInInspector] public RectTransform rectTransform;
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

    [Header("Double Click Behavior")]
    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f;
    private int clickCount = 0;

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


        float clickTime = Time.time;
        if (clickTime - lastClickTime <= doubleClickThreshold)
            clickCount++;
        else
            clickCount = 1;

        lastClickTime = clickTime;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float clickDuration = Time.time - clickStartTime;

        if (clickCount == 2)
        {
            CallItemEquip();
            clickCount = 0;
            return;
        }

        if (clickDuration <= clickTimeThreshold && !isDragging)
            ToggleItemOptions();
    }

    private void ToggleItemOptions()
    {
        if (itemOptions == null) return;

        ButtonBehaviorForEquippeds();

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
        Vector3 spawnPosition = PlayerComponents.Instance.playerObject.transform.position + PlayerComponents.Instance.playerObject.transform.forward * 1.5f;
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
        StartCoroutine(ItemEquip());
    }

    private IEnumerator ItemEquip()
    {
        ReturnItemOptionsToOriginalParent();
        ButtonBehaviorForEquippeds();

        InventorySlot invSlot = GetComponentInParent<InventorySlot>();

        if (invSlot.slotType is not (InventorySlot.SlotType.Inventory or InventorySlot.SlotType.Stash or InventorySlot.SlotType.Nothing))
        {
            CallItemUnequip();
            yield break;
        }

        Transform targetSlot = item switch
        {
            Weapon => InventoryManager.Instance.weaponSlot.transform,
            Secondary => InventoryManager.Instance.secondarySlot.transform,
            Jewel jewel when jewel is Ring => InventoryManager.Instance.ringSlot.transform,
            Jewel jewel when jewel is Necklace => InventoryManager.Instance.neckSlot.transform,
            Jewel jewel when jewel is Amulet => InventoryManager.Instance.amuletSlot.transform,
            Potion => InventoryManager.Instance.potionSlot.transform,
            _ => null
        };

        if (item is Weapon && ItemIsTwoHanded() && IsWithSecondaryEquipped())
            UnequipHandsItem(InventoryManager.Instance.secondarySlot.transform.GetChild(0).GetComponent<InventoryItem>());
        else if (item is Secondary && isWithTwoHandEquipped())
            UnequipHandsItem(InventoryManager.Instance.weaponSlot.transform.GetChild(0).GetComponent<InventoryItem>());

        if (targetSlot != null && targetSlot.childCount > 0)
        {
            var existingItem = targetSlot.GetChild(0).GetComponent<InventoryItem>();
            existingItem.transform.SetParent(invSlot.transform);
            existingItem.rectTransform.localPosition = Vector3.zero;
        }

        yield return null;
        transform.SetParent(targetSlot);
        rectTransform.localPosition = Vector3.zero;
    }

    private void CallItemUnequip()
    {
        var inventorySlots = InventoryManager.Instance.inventorySlots;
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


    private void ButtonBehaviorForEquippeds()
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

    private bool ItemIsTwoHanded()
    {
        return item is Bow || item is GreatHammer || item is GreatSword || item is GreatAxe;
    }
    private bool ItemIsSecondary()
    {
        if (item is Secondary secondary)
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
            return true;
        else
            return false;
    }

    private void UnequipHandsItem(InventoryItem reference)
    {
        List<InventorySlot> inventorySlots = new List<InventorySlot>(FindObjectsOfType<InventorySlot>());
        int nullIndex = InventoryManager.Instance.inventoryItems.FindIndex(item => item == null);

        InventorySlot emptySlot = inventorySlots.Find(s => s.slotIndex == nullIndex);

        InventoryItem invItem = reference;
        invItem.transform.SetParent(emptySlot.transform);
        invItem.rectTransform.localPosition = Vector3.zero;
    }
}
