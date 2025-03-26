using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [Header ("References")]
    public PlayerStatus playerStatus;
    private PlayerFSM fsm;
    public InputHandler inputHandler;

    public CinemachineVirtualCamera cinemachineCam;
    public bool playerIsDeath = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerStatus = GetComponent<PlayerStatus>();
        fsm = GetComponent<PlayerFSM>();
        inputHandler = GetComponent<InputHandler>();
    }

    private float lastFootstepTime = 0f; 
    private float minStepInterval = 0.2f;
    private void CallStep()
    {
        if (Time.time - lastFootstepTime < minStepInterval)
            return;

        lastFootstepTime = Time.time; 

        SFXManager.Instance.playerFootSource.pitch = Random.Range(0.6f, 0.8f);
        SFXManager.Instance.playerFootSource.PlayOneShot(SFXManager.Instance.actualStepSound);
    }

    public void DoDamage(float damage)
    {
        if (!playerIsDeath)
        {
            playerStatus.currentHp -= damage;
            GetComponent<PlayerFSM>()._handsPOV.GetComponent<Animator>().SetTrigger("damage");

            if (playerStatus.currentHp <= 0)
                fsm.FSM.SetState("Death");

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

        playerStatus.currentHp = playerStatus.maxHp;
    }

}

