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

    public void UseBookSpell(bool canUseSkill, float resourceCost, Animator secondaryAnim)
    {
        if (!cooldownController.CanUseMagic()) return;


        if (canUseSkill)
        {
            if (playerStatus.currentMana < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("mana");
                return;
            }
            secondaryAnim.SetTrigger("bookAttack");
            cooldownController.StartMagicCooldown();
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