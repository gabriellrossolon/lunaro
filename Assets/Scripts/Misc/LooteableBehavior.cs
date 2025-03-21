using UnityEngine;

public class LooteableBehavior : MonoBehaviour, IInteractable
{
    [Header("References")]
    private GameObject player;
    private PlayerStatus playerStatus;

    [Header("Configs")]
    public float maxDistance = 2f;

    [Header("Looting")]
    public float gold;

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;
        playerStatus = player.GetComponent<PlayerStatus>();

        if (GetComponentInParent<EnemyStats>() is EnemyStats stats) //Caso o looteable em questão seja um Enemy
            gold = stats.goldCarryed;
    }

    public void Interact()
    {
        playerStatus.ControlGold(gold);
        DisableBehavior();
    }

    private void DisableBehavior()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (GetComponent<Outline>() is Outline outline)
            outline.enabled = false;
        enabled = false;
    }
}
