using UnityEngine;

public class StaffBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public GameObject projectileExit;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        playerStatus.ControlDamage(damage);
    }

    private void OnDestroy()
    {
        playerStatus.ControlDamage(-damage);
    }
}
