using UnityEngine;

public class GreatSwordBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private CooldownController cooldownController;

    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public Collider weaponColl;

    public GameObject weaponSkillPrefab;

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

            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteGreatSwordDashAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatSwordDashAttack");
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    public void ExecuteGreatSwordJumpAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentStamina >= resourceCost)
        {
            weaponAnim.SetTrigger("greatSwordJumpAttack");
            SFXManager.Instance.playerAttacksSource.pitch = Random.Range(0.5f, 0.7f);
            SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.attackSwooshSound);
        }
    }

    //Special Habilities
    public void UseCircularCut(bool chargedAttack, float resourceCost, bool canAttack, Animator weaponAnim)
    {
        if (!cooldownController.CanUseSkill()) return;

        if (chargedAttack && playerStatus.currentStamina >= resourceCost && canAttack) //A propria animação controla se canAttack ou não
        {
            weaponAnim.SetTrigger("greatSwordSpecial");
            cooldownController.StartSkillCooldown();
        }
        chargedAttack = false;
    }

    public void CallCircularCutPrefab() //Quem instancia o CircularCut (Especial do GreatHammer) é a propria animation;
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.circularCutSound);

        Vector3 playerPos = new Vector3(PlayerManager.Instance.transform.position.x, PlayerManager.Instance.transform.position.y + 0.5f, PlayerManager.Instance.transform.position.z);

        GameObject currentCircularCut = Instantiate(weaponSkillPrefab, playerPos, Camera.main.transform.rotation);
        var cutBeh = currentCircularCut.GetComponent<CircularCutBehavior>();
        cutBeh.circularCutDamage = damage;
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