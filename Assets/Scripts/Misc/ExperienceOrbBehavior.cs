using UnityEngine;
using System.Collections;
using static Unity.Burst.Intrinsics.X86;

public class ExperienceOrbBehavior : MonoBehaviour
{
    [SerializeField] private float attractionSpeed = 3f;
    [SerializeField] private float attractionDistance = 8f;
    [SerializeField] private float throwRadius = 4f;
    [SerializeField] private float throwHeight = 4f;
    [SerializeField] private float throwForce = 4f;

    private Rigidbody rb;
    [SerializeField] private Collider triggerColl; 

    public float xpValue;

    private bool canGoToPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Vector3 randomDirection = new Vector3(Random.Range(-throwRadius, throwRadius), Random.Range(throwHeight * 0.5f, throwHeight), Random.Range(0f, throwRadius));
        rb.AddForce(randomDirection.normalized * throwForce, ForceMode.Impulse);


        StartCoroutine(ActivateMovementAfterDelay(1f));

        Destroy(this.gameObject, 120f);
    }

    void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, PlayerComponents.Instance.playerObject.transform.position);

        if (distance <= attractionDistance && canGoToPlayer)
        {
            Vector3 direction = (PlayerComponents.Instance.playerObject.transform.position - transform.position).normalized;
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
        triggerColl.enabled = true;
        canGoToPlayer = true;
    }
}
