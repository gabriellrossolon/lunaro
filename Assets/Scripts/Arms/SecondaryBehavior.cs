using UnityEngine;
using static Secondary;

public class SecondaryBehavior : MonoBehaviour
{
    [Header("References")]
    private PlayerStatus playerStatus;
    [HideInInspector] public Animator secondaryAnim;
    public Animator armBonesAnim;

    [Header("Use Resource Cost (Stamina/Mana")]
    [HideInInspector] public float resourceCost;

    [Header("Controllers")]
    public bool blockInput = false;
    public bool blocking = false;
    public bool skillInput = false;
    private bool canUseSkill = true;

    public SecondaryType secondaryType;

    private void Start()
    {
        secondaryAnim = GetComponent<Animator>();
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        ChooseAction();
        ControlAnimations();
    }

    private void ChooseAction()
    {
        if (skillInput)
        {
            switch (secondaryType)
            {
                case SecondaryType.Orb:
                    var magicOrbBeh = GetComponentInChildren<MagicOrbBehavior>();
                    magicOrbBeh.UseOrbSkill(canUseSkill, resourceCost, secondaryAnim);
                    break;

                case SecondaryType.Book:
                    var magicBookBeh = GetComponentInChildren<MagicBookBehavior>();
                    magicBookBeh.UseBookSkill();
                    break;
            }

            skillInput = false;
        }

        if (blockInput)
        {
            switch (secondaryType)
            {
                case SecondaryType.Shield:
                    var shieldBeh = GetComponentInChildren<ShieldBehavior>();
                    shieldBeh.UseBlock(blocking, secondaryAnim);

                    blocking = blockInput && playerStatus.currentStamina >= resourceCost;
                    break;
            }
        }
    }

    private void ControlAnimations()
    {
        armBonesAnim.SetBool("isWithOrb", secondaryType == SecondaryType.Orb);
        armBonesAnim.SetBool("isWithShield", secondaryType == SecondaryType.Shield);
        armBonesAnim.SetBool("isWithBook", secondaryType == SecondaryType.Book);
    }


    public void CallProjectileInLeft()
    {
        if (secondaryType == SecondaryType.Orb)
        {
            var magicOrbBeh = GetComponentInChildren<MagicOrbBehavior>();
            magicOrbBeh.CallSkillProjectilePrefab();
        }
        if(secondaryType == SecondaryType.Book)
        {
            var magicBookBeh = GetComponentInChildren<MagicBookBehavior>();
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

    //Controll Weapon Type
    public void SetSecondaryType(Secondary.SecondaryType type)
    {
        secondaryType = type;
    }
}
