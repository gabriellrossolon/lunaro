using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScriptableObject item;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    [HideInInspector] public Transform previousParent;

    void Awake()
    {
        InitializeComponents();
        previousParent = transform.parent;
    }

    private void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StoreOriginalPosition();
        SetDraggableState();
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
    }

    private void ResetDraggableState()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private bool ShouldReturnItem(PointerEventData eventData)
    {
        // Verifica se o item foi solto no mesmo slot ou em nenhum slot válido
        return eventData.pointerEnter == null || eventData.pointerEnter.GetComponent<InventorySlot>() == null ||
               eventData.pointerEnter.transform == previousParent;
    }

    private void ReturnItemToOriginalPosition()
    {
        // Coloca o item de volta ao seu parent original
        transform.SetParent(previousParent);

        // Centraliza o item dentro do slot
        transform.localPosition = Vector3.zero;
    }
}

