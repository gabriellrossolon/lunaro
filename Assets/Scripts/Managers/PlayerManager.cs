using UnityEngine;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public CinemachineVirtualCamera cinemachineCam;
    public static PlayerManager Instance;
    private PlayerFSM fsm;
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
    }

    private void CallStep()
    {
        SFXManager.Instance.playerFootSource.pitch = Random.Range(0.6f, 0.8f);
        SFXManager.Instance.playerFootSource.PlayOneShot(SFXManager.Instance.actualStepSound);
    }

    public void DoDamage(float damage)
    {
        if (!playerIsDeath)
        {
            playerStatus.currentHp -= damage;
            HudManager.Instance.ChangeVignetteColorToRed();
            GetComponent<PlayerFSM>()._handsPOV.GetComponent<Animator>().SetTrigger("damage");

            if (playerStatus.currentHp <= 0)
            {
                fsm.FSM.SetState("Death");
            }
        }
    }

    public void Ressurect()
    {
        playerIsDeath = false;
        TeleportManager.Instance.GoToHUB();
        playerStatus.currentHp = playerStatus.maxHp;
    }
}

