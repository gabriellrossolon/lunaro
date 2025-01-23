using UnityEngine;
using static Secondary;

public class SecondaryBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public SecondaryType secondaryType;
    [HideInInspector] public Animator secondaryAnim;
    public Animator armBonesAnim;

    [HideInInspector] public AudioClip blockSound;
    [HideInInspector] public float resourceCost;

    [HideInInspector] public bool blockInputReceived = false;
    [HideInInspector] public bool blocking = false;

    public WeaponBehavior weaponBeh;

    private void Start()
    {
        secondaryAnim = GetComponent<Animator>();
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        if (secondaryType == SecondaryType.Shield)
        {
            secondaryAnim.SetBool("isBlocking", blocking);
            blocking = blockInputReceived && playerStatus.currentStamina >= resourceCost;
        }

        armBonesAnim.SetBool("isWithOrb", secondaryType == SecondaryType.Orb);
        armBonesAnim.SetBool("isWithShield", secondaryType == SecondaryType.Shield);
        armBonesAnim.SetBool("isWithBook", secondaryType == SecondaryType.Book);
    }

    //Funções do Weapon Behavior
    public void CanAttack()
    {
        weaponBeh.CanAttack();
    }

    public void CallProjectile()
    {
        weaponBeh.CallProjectile();
    }

    public void ConsumeStamina()
    {
        weaponBeh.ConsumeStamina();
    }
}
