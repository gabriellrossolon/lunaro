using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondary")]
public class Secondary : ItemBase
{
    public enum SecondaryType { Empty, Shield, Orb, Book}
    [Header("Weapon Type")]
    public SecondaryType secondaryType;

    public GameObject secondaryPrefab;

    [Header("If Shield")]
    public AudioClip blockSound;
    public float resourceCost;

}