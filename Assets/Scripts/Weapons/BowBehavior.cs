using UnityEngine;
using System.Collections;

public class BowBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public float damage;
    public GameObject projectileExit;

    public GameObject bowProjectilePrefab;

    private bool isTimeSlowedByBow = false;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    //Common Attacks
    public void ExecuteBowAttack(bool canAttack, float resourceCost, Animator weaponAnim)
    {
        if (canAttack && playerStatus.currentMana >= resourceCost)
        {
            weaponAnim.SetTrigger("bowAttack");
            GetComponentInChildren<BowBehavior>().GetComponent<Animator>().SetTrigger("attack");
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
        float startFOV = PlayerManager.Instance.cinemachineCam.m_Lens.FieldOfView;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 5f;
            PlayerManager.Instance.cinemachineCam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);
            yield return null;
        }

        PlayerManager.Instance.cinemachineCam.m_Lens.FieldOfView = targetFOV;
    }

    //Bow Projectile to be Called
    public void CallBowProjectile()
    {
        GameObject newProjectile = Instantiate(bowProjectilePrefab, projectileExit.transform.position, Camera.main.transform.rotation);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        projectileScript.projectileDamage = damage;
        projectileScript.SetDirection(Camera.main.transform.forward);

        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.bowSwooshSound);
    }
}
