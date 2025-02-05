using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class WeaponBehavior : MonoBehaviour
{
    [Header("References")]
    private PlayerStatus playerStatus;
    [SerializeField] private Animator leftHandAnim;
    public Animator armBonesAnim;
    private Animator weaponAnim;
    [SerializeField] private GameObject leftHandGrip;
    [SerializeField] private GameObject holdingObjectL;
    [SerializeField] private GameObject holdingObjectR;

    [Header("Weapon Scripts")]
    private SwordBehavior swordBeh;
    private HammerBehavior hammerBeh;
    private GreatSwordBehavior greatSwordBeh;
    private GreatHammerBehavior greatHammerBeh;
    private StaffBehavior staffBeh;
    private BowBehavior bowBeh;

    [Header("Controllers")]
    public bool attack;
    public bool attackCharging;
    public bool leftBtnInput;
    public bool dashAttack;
    public bool jumpAttack;
    public bool canAttack = true;
    private bool emittingTrail = false;

    [Header("Attack Resource Cost (Stamina/Mana)")]
    public float resourceCost;

    public enum WeaponType
    {
        Empty,
        Sword,
        Hammer,
        GreatSword,
        GreatHammer,
        Staff,
        Bow
    }
    public WeaponType weaponType;

    private void Awake()
    {
        weaponAnim = GetComponent<Animator>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    private void Update()
    {
        ChooseAttack();
        ControlHandsAnimations();
        ControlSpecialAttacks();
    }

    public void ChooseAttack()
    {
        if (attack) // Normal attack
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    swordBeh.ExecuteSwordAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case WeaponType.Hammer:
                    hammerBeh.ExecuteHammerAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case WeaponType.GreatSword:
                    greatSwordBeh.ExecuteGreatSwordAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.GreatHammer:
                    greatHammerBeh.ExecuteGreatHammerAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.Staff:
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, leftBtnInput, weaponAnim, leftHandAnim);
                    break;
            }

            attack = false;
        }

        switch (weaponType)
        {
            case WeaponType.Bow:
                bowBeh.ChargingBowAttack(canAttack, resourceCost, weaponAnim);
                bowBeh.attackCharging = attackCharging;
                break;
        }

        if (dashAttack) // Dash attack
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    swordBeh.ExecuteSwordDashAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case WeaponType.Hammer:
                    hammerBeh.ExecuteHammerDashAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case WeaponType.GreatSword:
                    greatSwordBeh.ExecuteGreatSwordDashAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.GreatHammer:
                    greatHammerBeh.ExecuteGreatHammerDashAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.Staff: // Staff has no Dash Attack
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, leftBtnInput, weaponAnim, leftHandAnim);
                    break;
            }
            dashAttack = false;
        }

        if (jumpAttack) // Jump attack
        {
            switch (weaponType)
            {
                case WeaponType.Sword:
                    swordBeh.ExecuteSwordJumpAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case WeaponType.Hammer:
                    hammerBeh.ExecuteHammerJumpAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case WeaponType.GreatSword:
                    greatSwordBeh.ExecuteGreatSwordJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.GreatHammer:
                    greatHammerBeh.ExecuteGreatHammerJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.Staff: // Staff has no jumpAttack
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, leftBtnInput, weaponAnim, leftHandAnim);
                    break;
            }
            jumpAttack = false;
        }
    }

    // Hand Animations Control
    public void ControlHandsAnimations()
    {
        leftHandAnim.SetBool("holdingBow", weaponType == WeaponType.Bow);
        weaponAnim.SetBool("BowHanded", weaponType == WeaponType.Bow);

        weaponAnim.SetBool("TwoHanded", (weaponType == WeaponType.GreatHammer || weaponType == WeaponType.GreatSword));
        leftHandAnim.SetBool("TwoHanded", (weaponType == WeaponType.GreatHammer || weaponType == WeaponType.GreatSword));

        armBonesAnim.SetBool("isWithBow", weaponType == WeaponType.Bow);
    }

    // Special Attack Control
    public void ControlSpecialAttacks()
    {
        switch (weaponType)
        {
            case WeaponType.GreatHammer:
                greatHammerBeh.UseEarthquake(leftBtnInput, resourceCost, canAttack, weaponAnim);
                break;

            case WeaponType.GreatSword:
                greatSwordBeh.UseCircularCut(leftBtnInput, resourceCost, canAttack, weaponAnim);
                break;

            case WeaponType.Staff:
                staffBeh.UseMagicBeam(leftBtnInput, resourceCost, weaponAnim);
                break;

            case WeaponType.Bow:
                bowBeh.UseConcentration(leftBtnInput);
                break;
        }
    }

    public void CanAttack()//Controlada pela propria animação
    {
        canAttack = !canAttack;
    }
    public void CallProjectile()
    {
        switch (weaponType)
        {
            case WeaponType.Staff:
                staffBeh.CallStaffProjectile();
                break;
            case WeaponType.Bow:
                bowBeh.CallBowProjectile();
                break;
        }
    }

    public void CallSpecialHability(string specialHabilityName) //Chamado pela animação
    {
        switch (specialHabilityName)
        {
            case "Earthquake":
                greatHammerBeh.CallEarthquakePrefab();
                break;

            case "MagicBeam":
                staffBeh.CallBeamPrefab();
                break;

            case "CircularCut":
                greatSwordBeh.CallCircularCutPrefab();
                break;

            default:
                Debug.LogWarning("Habilidade não reconhecida: " + specialHabilityName);
                break;
        }
    }//Chama a habilidade especial pela animação
    public void CallTrail()
    {
        TrailRenderer trailRend = GetComponentInChildren<TrailRenderer>();
        emittingTrail = !emittingTrail;

        if (trailRend != null)
            trailRend.emitting = emittingTrail;
    }//Chama o Trail pela Propria animação
    public void ActiveCollider()
    {
        SwordBehavior swordBehavior = GetComponentInChildren<SwordBehavior>();
        HammerBehavior hammerBehavior = GetComponentInChildren<HammerBehavior>();
        GreatHammerBehavior greatHammerBehavior = GetComponentInChildren<GreatHammerBehavior>();
        GreatSwordBehavior greatSwordBehavior = GetComponentInChildren<GreatSwordBehavior>();

        if (swordBehavior != null)
            swordBehavior.colliderEnabled = !swordBehavior.colliderEnabled;

        if (hammerBehavior != null)
            hammerBehavior.colliderEnabled = !hammerBehavior.colliderEnabled;

        if (greatHammerBehavior != null)
            greatHammerBehavior.colliderEnabled = !greatHammerBehavior.colliderEnabled;

        if (greatSwordBehavior != null)
            greatSwordBehavior.colliderEnabled = !greatSwordBehavior.colliderEnabled;
    }//Ativa o Colisor da Arma pela propria animação

    // Mana and Stamina Consumption
    public void ConsumeMana() // Typically called by animation
    {
        playerStatus.SubtractMana(resourceCost);
    }

    public void ConsumeStamina() // Typically called by animation
    {
        playerStatus.SubtractStamina(resourceCost);
    }

    // Set Weapon Type
    public void SetWeaponType(Weapon weapon)
    {
        swordBeh = null;
        hammerBeh = null;
        greatSwordBeh = null;
        greatHammerBeh = null;
        staffBeh = null;
        bowBeh = null;

        if (weapon == null)
        {
            weaponType = WeaponType.Empty;
        }
        else if (weapon is Sword)
        {
            weaponType = WeaponType.Sword;
            swordBeh = GetComponentInChildren<SwordBehavior>();
        }
        else if (weapon is Hammer)
        {
            weaponType = WeaponType.Hammer;
            hammerBeh = GetComponentInChildren<HammerBehavior>();
        }
        else if (weapon is GreatSword)
        {
            weaponType = WeaponType.GreatSword;
            greatSwordBeh = GetComponentInChildren<GreatSwordBehavior>();
        }
        else if (weapon is GreatHammer)
        {
            weaponType = WeaponType.GreatHammer;
            greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
        }
        else if (weapon is Staff)
        {
            weaponType = WeaponType.Staff;
            staffBeh = GetComponentInChildren<StaffBehavior>();
        }
        else if (weapon is Bow)
        {
            weaponType = WeaponType.Bow;
            bowBeh = GetComponentInChildren<BowBehavior>();
        }

        UpdateHandAnimation();
    }

    // Hand Animation Update for Two Handed Weapons
    private void UpdateHandAnimation()
    {
        if (weaponType == WeaponType.GreatSword || weaponType == WeaponType.GreatHammer)
        {
            Invoke("UpdateLeftGripToHandR", 0.2f);  // Removing "nameof"
        }
        else
        {
            CancelInvoke("UpdateLeftGripToHandR");  // Removing "nameof"
            leftHandGrip.transform.SetParent(holdingObjectL.transform);
        }
    }

    private void UpdateLeftGripToHandR()
    {
        leftHandGrip.transform.SetParent(holdingObjectR.transform);
    }
}
