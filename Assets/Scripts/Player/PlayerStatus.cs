using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float playerGravity;

    public float walkSpeed;
    public float runSpeed;
    public float actualSpeed;

    public float jumpHeight;

    public float maxHp;
    public float currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void DoDamage(float damage)
    {
        currentHp -= damage;
        HudManager.Instance.ChangeVignetteColorToRed();
        GetComponent<PlayerFSM>()._handsPOV.GetComponent<Animator>().SetTrigger("damage");
    }
}
