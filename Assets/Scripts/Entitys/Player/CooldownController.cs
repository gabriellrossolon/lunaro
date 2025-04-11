using System.Collections;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    private bool isOnSkillCooldown = false;
    private bool isOnDashCooldown = false;
    private bool isOnMagicCooldown = false;

    [Header("Cooldowns")]
    [SerializeField] private float dashCooldown;
    [SerializeField] private float skillCooldown;
    [SerializeField] private float magicCooldown;

    private void Start()
    {
        HudManager.Instance.dashCooldownSlider.maxValue = dashCooldown;
        HudManager.Instance.skillCooldownSlider.maxValue = skillCooldown;
        HudManager.Instance.magicCooldownSlider.maxValue = magicCooldown;
    }

    public bool CanUseSkill() => !isOnSkillCooldown;
    public bool CanUseDash() => !isOnDashCooldown;
    public bool CanUseMagic() => !isOnMagicCooldown;

    public void StartSkillCooldown() => StartCooldown(() => isOnSkillCooldown, (value) => isOnSkillCooldown = value, skillCooldown, HudManager.Instance.skillCooldownSlider);
    public void StartDashCooldown() => StartCooldown(() => isOnDashCooldown, (value) => isOnDashCooldown = value, dashCooldown, HudManager.Instance.dashCooldownSlider);
    public void StartMagicCooldown() => StartCooldown(() => isOnMagicCooldown, (value) => isOnMagicCooldown = value, magicCooldown, HudManager.Instance.magicCooldownSlider);

    private void StartCooldown(System.Func<bool> getCooldownFlag, System.Action<bool> setCooldownFlag, float cooldownTime, UnityEngine.UI.Slider cooldownSlider)
    {
        if (!getCooldownFlag())
        {
            StartCoroutine(CooldownRoutine(setCooldownFlag, cooldownTime, cooldownSlider));
        }
    }

    private IEnumerator CooldownRoutine(System.Action<bool> setCooldownFlag, float cooldownTime, UnityEngine.UI.Slider cooldownSlider)
    {
        setCooldownFlag(true);
        cooldownSlider.value = cooldownTime;

        float timeLeft = cooldownTime;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            cooldownSlider.value = timeLeft;
            yield return null;
        }

        cooldownSlider.value = 0;
        setCooldownFlag(false);
    }
}
