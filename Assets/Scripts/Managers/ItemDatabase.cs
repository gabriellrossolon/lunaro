using UnityEngine;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    public List<ItemBase> commonItems = new List<ItemBase>();
    public List<ItemBase> uncommonItems = new List<ItemBase>();
    public List<ItemBase> rareItems = new List<ItemBase>();
    public List<ItemBase> epicItems = new List<ItemBase>();
    public List<ItemBase> legendaryItems = new List<ItemBase>();

    public List<ItemBase> allItems = new List<ItemBase>(); // Lista contendo todos os itens

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        OrganizeItemsByRarity();
    }

    private void OrganizeItemsByRarity()
    {
        commonItems.Clear();
        uncommonItems.Clear();
        rareItems.Clear();
        epicItems.Clear();
        legendaryItems.Clear();

        foreach (var item in allItems)
        {
            switch (item.itemRarity)
            {
                case ItemBase.ItemRarity.Common:
                    commonItems.Add(item);
                    break;
                case ItemBase.ItemRarity.Uncommon:
                    uncommonItems.Add(item);
                    break;
                case ItemBase.ItemRarity.Rare:
                    rareItems.Add(item);
                    break;
                case ItemBase.ItemRarity.Epic:
                    epicItems.Add(item);
                    break;
                case ItemBase.ItemRarity.Legendary:
                    legendaryItems.Add(item);
                    break;
            }
        }
    }

    public List<ItemBase> GetItemsByRarity(ItemBase.ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemBase.ItemRarity.Common:
                return commonItems;
            case ItemBase.ItemRarity.Uncommon:
                return uncommonItems;
            case ItemBase.ItemRarity.Rare:
                return rareItems;
            case ItemBase.ItemRarity.Epic:
                return epicItems;
            case ItemBase.ItemRarity.Legendary:
                return legendaryItems;
            default:
                return null;
        }
    }
}
