using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header ("Player Feet Controll")]
    public GameObject leftFeetPos;
    public GameObject rightFeetPos;
    private bool isRightFoot = true;

    [Header("Player SFX")]
    public AudioSource footSoundSource;
    public AudioSource movementSoundSource;
    public AudioSource attackSoundSource;
    public AudioSource miscSoundSource;

    [Header("PlayerVFX")]
    public GameObject stepParticle;
    public GameObject dashParticle;
    public GameObject jumpingParticle;
    public GameObject landingParticle;
    public GameObject waterLandingParticle;

    [Header("Player Death Controll")]
    public bool playerIsDeath = false;

    private float lastFootstepTime = 0f; 
    private float minStepInterval = 0.2f;

    private void Start()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.playerFootSource = PlayerComponents.Instance.playerManager.footSoundSource;
            SFXManager.Instance.playerAttacksSource = PlayerComponents.Instance.playerManager.attackSoundSource;
            SFXManager.Instance.playerMovementsSource = PlayerComponents.Instance.playerManager.movementSoundSource;
            SFXManager.Instance.miscSoundSource = PlayerComponents.Instance.playerManager.miscSoundSource;
        }
    }
    private void CallStep()
    {
        if (Time.time - lastFootstepTime < minStepInterval)
            return;

        lastFootstepTime = Time.time;

        SFXManager.Instance.playerFootSource.pitch = Random.Range(0.7f, 0.8f);
        SFXManager.Instance.playerFootSource.PlayOneShot(SFXManager.Instance.actualStepSound);

        Transform footPos = isRightFoot ? rightFeetPos.transform : leftFeetPos.transform;
        isRightFoot = !isRightFoot;

        Instantiate(stepParticle, footPos.position, Quaternion.identity);
    }

    public void DoDamage(float damage)
    {
        if (!playerIsDeath)
        {
            PlayerComponents.Instance.playerStatus.currentHp -= damage;
            PlayerComponents.Instance.fsm._handsPOV.GetComponent<Animator>().SetTrigger("damage");

            if (PlayerComponents.Instance.playerStatus.currentHp <= 0)
                PlayerComponents.Instance.fsm.FSM.SetState("Death");

            if (damage < 1)
                return;
            else if (damage <= 2)
                HudManager.Instance.ChangeVignetteColorToRed(0.2f);
            else
                HudManager.Instance.ChangeVignetteColorToRed(0.6f); 
        }
    }

    public void Ressurect()
    {
        Debug.Log("Ressucitou");
        playerIsDeath = false;
        Checkpoint activeCheckpoint = null;

        var allCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var checkpoint in allCheckpoints)
        {
            if (checkpoint.GetComponent<Checkpoint>().checkpointActived)
            {
                activeCheckpoint = checkpoint.GetComponent<Checkpoint>();
                break;
            }
        }

        var enemySpawner = GameObject.FindWithTag("EnemySpawner");
        if(enemySpawner != null)
            enemySpawner.GetComponent<DungeonSpawner>().SpawnEnemies();

        if (activeCheckpoint != null)
        {
            transform.position = activeCheckpoint.transform.position;
        }
        else
        {
            TeleportManager.Instance.GoToHUB();
        }

        PlayerComponents.Instance.playerStatus.currentHp = PlayerComponents.Instance.playerStatus.maxHp;
    }

}
