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

    [HideInInspector] public bool skillInputReceived = false;

    public WeaponBehavior weaponBeh;

    private bool canUseSkill = true;
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
        else if (secondaryType == SecondaryType.Orb)
        {
            if (skillInputReceived && canUseSkill && playerStatus.currentMana >= resourceCost)
            {
                secondaryAnim.SetTrigger("orbAttack");
            }
            skillInputReceived = false;
        }

        armBonesAnim.SetBool("isWithOrb", secondaryType == SecondaryType.Orb);
        armBonesAnim.SetBool("isWithShield", secondaryType == SecondaryType.Shield);
        armBonesAnim.SetBool("isWithBook", secondaryType == SecondaryType.Book);
    }

    public void CallProjectileInLeft()
    {
        var magicOrbBeh = GetComponentInChildren<MagicOrbBehavior>();
        if(magicOrbBeh != null)
        {
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);

            GameObject newProjectile = Instantiate(magicOrbBeh.skillPrefab, magicOrbBeh.exitPoint.transform.position, Quaternion.identity);
            ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
            projectileScript.projectileDamage = magicOrbBeh.damage;
            projectileScript.SetDirection(Camera.main.transform.forward);
        }
    }

    public void CanUseSkill()
    {
        canUseSkill = !canUseSkill;
    }

    public void ConsumeMana()
    {
        playerStatus.SubtractMana(resourceCost);
    }

    public void ConsumeStamina()
    {
        playerStatus.SubtractStamina(resourceCost);
    }


    //Funções do Weapon Behavior
    //public void CanAttack()
    //{
    //    weaponBeh.CanAttack();
    //}

    //public void CallProjectile()
    //{
    //    weaponBeh.CallProjectile();
    //}

    //public void ConsumeStamina()
    //{
    //    weaponBeh.ConsumeStamina();
    //}
}
