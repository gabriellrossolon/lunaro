using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float playerGravity;

    public float walkSpeed;
    public float runSpeed;
    public float actualSpeed;

    public float jumpHeight;

    public float damage;

    public float maxHp;
    public float currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void ControlDamage(float value)
    {
        damage += value;

        if (damage < 0)
        {
            damage = 0;
        }
    }

    public void ControlHp(float value)
    {
        maxHp += value;

        if (currentHp >= maxHp)
        {
            currentHp = maxHp;
        }
    }
}
