using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public ItemBase item;
    private CanvasGroup canvasGroup;
    private Image iconImage;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    [HideInInspector] public Transform previousParent;
    private Canvas originalCanvas;

    [SerializeField] private GameObject itemOptions;
    private float clickTimeThreshold = 0.2f;
    private float clickStartTime;
    private bool isDragging = false;
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
            itemOptions.SetActive(false);
        }
    }
}
