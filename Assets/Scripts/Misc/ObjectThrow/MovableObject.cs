using UnityEngine;

public class MovableObject : MonoBehaviour, IInteractable
{
    public bool isHolding = false;
    public bool isThrown = false; // Flag para verificar se o objeto foi arremessado

    [SerializeField] private float throwForce = 600f;
    [SerializeField] private float damage = 2f;
    private float distance;

    private TempParent tempParent;
    private Rigidbody rb;
    public GameObject collisionParticle;
    public GameObject destroyParticle;
    public AudioClip destroySound;

    Vector3 objectPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempParent = TempParent.Instance;
    }

    private void Update()
    {
        if (isHolding)
        {
            Hold();
            TempParent.Instance.isHoldingAnything = true;
        }
    }

    public void Interact()
    {
        if (!isHolding)
        {
            StartHolding();
        }
        else
        {
            Drop();
        }
    }

    private void StartHolding()
    {
        isHolding = true;
        isThrown = false;
        rb.useGravity = false;
        rb.detectCollisions = true;

        this.transform.SetParent(tempParent.transform);
    }

    private void Hold()
    {
        distance = Vector3.Distance(this.transform.position, tempParent.transform.position);

        //if (distance >= maxDistance)
        //    Drop();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            isThrown = true;
            rb.AddForce(tempParent.transform.forward * throwForce);
            SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.objectThrowSound);
            Drop();
        }
    }

    private void Drop()
    {
        if (isHolding)
        {
            isHolding = false;
            objectPos = this.transform.position;
            this.transform.position = objectPos;
            this.transform.SetParent(null);
            rb.useGravity = true;
        }
        TempParent.Instance.isHoldingAnything = false;
        HudManager.Instance.warnBtn.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isThrown)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<EnemyFSM>().DoDamage(damage);

                Instantiate(collisionParticle, other.ClosestPoint(transform.position), Quaternion.identity);
                GameObject destroyPart = Instantiate(destroyParticle, other.ClosestPoint(transform.position), Quaternion.identity);
                destroyPart.GetComponent<AudioSource>().PlayOneShot(destroySound);
                Destroy(this.gameObject);
            }
            TempParent.Instance.isHoldingAnything = false;
            HudManager.Instance.warnBtn.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isThrown)
        {
            GameObject destroyPart = Instantiate(destroyParticle, other.contacts[0].point, Quaternion.identity);
            destroyPart.GetComponent<AudioSource>().PlayOneShot(destroySound);
            Destroy(this.gameObject);

            TempParent.Instance.isHoldingAnything = false;
            HudManager.Instance.warnBtn.SetActive(false);
        }
    }
}
