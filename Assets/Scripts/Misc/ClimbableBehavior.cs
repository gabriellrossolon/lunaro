using UnityEngine;

public class ClimbableBehavior : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject playerArea;

    public void Interact()
    {
        Vector3 climbablePos = new Vector3(playerArea.transform.position.x, PlayerComponents.Instance.playerObject.transform.position.y, playerArea.transform.position.z);
        PlayerComponents.Instance.playerObject.transform.position = climbablePos;
        PlayerComponents.Instance.fsm.FSM.SetState("Climb");
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerComponents.Instance.fsm.FSM.SetState("Idle");
    }
}
