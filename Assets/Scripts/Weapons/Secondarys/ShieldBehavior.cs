using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    private Collider coll;

    public AudioClip blockedAttackSound;
    public float resourceCost;
    public int blockPercent;

    private void Start()
    {
        coll = GetComponent<Collider>();
        resourceCost = PlayerComponents.Instance.secondaryBehavior.resourceCost;
    }

    private void Update()
    {
        coll.enabled = PlayerComponents.Instance.secondaryBehavior.blocking;
    }

    //Ações básicas
    public void UseBlock(bool blocking, Animator secondaryAnim)
    {
        secondaryAnim.SetBool("isBlocking", blocking);
    }

    public void ConsumeStamina()
    {
        PlayerComponents.Instance.playerStatus.SubtractStamina(resourceCost);
    }
}

