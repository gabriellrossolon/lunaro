using UnityEngine;

[System.Serializable]
[ES3Serializable]
public class ItemBase : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public string description;
    public enum ItemRarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public ItemRarity itemRarity;
}
