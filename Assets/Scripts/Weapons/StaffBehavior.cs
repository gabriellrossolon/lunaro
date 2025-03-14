using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class StaffBehavior : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public GameObject staffProjectilePrefab;

    public float damage;
    public GameObject projectileExit;
    private int attackSequence = 1;

    public GameObject beamPrefab;
    private GameObject currentBeam;
    private bool usingBeam;

    public float skillDamageMultiplier;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
    }

    //Common Attacks
    public void ExecuteStaffAttack(bool canAttack, float resourceCost, bool chargedAttack,Animator weaponAnim, Animator leftHandAnim)
    {
        if (canAttack && playerStatus.currentMana >= resourceCost && !chargedAttack)
        {
            switch (attackSequence)
            {
                case 1:
                    weaponAnim.SetTrigger("staffAttack1");
                    attackSequence++;
                    break;
                case 2:
                    weaponAnim.SetTrigger("staffAttack2");
                    attackSequence++;
                    break;
                case 3:
                    weaponAnim.SetTrigger("staffAttack3");
                    attackSequence = 1;
                    break;
                default:
                    attackSequence = 1;
                    break;
            }
            string retreatTrigger = "retreat" + Random.Range(1, 4);
            leftHandAnim.SetTrigger(retreatTrigger);
        }
    }

    //Special Habilities
    public void UseMagicBeam(bool chargedAttack, float resourceCost, Animator weaponAnim)
    {
        if (chargedAttack && playerStatus.currentMana >= resourceCost)
        {
            if (!usingBeam) // Garante que a Coroutine não rode múltiplas vezes
            {
                usingBeam = true;
                StartCoroutine(HandleChargingBeam(resourceCost));
            }
            weaponAnim.SetBool("ChargingAttack", true);
        }
        else
        {
            usingBeam = false; // Isso faz a Coroutine parar no próximo loop
            weaponAnim.SetBool("ChargingAttack", false);
        }
    }

    private IEnumerator HandleChargingBeam(float resourceCost)
    {
        while (usingBeam) // Verifica a variável global que pode mudar
        {
            if (playerStatus.currentMana >= resourceCost)
            {
                ConsumeManaForBeam(resourceCost / 5);
                playerStatus.canRegenMana = false;
            }
            yield return new WaitForSeconds(0.1f);
        }
        playerStatus.canRegenMana = true;
    }

    public void CallBeamPrefab() //Quem chama o Burst (Beam) é a propria animação
    {
        if (currentBeam != null)
        {
            Destroy(currentBeam);
            currentBeam = null;
        }
        else
        {
            currentBeam = Instantiate(beamPrefab, projectileExit.transform.position, projectileExit.transform.rotation, projectileExit.transform);
            var beamBeh = currentBeam.GetComponent<BeamBehavior>();
            var staffBeh = GetComponentInChildren<StaffBehavior>();

            beamBeh.damage = staffBeh.damage * skillDamageMultiplier;

            VisualEffect projectileEffect = currentBeam.GetComponentInChildren<VisualEffect>();
            if (projectileEffect != null)
            {
                projectileEffect.Play();
            }
        }
    }

    public void ConsumeManaForBeam(float value)
    {
        playerStatus.SubtractMana(value);
    }

    //Projectile to Call
    public void CallStaffProjectile()
    {
        GameObject newProjectile = Instantiate(staffProjectilePrefab, projectileExit.transform.position, Camera.main.transform.rotation);
        ProjectileBehavior projectileScript = newProjectile.GetComponent<ProjectileBehavior>();
        projectileScript.projectileDamage = damage;
        projectileScript.SetDirection(Camera.main.transform.forward);

        if(projectileScript.muzzleParticle != null)
            Instantiate(projectileScript.muzzleParticle, projectileExit.transform.position, Quaternion.identity);

        SFXManager.Instance.playerAttacksSource.pitch = Random.Range(1f, 1.2f);
        SFXManager.Instance.playerAttacksSource.PlayOneShot(SFXManager.Instance.magicCastSound);
    }
}
