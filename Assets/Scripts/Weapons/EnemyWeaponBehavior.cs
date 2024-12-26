using UnityEngine;

public class EnemyWeaponBehavior : MonoBehaviour
{
    private EnemySwordBehavior swordBeh;
    private void Awake()
    {
        swordBeh = GetComponentInChildren<EnemySwordBehavior>();
    }

    public void CallCollider()
    {
        if(swordBeh != null)
        {
            swordBeh.EnableCollider();
        }
    }
}
