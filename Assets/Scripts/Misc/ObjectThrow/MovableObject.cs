using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public bool isHolding = false;
    public bool isThrown = false; // Flag para verificar se o objeto foi arremessado

    [SerializeField] private float throwForce = 600f;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float damage = 2f;
    private float distance;

    TempParent tempParent;
    Rigidbody rb;
    GameObject player;
    public GameObject collisionParticle;
    public GameObject destroyParticle;

    Vector3 objectPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        tempParent = TempParent.Instance;
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        distance = Vector3.Distance(this.transform.position, tempParent.transform.position);

        if (isHolding)
            Hold();
    }

    private void OnMouseOver()
    {
        if (distance <= maxDistance)
        {
            if (!isHolding)
            {
                HudManager.Instance.warnBtn.SetActive(true);
            }
            else
            {
                HudManager.Instance.warnBtn.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E) && !isHolding)
            {
                isHolding = true;
                isThrown = false; // Resetar o estado de arremesso
                rb.useGravity = false;
                rb.detectCollisions = true;

                this.transform.SetParent(tempParent.transform);
            }
            else if (Input.GetKeyDown(KeyCode.E) && isHolding)
            {
                Drop();
            }
        }
        else
        {
            HudManager.Instance.warnBtn.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        Drop();
        HudManager.Instance.warnBtn.SetActive(false);
    }

    private void Hold()
    {
        distance = Vector3.Distance(this.transform.position, tempParent.transform.position);

        if (distance >= maxDistance)
        {
            Drop();
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (Input.GetMouseButtonDown(0))
        {
            isThrown = true;
            rb.AddForce(tempParent.transform.forward * throwForce);
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isThrown)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<EnemyFSM>().DoDamage(damage);

                Instantiate(collisionParticle, other.ClosestPoint(transform.position), Quaternion.identity);
                Instantiate(destroyParticle, other.ClosestPoint(transform.position), Quaternion.identity);
                Destroy(this.gameObject);
            }        
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isThrown)
        {
            Instantiate(destroyParticle, other.contacts[0].point, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
