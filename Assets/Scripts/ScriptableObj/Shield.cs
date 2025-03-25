using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondarys/Shield")]
public class Shield : Secondary
{
    public AudioClip blockSound;

    [Range(1, 100)]
    public float blockResourceCost;
    [Range(1, 100)]
    public int blockPercentValue;
}
