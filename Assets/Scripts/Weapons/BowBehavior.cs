using UnityEngine;
using System.Collections;

public class BowBehavior : MonoBehaviour
{
    public bool attackCharging;
    private float chargeTime;
    private const float maxChargeTime = 2f;
    private const float minSpeed = 3f;
    private const float maxSpeed = 50f;

    public float damage;
    public GameObject projectileExit;

    public GameObject bowProjectilePrefab;

    private bool isTimeSlowedByBow = false;

    private void Update()
    {
        if (attackCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
        }

    }
    //Common Attacks
    public void ChargingBowAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && attackCharging)
        {
            if (PlayerComponents.Instance.playerStatus.currentStamina < resourceCost)
            {
                HudManager.Instance.InsuficientResourceWarn("stamina");
                return;
            }
            weaponAnim.SetBool("bowAttackCast", true);
            GetComponentInChildren<BowBehavior>().GetComponent<Animator>().SetBool("AttackStart", true);
        }
        else if (!attackCharging)
        {
            weaponAnim.SetBool("bowAttackCast", false);
            GetComponentInChildren<BowBehavior>().GetComponent<Animator>().SetBool("AttackStart", false);
        }
    }

    //Special habilities

    public void UseConcentration(bool chargedAttack)
    {
        if (chargedAttack && !isTimeSlowedByBow)
        {
            Time.timeScale = 0.7f;
            StartCoroutine(SmoothFOVTransition(50f));
            isTimeSlowedByBow = true;
        }
        else if (!chargedAttack && isTimeSlowedByBow)
        {
            Time.timeScale = 1f;
            StartCoroutine(SmoothFOVTransition(70f));
            isTimeSlowedByBow = false;
        }
    }

    private IEnumerator SmoothFOVTransition(float targetFOV)
    {
        float startFOV = PlayerComponents.Instance.virtualCamera.m_Lens.FieldOfView;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 5f;
            PlayerComponents.Instance.virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        PlayerComponents.Instance.virtualCamera.m_Lens.FieldOfView = targetFOV;
    }

    //Bow Projectile to be Called
    public void CallBowProjectile()
    {
        GameObject newProjectile = Instantiate(bowProjectilePrefab, projectileExit.transform.position, Camera.main.transform.rotation);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        projectileScript.projectileDamage = damage;
        projectileScript.SetDirection(Camera.main.transform.forward);

        float speedMultiplier = Mathf.Lerp(minSpeed, maxSpeed, chargeTime / maxChargeTime);
        projectileScript.speed = speedMultiplier;

        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.bowSwooshSound);

        chargeTime = 0f; // Reset charge time after firing
    }
}
