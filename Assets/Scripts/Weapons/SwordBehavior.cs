using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    public float damage;
    public bool colliderEnabled = false;

    [HideInInspector] public MeshCollider meshColl;

    private void Awake()
    {
        meshColl = GetComponent<MeshCollider>();
    }
    private void Update()
    {
        meshColl.enabled = colliderEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
