using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool checkpointActived = false;
    [SerializeField] private bool checkpointDiscovered = false;

    private GameObject player;
    private Collider checkCollider;
    private AudioSource soundSource;
    private AudioClip checkpointActiveSound;

    public float maxDistance = 2f;

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;
        checkCollider = GetComponent<Collider>();
        soundSource = GetComponent<AudioSource>();
    }

    private void OnMouseOver()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.warnBtn.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                ActiveCheckPoint();
        }
        else
        {
            HudManager.Instance.warnBtn.SetActive(false);
            HudManager.Instance.itemShow.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        HudManager.Instance.warnBtn.SetActive(false);
        HudManager.Instance.itemShow.SetActive(false);
    }

    private void ActiveCheckPoint()
    {
        checkpointDiscovered = true;
        checkpointActived = true;

        if (checkCollider != null)
            checkCollider.enabled = false;

        soundSource.PlayOneShot(checkpointActiveSound);

        // Desativa o checkpoint anterior
        var allCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (var checkpoint in allCheckpoints)
        {
            if (checkpoint != gameObject)
            {
                checkpoint.GetComponent<Checkpoint>().checkpointActived = false;
            }
        }
    }
}
