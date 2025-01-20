using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class WeaponBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    [HideInInspector] public bool attack;
    public bool canAttack = true;
    private Animator weaponAnim;
    [SerializeField] private Animator leftHandAnim;

    public bool chargedAttack;

    [Header("Only if Melee")]
    private int attackSequence = 1;

    [Header("Only if Ranged")]
    public GameObject projectilePrefab;
    public GameObject beamPrefab;
    public GameObject projectileExitPoint;

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
                // Implementar lógica para Bow
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


    public void CanAttack()
    {
        canAttack = !canAttack;
    }

    public void CallProjectile()
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);

        GameObject newProjectile = Instantiate(projectilePrefab, projectileExitPoint.transform.position, Quaternion.identity);
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

        if(swordBehavior != null)
            swordBehavior.colliderEnabled = !swordBehavior.colliderEnabled;

        if(hammerBehavior != null)
            hammerBehavior.colliderEnabled = !hammerBehavior.colliderEnabled;
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
}

