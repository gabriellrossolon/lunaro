using UnityEngine;

[CreateAssetMenu(fileName = "NewPotion", menuName = "Consumables/Potion")]
public class Potion : ItemBase
{
    public enum PotionType { Empty, Health, Stamina, Mana }
    public PotionType potionType;

    public int valueToRecover;
}
