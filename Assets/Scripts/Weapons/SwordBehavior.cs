using UnityEngine;
using UnityEngine.AI;

public class SwordBehavior : MonoBehaviour
{
    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public BoxCollider weaponColl;
    public GameObject collisionParticle;

    private void Awake()
    {
        weaponColl = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        weaponColl.enabled = colliderEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {      
            other.GetComponent<EnemyStats>().DoDamage(damage);
            Instantiate(collisionParticle, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }
}
