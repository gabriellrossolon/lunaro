using UnityEngine;

public class Checkpoint : MonoBehaviour, IInteractable
{
    public bool checkpointActived = false;
    [SerializeField] private bool checkpointDiscovered = false;

    private Collider checkCollider;
    private AudioSource soundSource;
    [SerializeField] private AudioClip checkpointActiveSound;

    [SerializeField] private GameObject fireFx;
    [SerializeField] private GameObject swordObj;

    public float maxDistance = 2f;

    private void Start()
    {
        checkCollider = GetComponent<Collider>();
        soundSource = GetComponent<AudioSource>();

        UpdateBehavior();
    }

    private void UpdateBehavior()
    {
        swordObj.SetActive(checkpointDiscovered);
        fireFx.SetActive(checkpointActived);
    }

    public void Interact()
    {
        checkpointDiscovered = true;
        checkpointActived = true;

        soundSource.PlayOneShot(checkpointActiveSound);

        var allCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var checkpoint in allCheckpoints)
        {
            if (checkpoint != gameObject)
            {
                checkpoint.GetComponent<Checkpoint>().checkpointActived = false;
                checkpoint.GetComponent<Checkpoint>().UpdateBehavior();
            }
        }
    }
}