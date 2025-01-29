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

    [Header("Controllers")]
    public bool attack;
    public bool chargedAttack;
    public bool dashAttack;
    public bool jumpAttack;
    public bool canAttack = true;
    private bool emittingTrail = false;

    [Header("Attack Resource Cost (Stamina/Mana)")]
    public float resourceCost;

    [Header("Weapon Type Equipped")]
    public Weapon.WeaponType weaponType;

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
        if (attack) //Caso seja ataque normal
        {
            switch (weaponType)
            {
                case Weapon.WeaponType.Sword:
                    var swordBeh = GetComponentInChildren<SwordBehavior>();
                    swordBeh.ExecuteSwordAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Hammer:
                    var hammerBeh = GetComponentInChildren<HammerBehavior>();
                    hammerBeh.ExecuteHammerAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;        
                case Weapon.WeaponType.GreatSword:
                    var greatSwordBeh = GetComponentInChildren<GreatSwordBehavior>();
                    greatSwordBeh.ExecuteGreatSwordAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.GreatHammer:
                    var greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
                    greatHammerBeh.ExecuteGreatHammerAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.Staff:
                    var staffBeh = GetComponentInChildren<StaffBehavior>();
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, chargedAttack, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Bow:
                    var bowBeh = GetComponentInChildren<BowBehavior>();
                    bowBeh.ExecuteBowAttack(canAttack, resourceCost, weaponAnim);
                    break;
            }

            attack = false;
        }

        if(dashAttack) //Caso seja dash attack
        {
            switch (weaponType)
            {
                case Weapon.WeaponType.Sword:
                    var swordBeh = GetComponentInChildren<SwordBehavior>();
                    swordBeh.ExecuteSwordDashAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Hammer:
                    var hammerBeh = GetComponentInChildren<HammerBehavior>();
                    hammerBeh.ExecuteHammerDashAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.GreatSword:
                    var greatSwordBeh = GetComponentInChildren<GreatSwordBehavior>();
                    greatSwordBeh.ExecuteGreatSwordDashAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.GreatHammer:
                    var greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
                    greatHammerBeh.ExecuteGreatHammerDashAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.Staff: //Staff não possui Dash Attack
                    var staffBeh = GetComponentInChildren<StaffBehavior>();
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, chargedAttack, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Bow: //Bow não possui Dash Attack
                    var bowBeh = GetComponentInChildren<BowBehavior>();
                    bowBeh.ExecuteBowAttack(canAttack, resourceCost, weaponAnim);
                    break;
            }
            dashAttack = false;
        }

        if (jumpAttack) //Caso seja jump attack
        {
            switch (weaponType)
            {
                case Weapon.WeaponType.Sword:
                    var swordBeh = GetComponentInChildren<SwordBehavior>();
                    swordBeh.ExecuteSwordJumpAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Hammer:
                    var hammerBeh = GetComponentInChildren<HammerBehavior>();
                    hammerBeh.ExecuteHammerJumpAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;  
                    
                case Weapon.WeaponType.GreatSword:
                    var greatSwordBeh = GetComponentInChildren<GreatSwordBehavior>();
                    greatSwordBeh.ExecuteGreatSwordJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.GreatHammer:
                    var greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
                    greatHammerBeh.ExecuteGreatHammerJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.Staff: //Staff não possui jumpAttack
                    var staffBeh = GetComponentInChildren<StaffBehavior>();
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, chargedAttack, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Bow: //Bow não possui Jump Attack
                    var bowBeh = GetComponentInChildren<BowBehavior>();
                    bowBeh.ExecuteBowAttack(canAttack, resourceCost, weaponAnim);
                    break;
            }
            jumpAttack = false;
        }
    }

    //Controladores
    public void ControlHandsAnimations()
    {
        leftHandAnim.SetBool("holdingBow", weaponType == Weapon.WeaponType.Bow);
        weaponAnim.SetBool("BowHanded", weaponType == Weapon.WeaponType.Bow);

        weaponAnim.SetBool("TwoHanded", (weaponType == Weapon.WeaponType.GreatHammer || weaponType == Weapon.WeaponType.GreatSword));
        leftHandAnim.SetBool("TwoHanded", (weaponType == Weapon.WeaponType.GreatHammer || weaponType == Weapon.WeaponType.GreatSword));

        armBonesAnim.SetBool("isWithBow", weaponType == Weapon.WeaponType.Bow);
    }
    public void ControlSpecialAttacks()
    {
        if (weaponType == Weapon.WeaponType.GreatHammer)
        {
            var greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
            greatHammerBeh.UseEarthquake(chargedAttack, resourceCost, canAttack, weaponAnim);
        }
        if (weaponType == Weapon.WeaponType.GreatSword)
        {
            var greatSwordrBeh = GetComponentInChildren<GreatSwordBehavior>();
            greatSwordrBeh.UseCircularCut(chargedAttack, resourceCost, canAttack, weaponAnim);
        }
        if (weaponType == Weapon.WeaponType.Staff)
        {
            var staffBeh = GetComponentInChildren<StaffBehavior>();
            staffBeh.UseMagicBeam(chargedAttack, resourceCost, weaponAnim);
        }
        if (weaponType == Weapon.WeaponType.Bow)
        {
            var bowBeh = GetComponentInChildren<BowBehavior>();
            bowBeh.UseConcentration(chargedAttack);
        } 
    }
    public void CanAttack()
    {
        canAttack = !canAttack;
    }//Controlada pela propria animação

    //Funções de Chamada
    public void CallProjectile() //Chama o Projectile no proprio Script da arma
    {
        if(weaponType == Weapon.WeaponType.Staff)
        {
            var staffBeh = GetComponentInChildren<StaffBehavior>();
            staffBeh.CallStaffProjectile();
        }
        else if (weaponType == Weapon.WeaponType.Bow)
        {
            var bowBeh = GetComponentInChildren<BowBehavior>();
            bowBeh.CallBowProjectile();
        }
    }
    public void CallSpecialHability(string specialHabilityName)
    {
        switch (specialHabilityName)
        {
            case "Earthquake":
                var greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
                greatHammerBeh.CallEarthquakePrefab();
                break;

            case "MagicBeam":
                var staffBeh = GetComponentInChildren<StaffBehavior>();
                staffBeh.CallBeamPrefab();
                break;

            case "CircularCut":
                var greatSwordBeh = GetComponentInChildren<GreatSwordBehavior>();
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

        if(trailRend != null)
            trailRend.emitting = emittingTrail;
    }//Chama o Trail pela Propria animação
    public void ActiveCollider()
    {
        SwordBehavior swordBehavior = GetComponentInChildren<SwordBehavior>();
        HammerBehavior hammerBehavior = GetComponentInChildren<HammerBehavior>();
        GreatHammerBehavior greatHammerBehavior = GetComponentInChildren<GreatHammerBehavior>();
        GreatSwordBehavior greatSwordBehavior = GetComponentInChildren<GreatSwordBehavior>();

        if(swordBehavior != null)
            swordBehavior.colliderEnabled = !swordBehavior.colliderEnabled;

        if(hammerBehavior != null)
            hammerBehavior.colliderEnabled = !hammerBehavior.colliderEnabled;

        if(greatHammerBehavior != null)
            greatHammerBehavior.colliderEnabled = !greatHammerBehavior.colliderEnabled;

        if (greatSwordBehavior != null)
            greatSwordBehavior.colliderEnabled = !greatSwordBehavior.colliderEnabled;
    }//Ativa o Colisor da Arma pela propria animação

    public void ConsumeMana() //Geralmente chamado pelo próprio Animation
    {   
        playerStatus.SubtractMana(resourceCost);
    }
    public void ConsumeStamina() //Geralmente chamado pelo próprio Animation
    {
        playerStatus.SubtractStamina(resourceCost);
    }

    //Controle da animação das mãos para 2Handed
    public void SetWeaponType(Weapon.WeaponType type)
    {
        weaponType = type;

        if (weaponType == Weapon.WeaponType.GreatSword || weaponType == Weapon.WeaponType.GreatHammer)
        {
            Invoke(nameof(UpdateLeftGripToHandR), 0.2f);
        }
        else
        {
            CancelInvoke(nameof(UpdateLeftGripToHandR));
            leftHandGrip.transform.SetParent(holdingObjectL.transform);
        }
    }
    private void UpdateLeftGripToHandR()
    {
        leftHandGrip.transform.SetParent(holdingObjectR.transform);
    }
}

