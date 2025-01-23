using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class WeaponBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    [HideInInspector] public bool attack;
    public bool canAttack = true;
    private Animator weaponAnim;
    [Header("Left Hand References")]
    [SerializeField] private Animator leftHandAnim;
    [SerializeField] private GameObject leftHandGrip;
    [SerializeField] private GameObject holdingObjectL;
    [SerializeField] private GameObject holdingObjectR;

    public bool chargedAttack;

    [Header("Only if Melee")]
    private int attackSequence = 1;

    [Header("Only if Ranged")]
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public GameObject beamPrefab;
    [HideInInspector] public GameObject projectileExitPoint;

    private GameObject currentBeam;

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
        if (attack)
        {
            if (weaponType == Weapon.WeaponType.Staff)
            {
                ExecuteStaffAttack();
            }
            else if (weaponType == Weapon.WeaponType.Sword)
            {
                ExecuteSwordAttack();
            }
            else if(weaponType == Weapon.WeaponType.Hammer)
            {
                ExecuteHammerAttack();
            }
            else if (weaponType == Weapon.WeaponType.Bow)
            {
                ExecuteBowAttack();
            }
            else if (weaponType == Weapon.WeaponType.GreatHammer)
            {
                ExecuteGreatHammerAttack();
            }
            else if (weaponType == Weapon.WeaponType.GreatSword)
            {
                ExecuteGreatSwordAttack();
            }
            attack = false;
        }

        if (weaponType == Weapon.WeaponType.Staff)
        {
            if (chargedAttack && playerStatus.currentMana >= resourceCost)
            {
                if (!weaponAnim.GetBool("ChargingAttack"))
                {
                    StartCoroutine(HandleChargingAttack());
                }
                weaponAnim.SetBool("ChargingAttack", true);
            }
            else
            {
                weaponAnim.SetBool("ChargingAttack", false);
            }
        }

        leftHandAnim.SetBool("holdingBow", weaponType == Weapon.WeaponType.Bow);
        weaponAnim.SetBool("BowHanded", weaponType == Weapon.WeaponType.Bow);

        weaponAnim.SetBool("TwoHanded", (weaponType == Weapon.WeaponType.GreatHammer || weaponType == Weapon.WeaponType.GreatSword));
        leftHandAnim.SetBool("TwoHanded", (weaponType == Weapon.WeaponType.GreatHammer || weaponType == Weapon.WeaponType.GreatSword));
        //UpdateLeftHandGripParent();
    }

    private void ExecuteStaffAttack()
    {
        if (canAttack && playerStatus.currentMana >= resourceCost && !chargedAttack)
        {
            weaponAnim.SetTrigger("staffAttack");
            string retreatTrigger = "retreat" + Random.Range(1, 4); 
            leftHandAnim.SetTrigger(retreatTrigger);
        }     
    }

    private void ExecuteBowAttack()
    {
        if (canAttack && playerStatus.currentMana >= resourceCost && !chargedAttack)
        {
            weaponAnim.SetTrigger("bowAttack");
            GetComponentInChildren<BowBehavior>().GetComponent<Animator>().SetTrigger("attack");
        }
    }

    public void ExecuteSwordAttack()
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("swordAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("swordAttack2");
                    attackSequence++;
                    break;
                case 3:
                    weaponAnim.SetTrigger("swordAttack3");
                    attackSequence++; 
                    break;
                case 4:
                    weaponAnim.SetTrigger("swordAttack4");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1; 
                    break;
            }
            string retreatTrigger = "retreat" + Random.Range(1, 4);
            leftHandAnim.SetTrigger(retreatTrigger);

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteHammerAttack()
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("hammerAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("hammerAttack2");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }
            string retreatTrigger = "retreat" + Random.Range(1, 4);
            leftHandAnim.SetTrigger(retreatTrigger);

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteGreatHammerAttack()
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("greatHammerAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("greatHammerAttack2");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteGreatSwordAttack()
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("greatSwordAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("greatSwordAttack2");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void CanAttack()
    {
        canAttack = !canAttack;
    }

    public void CallProjectile()
    {
        if(weaponType == Weapon.WeaponType.Staff)
        {
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);
        }
        else if (weaponType == Weapon.WeaponType.Bow)
        {
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
            if(SFXManager.Instance.bowSwooshSound != null)
                SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.bowSwooshSound);
        }

        GameObject newProjectile = Instantiate(projectilePrefab, projectileExitPoint.transform.position, transform.rotation);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        projectileScript.SetDirection(transform.forward);
    }

    public void CallBurst()
    {
        if (currentBeam != null)
        {
            Destroy(currentBeam);
            currentBeam = null;
        }
        else
        {
            currentBeam = Instantiate(beamPrefab, projectileExitPoint.transform.position, projectileExitPoint.transform.rotation, projectileExitPoint.transform);

            VisualEffect projectileEffect = currentBeam.GetComponentInChildren<VisualEffect>();
            if (projectileEffect != null)
            {
                projectileEffect.Play();
            }
        }
    }

    private IEnumerator HandleChargingAttack()
    {
        while (chargedAttack)
        {
            if (playerStatus.currentMana >= resourceCost)
            {
                ConsumeManaControlled(resourceCost / 5);
                playerStatus.canRegenMana = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
        playerStatus.canRegenMana = true;
    }

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
    }

    public void ConsumeMana()
    {   
        playerStatus.SubtractMana(resourceCost);
    }

    public void ConsumeManaControlled(float value)
    {
        playerStatus.SubtractMana(value);
    }

    public void ConsumeStamina()
    {
        playerStatus.SubtractStamina(resourceCost);
    }

    //
    private void UpdateLeftGripToHandR()
    {
        leftHandGrip.transform.SetParent(holdingObjectR.transform);
    }

    private void UpdateLeftGripToHandL()
    {
        leftHandGrip.transform.SetParent(holdingObjectL.transform);
    }

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
            UpdateLeftGripToHandL();
        }
    }
}

