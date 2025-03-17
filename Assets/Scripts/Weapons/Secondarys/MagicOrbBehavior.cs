using UnityEngine;

public class MagicOrbBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    private CooldownController cooldownController;

    public GameObject skillPrefab;
    public GameObject exitPoint;
    public float damage;

    private void Awake()
    {
        cooldownController = GetComponentInParent<CooldownController>();
    }
    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    //Common Skills
    public void UseOrbSpell(bool canUseSkill, float resourceCost, Animator secondaryAnim)
    {
        if (!cooldownController.CanUseMagic()) return;


        if (canUseSkill)
        {
            if (playerStatus.currentMana < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("mana");
                return;
            }
            secondaryAnim.SetTrigger("orbAttack");
            cooldownController.StartMagicCooldown();
        }
    }

    //Skill Projectile Prefab
    public void CallSkillPrefab(string skillName)
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);

        if (skillName == "Bolide")
        {
            GameObject newProjectile = Instantiate(skillPrefab, exitPoint.transform.position, Camera.main.transform.rotation);
            ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
            projectileScript.projectileDamage = damage;
            projectileScript.SetDirection(Camera.main.transform.forward);
        }
        else if (skillName == "MagicMissile")
        {
            GameObject newProjectile = Instantiate(skillPrefab, exitPoint.transform.position, Camera.main.transform.rotation);
            MagicMissileBehavior magicMissileBeh = newProjectile.GetComponent<MagicMissileBehavior>();
            magicMissileBeh.damage = damage;
        }
    }

}
