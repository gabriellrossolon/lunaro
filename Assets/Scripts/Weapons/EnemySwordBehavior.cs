using UnityEngine;

public class EnemySwordBehavior : MonoBehaviour
{
    private MeshCollider _coll;
    public bool colliderEnabled = false;
    private void Awake()
    {
        _coll = GetComponent<MeshCollider>();
    }

    private void Update()
    {
        _coll.enabled = colliderEnabled;
    }

    public void EnableCollider()
    {
        colliderEnabled = !colliderEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Colidiu com player");
            HudManager.Instance.ChangeVignetteColorToRed();
        }
    }
}
