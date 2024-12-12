using UnityEngine;

[CreateAssetMenu(fileName = "NewPotion", menuName = "Potions/Potion")]
public class Potion : ScriptableObject
{
    public string potionName;
    public Sprite icon;
}
