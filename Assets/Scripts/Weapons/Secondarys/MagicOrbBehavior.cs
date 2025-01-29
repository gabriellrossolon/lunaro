using UnityEngine;

public class MagicOrbBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public GameObject skillPrefab;
    public GameObject exitPoint;
    public float damage;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    //Common Skills
    public void UseOrbSkill(bool canUseSkill, float resourceCost, Animator secondaryAnim)
    {
        if (canUseSkill && playerStatus.currentMana >= resourceCost)
        {
            secondaryAnim.SetTrigger("orbAttack");
        }
    }

    //Skill Projectile Prefab
    public void CallSkillProjectilePrefab()
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);

        GameObject newProjectile = Instantiate(skillPrefab, exitPoint.transform.position, Camera.main.transform.rotation);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        projectileScript.projectileDamage = damage;
        projectileScript.SetDirection(Camera.main.transform.forward);
    }
}
