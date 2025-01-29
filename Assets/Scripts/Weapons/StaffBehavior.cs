using UnityEngine;

public class StaffBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public GameObject projectileExit;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        //playerStatus.ControlBaseDamage(damage);
    }

    private void OnDestroy()
    {
        //    if(playerStatus != null)
        //        playerStatus.ControlBaseDamage(-damage);
    }
}
