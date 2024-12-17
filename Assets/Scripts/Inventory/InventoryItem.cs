using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScriptableObject item;
    private CanvasGroup canvasGroup;
    private Image iconImage;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    [HideInInspector] public Transform previousParent;
    private Canvas originalCanvas; // Armazena o Canvas original do item

    void Awake()
    {
        InitializeComponents();
        previousParent = transform.parent;
        originalCanvas = GetComponentInParent<Canvas>(); // Armazena o Canvas atual
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
        // Garante que o item fique acima de todos os outros UI Elements
        transform.SetAsLastSibling();
    }

    private void MoveItemToTopCanvas()
    {
        // Remove o item da hierarquia atual e coloca no Canvas principal
        transform.SetParent(originalCanvas.transform, true); // 'true' mantém a posição relativa
        rectTransform.position = Input.mousePosition; // Coloca o item na posição do mouse
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
            // Caso o item seja solto em uma posição válida, ele não volta para a posição original
            // Aqui você pode adicionar lógica para manipular a troca ou remoção de item.
        }

        // Voltar o item para a hierarquia original
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

