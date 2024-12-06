using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public float playerGravity;

    public float walkSpeed;
    public float runSpeed;
    public float actualSpeed;

    public float jumpHeight;

    private void Start()
    {
        {
            actualSpeed = walkSpeed;
        }
    }
}
