using UnityEngine;

public class ClimbableBehavior : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject playerArea;

    public void Interact()
    {
        Vector3 climbablePos = new Vector3(playerArea.transform.position.x, PlayerManager.Instance.transform.position.y, playerArea.transform.position.z);
        PlayerManager.Instance.transform.position = climbablePos;
        PlayerManager.Instance.GetComponent<PlayerFSM>()._fsm.SetState("Climb");
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerManager.Instance.GetComponent<PlayerFSM>()._fsm.SetState("Idle");
    }
}
