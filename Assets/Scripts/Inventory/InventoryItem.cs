using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScriptableObject item;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position; // Armazena a posição original
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!eventData.pointerEnter || eventData.pointerEnter.GetComponent<InventorySlot>() == null)
        {
            // Se o item não for solto em um slot válido, retorna à posição original
            rectTransform.position = originalPosition;
        }
        else
        {
            // Se o item for solto em um slot válido, centraliza o item no slot
            InventorySlot slot = eventData.pointerEnter.GetComponent<InventorySlot>();
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            rectTransform.position = slotRect.position;
        }
    }
}