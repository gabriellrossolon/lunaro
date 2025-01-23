using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour
{
    private Animator animator;
    public GameObject dropBag;
    public GameObject dropPoint;
    private GameObject player;
    private Collider chestCollider;
    public float maxDistance;

    public List<ItemBase> chestItems = new List<ItemBase>();

    [Header("Random Item Configuration")]
    public bool randomItems = false;
    public int chestCapacity = 3;

    [Range(0, 100)] public float commonChance = 50f;
    [Range(0, 100)] public float uncommonChance = 30f;
    [Range(0, 100)] public float rareChance = 15f;
    [Range(0, 100)] public float legendaryChance = 5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        chestCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;

        if (randomItems)
            GenerateRandomItems();
    }

    public void OpenChest()
    {
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

            if (droppedItem != null)
                droppedItem.item = item;

            yield return new WaitForSeconds(0.5f);
        }

        DisableChestBehavior();
    }

    private void DisableChestBehavior()
    {
        enabled = false;
        if (chestCollider != null)
            chestCollider.enabled = false;
    }

    private void OnMouseOver()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.warnBtn.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                OpenChest();
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

    private void GenerateRandomItems()
    {
        chestItems.Clear();

        for (int i = 0; i < chestCapacity; i++)
        {
            int maxAttempts = 10;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Rarity chosenRarity = GetRandomRarity();
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

    private Rarity GetRandomRarity()
    {
        float roll = Random.Range(0f, 100f);

        if (roll <= commonChance) return Rarity.Common;
        if (roll <= commonChance + uncommonChance) return Rarity.Uncommon;
        if (roll <= commonChance + uncommonChance + rareChance) return Rarity.Rare;
        return Rarity.Legendary;
    }
}
