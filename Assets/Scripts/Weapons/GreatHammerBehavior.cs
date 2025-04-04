using UnityEngine;

public class GreatHammerBehavior : MonoBehaviour
{
    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;
    public GameObject collisionParticle;

    private int attackSequence = 1;

    public GameObject weaponSkillPrefab;
    public float skillDamageMultiplier;

    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
    }

    private void Update()
    {
        weaponColl.enabled = colliderEnabled;
    }

    //Attack Variations (Only Basic Attacks)

    public void ExecuteGreatHammerAttack(bool canAttack, float resourceCost, Animator weaponAnim)
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
        }
    }
    public void ExecuteGreatHammerDashAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack)
        {
            if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("stamina");
                return;
            }
            weaponAnim.SetTrigger("greatHammerDashAttack");
        }
    }
    public void ExecuteGreatHammerJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack)
        {
            if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("stamina");
                return;
            }
            weaponAnim.SetTrigger("greatHammerJumpAttack");
        }
    }

    //Special Attacks
    public void UseGreatHammerSkill(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!PlayerComponents.Instance.cooldownController.CanUseSkill() || !chargedAttack || !canAttack) return;
        if(PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
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

            case Skill.SkillType.Earthquake:
                weaponAnim.SetTrigger("greatHammerSpecial");
                break;
        }

        PlayerComponents.Instance.cooldownController.StartSkillCooldown();
        chargedAttack = false;
    }

    public void CallEarthquakePrefab() //Quem instancia o Earthquake (Especial do GreatHammer) é a propria animation;
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.heavyImpactSound);

        GameObject currentEarthquake = Instantiate(weaponSkillPrefab, transform.position - new Vector3(0, 1.2f, 0), Quaternion.identity);

        var explosionBeh = currentEarthquake.GetComponent<ExplosionBehavior>();

        explosionBeh.explosionDamage = damage * skillDamageMultiplier;
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

            //GameManager.Instance.TriggerSlowMotion(0.1f, 0.2f);
        }
    }
}