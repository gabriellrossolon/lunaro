using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float maxDistance = 2f;

    private Transform cameraTransform;
    private bool isInteractingWithItem;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        CheckForInteractions();
    }

    private void CheckForInteractions()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            HudManager.Instance.warnBtn.SetActive(true);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && PlayerManager.Instance.inputHandler.interactInput)
            {
                interactable.Interact();
                PlayerManager.Instance.inputHandler.interactInput = false;
            }

            DroppedItem droppedItem = hit.collider.GetComponent<DroppedItem>();
            if (droppedItem != null)
            {
                HudManager.Instance.RefreshItemShow(droppedItem.item);
                HudManager.Instance.itemShow.SetActive(true);
                isInteractingWithItem = true;
            }
            else if (isInteractingWithItem)
            {
                HudManager.Instance.itemShow.SetActive(false);
                isInteractingWithItem = false;
            }
        }
        else
        {
            HudManager.Instance.warnBtn.SetActive(false);

            if (isInteractingWithItem)
            {
                HudManager.Instance.itemShow.SetActive(false);
                isInteractingWithItem = false;
            }
        }
    }
}
