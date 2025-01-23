using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ItemBase item;
    public float maxDistance = 2f;
    private GameObject player;
    private Rigidbody rb;

    public float throwForce = 5f;
    public float throwRadius = 2f;
    public float throwHeight = 2f; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;

        if (rb != null)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-throwRadius, throwRadius), Random.Range(throwHeight * 0.5f, throwHeight), Random.Range(0f, throwRadius));
            rb.AddForce(randomDirection.normalized * throwForce, ForceMode.Impulse);

            Vector3 randomRotation = new Vector3(0f, Random.Range(-1f, 1f), 0f); // Somente rotação no eixo Y
            rb.AddTorque(randomRotation, ForceMode.Impulse);
        }
    }

    private void OnMouseOver()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.RefreshItemShow(item);
            HudManager.Instance.itemShow.SetActive(true);
            HudManager.Instance.warnBtn.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.itemPickupSound);
                player.GetComponentInChildren<SecondaryBehavior>().gameObject.GetComponent<Animator>().SetTrigger("Catch");

                InventoryManager.Instance.AddItem(item);
                Destroy(gameObject, 0.2f);
            }
        }
        else
        {
            HudManager.Instance.warnBtn.SetActive(false);
            HudManager.Instance.itemShow.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        HudManager.Instance.warnBtn.SetActive(false);
        HudManager.Instance.itemShow.SetActive(false);
    }

    private void OnDestroy()
    {
        if (HudManager.Instance != null)
        {
            if (HudManager.Instance.warnBtn.activeSelf)
                HudManager.Instance.warnBtn.SetActive(false);

            if (HudManager.Instance.itemShow.activeSelf)
                HudManager.Instance.itemShow.SetActive(false);
        }
    }
}




