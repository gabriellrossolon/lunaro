using UnityEngine;

[CreateAssetMenu(fileName = "NewSecondary", menuName = "Equippables/Secondarys/Shield")]
public class Shield : Secondary
{
    [Header("If Shield")]
    public AudioClip blockSound;
    public float blockResourceCost;
}
