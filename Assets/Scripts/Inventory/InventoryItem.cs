using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemBase item;
    private CanvasGroup canvasGroup;
    private Image iconImage;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    [HideInInspector] public Transform previousParent;
    private Canvas originalCanvas;

    void Awake()
    {
        InitializeComponents();
        previousParent = transform.parent;
        originalCanvas = GetComponentInParent<Canvas>();
    }

    private void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        iconImage = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        transform.SetParent(originalCanvas.transform, true); //Tira o objeto como filho do Slot assim que começa a mexer ele.
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
        {
            ReturnItemToOriginalPosition();
        }
        else
        {
        }

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
        return eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InventorySlot>() == null ||
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

        Vector3 mousePosition = eventData.position;  // Posição do mouse na tela
        HudManager.Instance.itemShow.transform.position = mousePosition;

        HudManager.Instance.SetCursor(HudManager.Instance.cursorClicker);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HudManager.Instance.itemShow.SetActive(false);

        HudManager.Instance.SetCursor(HudManager.Instance.cursorPointer);
    }
}


