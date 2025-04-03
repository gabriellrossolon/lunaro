using TMPro;
using UnityEngine;
using static ItemBase;

public class DroppedItem : MonoBehaviour, IInteractable
{
    public ItemBase item;
    public float maxDistance = 2f;
    private Rigidbody rb;
    [SerializeField] private GameObject dropInfos;
    private TextMeshProUGUI dropName;

    private ParticleSystem[] particles;
    private Light itemLight;

    [SerializeField] private AudioClip droppedSound;
    private AudioSource dropSoundSource;

    public bool canThrow = true;
    public float throwForce = 5f;
    public float throwRadius = 2f;
    public float throwHeight = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        particles = GetComponentsInChildren<ParticleSystem>();
        itemLight = GetComponentInChildren<Light>();
        dropSoundSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        dropSoundSource.clip = droppedSound;
        dropSoundSource.loop = true;
        dropSoundSource.Play();

        if (rb != null && canThrow)
        {
            Vector3 randomDirection = new Vector3(Random.Range(-throwRadius, throwRadius), Random.Range(throwHeight * 0.5f, throwHeight), Random.Range(0f, throwRadius));
            rb.AddForce(randomDirection.normalized * throwForce, ForceMode.Impulse);

            Vector3 randomRotation = new Vector3(0f, Random.Range(-1f, 1f), 0f); // Somente rotação no eixo Y
            rb.AddTorque(randomRotation, ForceMode.Impulse);
        }

        if (dropInfos != null)
        {
            dropName = dropInfos.GetComponentInChildren<TextMeshProUGUI>();
            dropName.text = item.itemName;
            dropName.color = GetRarityColor(item.itemRarity);
        }
    }

    private void Update()
    {
        UpdateVisuals();
        ControlDropCanvas();
    }

    public void Interact()
    {
        HudManager.Instance.RefreshItemShow(item);

        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.itemPickupSound);
        PlayerComponents.Instance.secondaryBehavior.gameObject.GetComponent<Animator>().SetTrigger("Catch");

        InventoryManager.Instance.AddItem(item);
        Destroy(gameObject);
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

    private void ControlDropCanvas()
    {
        if (PlayerComponents.Instance.playerManager != null && dropInfos != null)
        {
            float distance = Vector3.Distance(PlayerComponents.Instance.playerObject.transform.position, transform.position);

            if (distance <= 15f)
            {
                dropInfos.SetActive(true);
                dropInfos.transform.position = transform.position + new Vector3(0, 1f, 0);
                Vector3 lookDirection = PlayerComponents.Instance.playerObject.transform.position - dropInfos.transform.position;
                dropInfos.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
            else
            {
                dropInfos.SetActive(false);
            }
        }
    }
}