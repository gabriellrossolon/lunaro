using UnityEngine;

public class MagicBookBehavior : MonoBehaviour
{
    public GameObject skillPrefab;
    public GameObject exitPoint;
    public float damage;


    public void UseBookSpell(bool canUseSkill, float resourceCost, Animator secondaryAnim)
    {
        if (!PlayerComponents.Instance.cooldownController.CanUseMagic()) return;


        if (canUseSkill)
        {
            if (PlayerComponents.Instance.playerStatus.currentMana < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("mana");
                return;
            }
            secondaryAnim.SetTrigger("bookAttack");
            PlayerComponents.Instance.cooldownController.StartMagicCooldown();
        }
    }

    public void CallSkillPrefab(string skillName)
    {
        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);

        if (skillName == "Slit")
        {
            GameObject newSkillPrefab = Instantiate(skillPrefab, transform.position - new Vector3(0, 1.2f, 0), Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0));

            SlitBehavior slitBeh = newSkillPrefab.GetComponent<SlitBehavior>();
            slitBeh.slitDamage = damage;
        }
        else
        {
            Debug.LogWarning("Habilidade não reconhecida: " + skillName);
        }
    }
}