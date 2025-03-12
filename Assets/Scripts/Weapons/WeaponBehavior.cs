using UnityEngine;

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
    [SerializeField] private SwordBehavior swordBeh;
    private HammerBehavior hammerBeh;
    private GreatSwordBehavior greatSwordBeh;
    private GreatHammerBehavior greatHammerBeh;
    private GreatAxeBehavior greatAxeBeh;
    private StaffBehavior staffBeh;
    private BowBehavior bowBeh;

    [Header("Skill Equipped")]
    public string skillType;

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
        GreatAxe,
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

    private void ChooseAttack()
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

                case WeaponType.GreatAxe:
                    greatAxeBeh.ExecuteGreatAxeAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.Staff:
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, leftBtnInput, weaponAnim, leftHandAnim);
                    break;
            }

            attack = false;
        }

        switch (weaponType) //Somente bow funciona com Input diferente, o botão precisa ser pressionado e solto para atacar
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

                case WeaponType.GreatAxe:
                    greatAxeBeh.ExecuteGreatAxeDashAttack(canAttack, resourceCost, weaponAnim);
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

                case WeaponType.GreatAxe:
                    greatAxeBeh.ExecuteGreatAxeJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case WeaponType.Staff: // Staff has no jumpAttack
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, leftBtnInput, weaponAnim, leftHandAnim);
                    break;
            }
            jumpAttack = false;
        }
    }

    // Hand Animations Control
    private void ControlHandsAnimations()
    {
        //Controla jeito que a mão fica caso esteja com arco
        leftHandAnim.SetBool("holdingBow", weaponType == WeaponType.Bow);
        weaponAnim.SetBool("BowHanded", weaponType == WeaponType.Bow);
        armBonesAnim.SetBool("isWithBow", weaponType == WeaponType.Bow);

        //Controla jeito que a mão fica caso esteja com arma 2Handed
        weaponAnim.SetBool("TwoHanded", (weaponType == WeaponType.GreatHammer || weaponType == WeaponType.GreatSword || weaponType == WeaponType.GreatAxe));
        leftHandAnim.SetBool("TwoHanded", (weaponType == WeaponType.GreatHammer || weaponType == WeaponType.GreatSword || weaponType == WeaponType.GreatAxe));
    }

    // Special Attack Control
    private void ControlSpecialAttacks()
    {
        switch (weaponType) //Esse case controla a chamada da Animação que chama o event CallSpecialHability
        {
            case WeaponType.Sword:
                if (swordBeh == null) Debug.Log("Sword Behavior nao encontrado");
                swordBeh.UseSwordSkill(leftBtnInput, resourceCost, canAttack, weaponAnim);
                break;

            case WeaponType.GreatHammer:
                greatHammerBeh.UseGreatHammerSkill(leftBtnInput, resourceCost, canAttack, weaponAnim);
                break;

            case WeaponType.GreatSword:
                greatSwordBeh.UseGreatSwordSkill(leftBtnInput, resourceCost, canAttack, weaponAnim);
                break;

            case WeaponType.GreatAxe:
                greatAxeBeh.UseGreatAxeSkill(leftBtnInput, resourceCost, canAttack, weaponAnim);
                break;

            case WeaponType.Staff:
                staffBeh.UseMagicBeam(leftBtnInput, resourceCost, weaponAnim);
                break;

            case WeaponType.Bow:
                bowBeh.UseConcentration(leftBtnInput);
                break;
        }   
    }

    public bool ChargingAnySkill()
    {
        if (weaponAnim.GetBool("ChargingAttack"))
            return true;
        else 
            return false;
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

    public void CallSpecialHability() //Chama a habilidade especial pela animação
    {
        switch (skillType)
        {
            case "AirCut":
                swordBeh.CallAirCutPrefab();
                break;

            case "Earthquake":
                greatHammerBeh.CallEarthquakePrefab();
                break;

            case "MagicBeam":
                staffBeh.CallBeamPrefab();
                break;

            case "CircularCut":
                greatSwordBeh.CallCircularCutPrefab();
                break;

            case "Fissure":
                greatAxeBeh.CallFissurePrefab();
                break;

            default:
                Debug.LogWarning("Habilidade não reconhecida: " + skillType);
                break;
        }
    }

    public void CallTrail() //Chama o Trail pela Propria animação
    {
        emittingTrail = !emittingTrail;

        TrailRenderer trailRend = GetComponentInChildren<TrailRenderer>();
        if (trailRend != null)
            trailRend.emitting = emittingTrail;
    }

    public void CallSwoosh(float pitch)
    {
        float minPitch = pitch - 0.1f;
        float maxPitch = pitch + 0.1f;
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(minPitch, maxPitch);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
    }

    public void ActiveCollider() //Ativa o Colisor da Arma pela propria animação
    {
        SwordBehavior swordBehavior = GetComponentInChildren<SwordBehavior>();
        HammerBehavior hammerBehavior = GetComponentInChildren<HammerBehavior>();
        GreatHammerBehavior greatHammerBehavior = GetComponentInChildren<GreatHammerBehavior>();
        GreatSwordBehavior greatSwordBehavior = GetComponentInChildren<GreatSwordBehavior>();
        GreatAxeBehavior greatAxeBehavior = GetComponentInChildren<GreatAxeBehavior>();

        if (swordBehavior != null)
            swordBehavior.colliderEnabled = !swordBehavior.colliderEnabled;

        if (hammerBehavior != null)
            hammerBehavior.colliderEnabled = !hammerBehavior.colliderEnabled;

        if (greatHammerBehavior != null)
            greatHammerBehavior.colliderEnabled = !greatHammerBehavior.colliderEnabled;

        if (greatSwordBehavior != null)
            greatSwordBehavior.colliderEnabled = !greatSwordBehavior.colliderEnabled;

        if (greatAxeBehavior != null)
            greatAxeBehavior.colliderEnabled = !greatAxeBehavior.colliderEnabled;
    }

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
        greatAxeBeh = null;
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
        else if (weapon is GreatAxe)
        {
            weaponType = WeaponType.GreatAxe;
            greatAxeBeh = GetComponentInChildren<GreatAxeBehavior>();
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
        if (weaponType == WeaponType.GreatSword || weaponType == WeaponType.GreatHammer || weaponType == WeaponType.GreatAxe)
        {
            Invoke("UpdateLeftGripToHandR", 0.2f);
        }
        else
        {
            CancelInvoke("UpdateLeftGripToHandR"); 
            leftHandGrip.transform.SetParent(holdingObjectL.transform);
        }
    }

    private void UpdateLeftGripToHandR()
    {
        leftHandGrip.transform.SetParent(holdingObjectR.transform);
    }
}
