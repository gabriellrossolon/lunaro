using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private float outlineMaxDistance = 5f;

    private Transform cameraTransform;
    private bool isInteractingWithItem;

    private void Start()
    {
        cameraTransform = PlayerComponents.Instance.playerCamera.transform;
    }

    private void Update()
    {
        CheckForInteractions();
        DetectOutlineObjects();
    }

    private void CheckForInteractions()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, interactableLayer))
        {
            HudManager.Instance.warnBtn.SetActive(true);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && PlayerComponents.Instance.inputHandler.interactInput)
            {
                interactable.Interact();
                PlayerComponents.Instance.inputHandler.interactInput = false;
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

    private void DetectOutlineObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(cameraTransform.position, outlineMaxDistance, interactableLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            Outline[] outlines = hitCollider.GetComponentsInChildren<Outline>(true); // Pega os outlines no próprio objeto e nos filhos

            foreach (Outline outline in outlines)
            {
                float distance = Vector3.Distance(cameraTransform.position, hitCollider.transform.position);
                outline.enabled = distance <= outlineMaxDistance;
            }
        }
    }

}
