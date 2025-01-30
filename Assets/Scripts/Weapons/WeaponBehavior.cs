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

    [Header("Scripts das Armas")]
    private SwordBehavior swordBeh;
    private HammerBehavior hammerBeh;
    private GreatSwordBehavior greatSwordBeh;
    private GreatHammerBehavior greatHammerBeh;
    private StaffBehavior staffBeh;
    private BowBehavior bowBeh;

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
                    swordBeh.ExecuteSwordAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Hammer:
                    hammerBeh.ExecuteHammerAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;        
                case Weapon.WeaponType.GreatSword:
                    greatSwordBeh.ExecuteGreatSwordAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.GreatHammer:
                    greatHammerBeh.ExecuteGreatHammerAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.Staff:
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, chargedAttack, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Bow:
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
                    swordBeh.ExecuteSwordDashAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Hammer:
                    hammerBeh.ExecuteHammerDashAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.GreatSword:
                    greatSwordBeh.ExecuteGreatSwordDashAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.GreatHammer:
                    greatHammerBeh.ExecuteGreatHammerDashAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.Staff: //Staff não possui Dash Attack
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, chargedAttack, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Bow: //Bow não possui Dash Attack
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
                    swordBeh.ExecuteSwordJumpAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Hammer:
                    hammerBeh.ExecuteHammerJumpAttack(canAttack, resourceCost, weaponAnim, leftHandAnim);
                    break;  
                    
                case Weapon.WeaponType.GreatSword:
                    greatSwordBeh.ExecuteGreatSwordJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.GreatHammer:
                    greatHammerBeh.ExecuteGreatHammerJumpAttack(canAttack, resourceCost, weaponAnim);
                    break;

                case Weapon.WeaponType.Staff: //Staff não possui jumpAttack
                    staffBeh.ExecuteStaffAttack(canAttack, resourceCost, chargedAttack, weaponAnim, leftHandAnim);
                    break;

                case Weapon.WeaponType.Bow: //Bow não possui Jump Attack
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
        switch (weaponType)
        {
            case Weapon.WeaponType.GreatHammer:
                greatHammerBeh.UseEarthquake(chargedAttack, resourceCost, canAttack, weaponAnim);
                break;

            case Weapon.WeaponType.GreatSword:
                greatSwordBeh.UseCircularCut(chargedAttack, resourceCost, canAttack, weaponAnim);
                break;

            case Weapon.WeaponType.Staff:
                staffBeh.UseMagicBeam(chargedAttack, resourceCost, weaponAnim);
                break;

            case Weapon.WeaponType.Bow:
                bowBeh.UseConcentration(chargedAttack);
                break;
        }
    }
    public void CanAttack()//Controlada pela propria animação
    {
        canAttack = !canAttack;
    }

    //Funções de Chamada
    public void CallProjectile() //Chama o Projectile no proprio Script da arma
    {
        if(weaponType == Weapon.WeaponType.Staff)
        {
            staffBeh.CallStaffProjectile();
        }
        else if (weaponType == Weapon.WeaponType.Bow)
        {
            bowBeh.CallBowProjectile();
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

    //Seta o tipo de arma que o jogador esta usando
    public void SetWeaponType(Weapon.WeaponType type)
    {
        weaponType = type;

        swordBeh = null;
        hammerBeh = null;
        greatSwordBeh = null;
        greatHammerBeh = null;
        staffBeh = null;
        bowBeh = null;

        switch (weaponType)
        {
            case Weapon.WeaponType.Sword:
                swordBeh = GetComponentInChildren<SwordBehavior>();
                break;

            case Weapon.WeaponType.Hammer:
                hammerBeh = GetComponentInChildren<HammerBehavior>();
                break;

            case Weapon.WeaponType.GreatSword:
                greatSwordBeh = GetComponentInChildren<GreatSwordBehavior>();
                break;

            case Weapon.WeaponType.GreatHammer:
                greatHammerBeh = GetComponentInChildren<GreatHammerBehavior>();
                break;

            case Weapon.WeaponType.Staff:
                staffBeh = GetComponentInChildren<StaffBehavior>();
                break;

            case Weapon.WeaponType.Bow:
                bowBeh = GetComponentInChildren<BowBehavior>();
                break;
        }

        UpdateHandAnimation(); //Atualiza animações das mãos sempre que troca de arma (Para 1Handed ou 2Handed)
    }
    //Controle da animação das mãos para 2Handed
    private void UpdateHandAnimation()
    {
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

