using UnityEngine;

public class MagicBookBehavior : MonoBehaviour
{
    public GameObject skillPrefab;
    public GameObject exitPoint;
    public float damage;

    private PlayerStatus playerStatus;
    private CooldownController cooldownController;

    private void Awake()
    {
        cooldownController = GetComponentInParent<CooldownController>();
    }

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    public void UseBookSkill(bool canUseSkill, float resourceCost, Animator secondaryAnim)
    {
        if (!cooldownController.CanUseMagic()) return;


        if (canUseSkill && playerStatus.currentMana >= resourceCost)
        {
            secondaryAnim.SetTrigger("orbAttack");
            cooldownController.StartMagicCooldown();
        }
    }

    public void CallSkillProjectilePrefab()
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);

        GameObject newProjectile = Instantiate(skillPrefab, exitPoint.transform.position, Camera.main.transform.rotation);
        //ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        //projectileScript.projectileDamage = damage;
        //projectileScript.SetDirection(Camera.main.transform.forward);
    }
}