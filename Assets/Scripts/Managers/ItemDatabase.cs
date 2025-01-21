using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    public List<ItemBase> commonItems = new List<ItemBase>();
    public List<ItemBase> uncommonItems = new List<ItemBase>();
    public List<ItemBase> rareItems = new List<ItemBase>();
    public List<ItemBase> legendaryItems = new List<ItemBase>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<ItemBase> GetItemsByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                return commonItems;
            case Rarity.Uncommon:
                return uncommonItems;
            case Rarity.Rare:
                return rareItems;
            case Rarity.Legendary:
                return legendaryItems;
            default:
                return null;
        }
    }
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}
