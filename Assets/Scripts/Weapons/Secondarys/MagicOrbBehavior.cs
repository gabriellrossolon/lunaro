using UnityEngine;

public class MagicOrbBehavior : MonoBehaviour
{
    public GameObject skillPrefab;
    public GameObject exitPoint;
    public float damage;

    //Common Skills
    public void UseOrbSpell(bool canUseSkill, float resourceCost, Animator secondaryAnim)
    {
        if (!PlayerComponents.Instance.cooldownController.CanUseMagic()) return;


        if (canUseSkill)
        {
            if (PlayerComponents.Instance.playerStatus.currentMana < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("mana");
                return;
            }
            secondaryAnim.SetTrigger("orbAttack");
            PlayerComponents.Instance.cooldownController.StartMagicCooldown();
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
