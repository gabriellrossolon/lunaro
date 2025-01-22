using UnityEngine;

public class SecondaryBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public Secondary.SecondaryType secondaryType;
    [HideInInspector] public Animator secondaryAnim;
    [HideInInspector] public Animator armBonesAnim;

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
        if (secondaryType == Secondary.SecondaryType.Shield)
        {
            secondaryAnim.SetBool("isBlocking", blocking);
            if (blockInputReceived && playerStatus.currentStamina >= resourceCost)
            {
                blocking = true;
            }
            else
            {
                blocking = false;
            }
        }

        if (secondaryType == Secondary.SecondaryType.Orb)
        {
            armBonesAnim.SetBool("isWithOrb", true);
            armBonesAnim.SetBool("isWithShield", false);
            armBonesAnim.SetBool("isWithBook", false);
        }
        else if (secondaryType == Secondary.SecondaryType.Shield)
        {
            armBonesAnim.SetBool("isWithShield", true);
            armBonesAnim.SetBool("isWithOrb", false);
            armBonesAnim.SetBool("isWithBook", false);
        }
        else if (secondaryType == Secondary.SecondaryType.Book)
        {
            armBonesAnim.SetBool("isWithShield", false);
            armBonesAnim.SetBool("isWithOrb", false);
            armBonesAnim.SetBool("isWithBook", true);
        }
        else
        {
            armBonesAnim.SetBool("isWithOrb", false);
            armBonesAnim.SetBool("isWithShield", false);
            armBonesAnim.SetBool("isWithBook", false);
        }
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
