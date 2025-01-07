using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerStatus playerStatus;

    public static PlayerManager Instance;

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
    }

    private void CallStep()
    {
        SFXManager.Instance.playerFootSource.pitch = Random.Range(0.6f, 0.8f);
        SFXManager.Instance.playerFootSource.PlayOneShot(SFXManager.Instance.actualStepSound);
    }



    public void DoDamage(float damage)
    {
        playerStatus.currentHp -= damage;
        HudManager.Instance.ChangeVignetteColorToRed();
        GetComponent<PlayerFSM>()._handsPOV.GetComponent<Animator>().SetTrigger("damage");
    }
}

