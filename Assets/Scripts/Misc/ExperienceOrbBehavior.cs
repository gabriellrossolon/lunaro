using UnityEngine;
using System.Collections;

public class ExperienceOrbBehavior : MonoBehaviour
{
    [SerializeField] private float attractionSpeed = 3f;
    [SerializeField] private float upwardImpulse = 3f;
    [SerializeField] private float attractionDistance = 8f;

    private Rigidbody rb;
    private Transform player;

    public float xpValue;

    private bool canGoToPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = PlayerManager.Instance.transform;

        rb.linearVelocity = new Vector3(0, upwardImpulse, 0);

        StartCoroutine(ActivateMovementAfterDelay(1f));

        Destroy(this.gameObject, 120f);
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attractionDistance && canGoToPlayer)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector3(direction.x * attractionSpeed, rb.linearVelocity.y, direction.z * attractionSpeed); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            LevelBehavior levelBeh = other.GetComponent<LevelBehavior>();
            levelBeh.AddExperience(xpValue);
            SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.xpCollectSound);
            Destroy(this.gameObject);
        }
    }
    private IEnumerator ActivateMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canGoToPlayer = true;
    }
}
