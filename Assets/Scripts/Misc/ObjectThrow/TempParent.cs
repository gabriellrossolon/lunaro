using UnityEngine;

public class TempParent : MonoBehaviour
{
    public bool isHoldingAnything;
    public static TempParent Instance {  get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}


