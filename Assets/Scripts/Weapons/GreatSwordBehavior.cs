using UnityEngine;

public class GreatSwordBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private CooldownController cooldownController;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;

    public GameObject weaponSkillPrefab;
    public float skillDamageMultiplier;

    private int attackSequence = 1;

    private void Awake()
    {
        weaponColl = GetComponent<Collider>();
        cooldownController = GetComponentInParent<CooldownController>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }
    private void Update()
    {
        weaponColl.enabled = colliderEnabled;
    }

    //Attack Variations (Only Basic Attacks)

    public void ExecuteGreatSwordAttack(bool canAttack, float resourceCost, Animator weaponAnim)
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
        }
    }

    public void ExecuteGreatSwordDashAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatSwordDashAttack");
        }
    }

    public void ExecuteGreatSwordJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatSwordJumpAttack");
        }
    }

    //Special Habilities
    public void UseGreatSwordSkill(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill() || !chargedAttack || playerStatus.currentStamina < resourceCost || !canAttack) return;
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

            case Skill.SkillType.CircularCut:
                weaponAnim.SetTrigger("greatSwordSpecial");
                break;
        }

        cooldownController.StartSkillCooldown();
        chargedAttack = false;
    }


    public void CallCircularCutPrefab() //Quem instancia o CircularCut é a propria animation;
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.circularCutSound);

        Vector3 playerPos = new Vector3(PlayerManager.Instance.transform.position.x, PlayerManager.Instance.transform.position.y + 0.5f, PlayerManager.Instance.transform.position.z);

        GameObject currentCircularCut = Instantiate(weaponSkillPrefab, playerPos, Camera.main.transform.rotation);
        var cutBeh = currentCircularCut.GetComponent<CircularCutBehavior>();
        cutBeh.circularCutDamage = damage * skillDamageMultiplier;
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
                enemyFSM.DoDamage(damage + playerStatus.damage);

            GameManager.Instance.TriggerSlowMotion(0.1f, 0.2f);
        }
    }
}