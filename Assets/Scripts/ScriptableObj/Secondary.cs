using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Secondarys/Secondary")]
public class Secondary : ItemBase
{
    public enum SecondaryType { Empty, Shield, Orb}
    [Header("Weapon Type")]
    public SecondaryType secondaryType;

    public GameObject secondaryPrefab;
}