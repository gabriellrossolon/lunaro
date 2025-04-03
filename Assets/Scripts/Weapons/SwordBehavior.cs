using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    private CooldownController cooldownController;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;

    public GameObject weaponSkillPrefab;
    public float skillDamageMultiplier;

    private int attackSequence = 1;

    private GameObject activedPowerAura;


    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
        cooldownController = GetComponentInParent<CooldownController>();

        GameObject swordAura = transform.Find("ActivedPowerAura")?.gameObject;
        if (swordAura != null)
            activedPowerAura = swordAura;
    }

    private void Start()
    {
        PlayerComponents.Instance.playerStatus = PlayerComponents.Instance.playerManager.GetComponent<PlayerStatus>();
        PlayerComponents.Instance.weaponBehavior = GetComponentInParent<WeaponBehavior>();
    }

    private void Update()
    {
        weaponColl.enabled = colliderEnabled;
    }

    //Attack Variations (Only Basic Attacks)

    public void ExecuteSwordAttack(bool canAttack, float resourceCost, Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack)
        {
            if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("stamina");
                return;
            }
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
        }
    }

    public void ExecuteSwordDashAttack(bool canAttack, float resourceCost, Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack)
        {
            if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("stamina");
                return;
            }
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
        }
    }

    public void ExecuteSwordJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack)
        {
            if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("stamina");
                return;
            }
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
        }
    }

    public void UseSwordSkill(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill() || !chargedAttack || !canAttack) return;
        if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
        {
            HudManager.Instance.InsuficientResourceWarn("stamina");
            return;
        }
        if (InventoryManager.Instance.equippedWeapon.weaponSkill == null)
        {
            Debug.Log("Arma não possui Skill");
            return;
        }

        var skillType = InventoryManager.Instance.equippedWeapon.weaponSkill.skillType;

        switch (skillType)
        {
            case Skill.SkillType.None:
                Debug.Log("A arma possui uma Skill, porém a Skill não possui um tipo");
                break;

            case Skill.SkillType.AirCut:
                weaponAnim.SetTrigger("swordSpecial");

                activedPowerAura.gameObject.SetActive(!activedPowerAura.gameObject.activeSelf);
                SFXManager.Instance.playerAttacksSource.pitch = Random.Range(2f, 2.1f);
                SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.activeSwordAura);

                cooldownController.StartSkillCooldown();
                break;
        }

        chargedAttack = false;
    }


    public void CallAirCutPrefab() 
    {
        if (!activedPowerAura.activeSelf || activedPowerAura == null) return;
        if (PlayerComponents.Instance.playerStatus.currentMana < PlayerComponents.Instance.weaponBehavior.resourceCost)
        {
            activedPowerAura.gameObject.SetActive(false);
            return;
        }
            
        PlayerComponents.Instance.weaponBehavior.ConsumeMana();
        PlayerComponents.Instance.weaponBehavior.ConsumeMana();

        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.airCutInstanceSound);

        GameObject newProjectile = Instantiate(weaponSkillPrefab, transform.position, transform.rotation);

        AirCutBehavior projectileScript = newProjectile.GetComponent<AirCutBehavior>();
        projectileScript.airCutDamage = damage * skillDamageMultiplier;
        projectileScript.SetDirection(Camera.main.transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyFSM enemyFSM = other.GetComponent<EnemyFSM>();
            EnemyEffects enemyFX = other.GetComponent<EnemyEffects>();
            Animator enemyAnim = other.GetComponent<Animator>();


            if (enemyAnim != null)
                enemyAnim.SetTrigger("hurt");

            if (enemyFX != null)
            {
                Instantiate(enemyFX.attackTakenParticle, other.ClosestPointOnBounds(transform.position), Quaternion.identity, other.transform);
                SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.7f, 1f);
                SFXManager.Instance.playerAttacksSource.PlayOneShot(enemyFX.attackTakenSound);
            }

            if (enemyFSM != null)
                enemyFSM.DoDamage(damage + PlayerComponents.Instance.playerStatus.damage);
        }
    }
}