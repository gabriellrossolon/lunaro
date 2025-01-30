using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    private Collider coll;
    private PlayerStatus playerStatus;
    private SecondaryBehavior secondBeh;

    public AudioClip blockedAttackSound;
    public float resourceCost;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        secondBeh = GetComponentInParent<SecondaryBehavior>();
    }
    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        resourceCost = secondBeh.resourceCost;
    }

    private void Update()
    {
        coll.enabled = secondBeh.blocking;
    }

    //Ações básicas
    public void UseBlock(bool blocking, Animator secondaryAnim)
    {
        secondaryAnim.SetBool("isBlocking", blocking);
    }

    public void ConsumeStamina()
    {
        playerStatus.SubtractStamina(resourceCost);
    }
}

