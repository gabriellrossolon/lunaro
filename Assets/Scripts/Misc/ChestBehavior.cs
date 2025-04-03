using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour, IInteractable
{
    private Animator animator;
    public GameObject dropBag;
    public GameObject dropPoint;
    private Collider chestCollider;
    public float maxDistance;
    private AudioSource chestSoundSource;

    public List<ItemBase> chestItems = new List<ItemBase>();

    [Header("Random Item Configuration")]
    public bool randomItems = false;
    public int chestCapacity = 3;

    [Range(0, 100)] public float commonChance = 45f;
    [Range(0, 100)] public float uncommonChance = 25f;
    [Range(0, 100)] public float rareChance = 15f;
    [Range(0, 100)] public float epicChance = 10f;
    [Range(0, 100)] public float legendaryChance = 5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        chestCollider = GetComponent<Collider>();
        chestSoundSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (randomItems)
            GenerateRandomItems();
    }

    public void Interact()
    {
        float distance = Vector3.Distance(transform.position, PlayerComponents.Instance.playerObject.transform.position);

        if (distance <= maxDistance)
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        DisableBehavior();

        animator.SetTrigger("Open");
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.chestOpeningSound);
        StartCoroutine(InstantiateItemsWithDelay());
    }

    private IEnumerator InstantiateItemsWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (ItemBase item in chestItems)
        {
            GameObject newDropBag = Instantiate(dropBag, dropPoint.transform.position, Quaternion.identity);
            DroppedItem droppedItem = newDropBag.GetComponent<DroppedItem>();

            chestSoundSource.PlayOneShot(SFXManager.Instance.dropSound);

            if (droppedItem != null)
                droppedItem.item = item;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DisableBehavior()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

        var outlines = GetComponentsInChildren<Outline>();
        foreach(var outline in outlines)
            outline.enabled = false;

        if (chestCollider != null)
            chestCollider.enabled = false;
    }

    private void OnMouseExit()
    {
        HudManager.Instance.warnBtn.SetActive(false);
        HudManager.Instance.itemShow.SetActive(false);
    }

    private void GenerateRandomItems()
    {
        chestItems.Clear();

        for (int i = 0; i < chestCapacity; i++)
        {
            int maxAttempts = 10;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                ItemBase.ItemRarity chosenRarity = GetRandomRarity();
                List<ItemBase> itemsByRarity = ItemDatabase.Instance.GetItemsByRarity(chosenRarity);

                if (itemsByRarity != null && itemsByRarity.Count > 0)
                {
                    ItemBase randomItem = itemsByRarity[Random.Range(0, itemsByRarity.Count)];

                    if (!chestItems.Contains(randomItem))
                    {
                        chestItems.Add(randomItem);
                        break;
                    }
                }

                attempts++;
            }
        }
    }

    private ItemBase.ItemRarity GetRandomRarity()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= commonChance) return ItemBase.ItemRarity.Common;
        if (roll <= commonChance + uncommonChance) return ItemBase.ItemRarity.Uncommon;
        if (roll <= commonChance + uncommonChance + rareChance) return ItemBase.ItemRarity.Rare;
        if (roll <= commonChance + uncommonChance + rareChance + epicChance) return ItemBase.ItemRarity.Epic;
        return ItemBase.ItemRarity.Legendary;
    }
}
