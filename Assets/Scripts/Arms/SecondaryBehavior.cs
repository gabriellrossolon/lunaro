using UnityEngine;
using static Secondary;
using static SecondaryBehavior;

public class SecondaryBehavior : MonoBehaviour
{
    [Header("References")]
    private PlayerStatus playerStatus;
    [HideInInspector] public Animator secondaryAnim;
    public Animator armBonesAnim;

    [Header("Secondary Scripts")]
    private ShieldBehavior shieldBeh;
    private MagicOrbBehavior magicOrbBeh;
    private MagicBookBehavior magicBookBeh;

    [Header("Use Resource Cost (Stamina/Mana")]
    [HideInInspector] public float resourceCost;

    [Header("Controllers")]
    public bool blockInput = false;
    public bool blocking = false;
    public bool skillInput = false;
    private bool canUseSkill = true;

    public enum SecondaryType
    {
        Empty,
        Shield,
        Orb,
        Book
    }
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
                    magicOrbBeh.UseOrbSkill(canUseSkill, resourceCost, secondaryAnim);
                    break;

                case SecondaryType.Book:
                    magicBookBeh.UseBookSkill();
                    break;
            }

            skillInput = false;
        }

        switch (secondaryType) //Esse deve sempre estar sendo verificado
        {
            case SecondaryType.Shield:
                shieldBeh.UseBlock(blocking, secondaryAnim);

                blocking = blockInput && playerStatus.currentStamina >= resourceCost;
                break;
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
            magicOrbBeh.CallSkillProjectilePrefab();
        }
        else if (secondaryType == SecondaryType.Book)
        {
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
    public void SetSecondaryType(Secondary secondary)
    {
        shieldBeh = null;
        magicOrbBeh = null;
        magicBookBeh = null;

        if (secondary == null)
        {
            secondaryType = SecondaryType.Empty;
        }
        else if (secondary is Shield)
        {
            secondaryType = SecondaryType.Shield;
            shieldBeh = GetComponentInChildren<ShieldBehavior>();
        }
        else if (secondary is Orb)
        {
            secondaryType = SecondaryType.Orb;
            magicOrbBeh = GetComponentInChildren<MagicOrbBehavior>();
        }
        else if (secondary is Book)
        {
            secondaryType = SecondaryType.Book;
            magicBookBeh = GetComponentInChildren<MagicBookBehavior>();
        }
    }

}
