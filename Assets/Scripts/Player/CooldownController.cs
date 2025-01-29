using UnityEngine;

public class CooldownController : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashCooldown = 1.5f;
    private float dashCooldownTimer;
    public bool canDash = true;

    private void Start()
    {
        dashCooldownTimer = dashCooldown;
        HudManager.Instance.dashCooldowSlider.maxValue = dashCooldown;
    }
    private void Update()
    {
        DashCooldown();
    }

    private void DashCooldown()
    {
        if (!canDash)
        {
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
                HudManager.Instance.dashCooldowSlider.value = dashCooldownTimer;
            }
            else
            {
                canDash = true;
                dashCooldownTimer = dashCooldown;
            }
            
        } 
    }
}
