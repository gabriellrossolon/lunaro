using UnityEngine;
using static ItemBase;
using static UnityEngine.ParticleSystem;

public class DroppedItem : MonoBehaviour
{
    public ItemBase item;
    public float maxDistance = 2f;
    private GameObject player;
    private Rigidbody rb;

    private ParticleSystem[] particles;
    private Light itemLight;

    public float throwForce = 5f;
    public float throwRadius = 2f;
    public float throwHeight = 2f; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        particles = GetComponentsInChildren<ParticleSystem>();
        itemLight = GetComponentInChildren<Light>();
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

    private void Update()
    {
        UpdateVisuals();
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
            if (HudManager.Instance.warnBtn.activeSelf && HudManager.Instance.warnBtn != null)
                HudManager.Instance.warnBtn.SetActive(false);

            if (HudManager.Instance.itemShow.activeSelf && HudManager.Instance.itemShow != null)
                HudManager.Instance.itemShow.SetActive(false);
        }
    }

    private void UpdateVisuals()
    {
        if (item == null) return;

        Color rarityColor = GetRarityColor(item.itemRarity);

        foreach (var particle in particles)
        {
            var main = particle.main;
            main.startColor = rarityColor;
        }

        if (itemLight != null)
            itemLight.color = rarityColor;
    }

    Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return Color.white;
            case ItemRarity.Uncommon: return new Color(0.5f, 1f, 0.5f);
            case ItemRarity.Rare: return new Color(0.1f, 0.1f, 0.6f);
            case ItemRarity.Epic: return new Color(0.6f, 0.1f, 0.6f);
            case ItemRarity.Legendary: return Color.yellow;
            default: return Color.gray;
        }
    }
}




