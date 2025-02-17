using UnityEngine;

public class EnemyExplosionBehavior : MonoBehaviour
{
    public float explosionDamage; //O dano vem ProjectileBehavior, que vem da SecondaryBehavior

    private PlayerStatus playerStatus;
    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.DoDamage(explosionDamage);
        }
    }
}
