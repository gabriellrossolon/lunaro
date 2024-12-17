using UnityEngine;

public class CooldownController : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashCooldown = 1.5f;
    private float dashCooldownTimer;
    public bool canDash = true; 

    private void Update()
    {
        if (!canDash)
        {
            if (dashCooldownTimer < dashCooldown)
            {
                dashCooldownTimer += Time.deltaTime;
            }
            else
            {
                canDash = true;
                dashCooldownTimer = 0;
            }
        }
    }
}
