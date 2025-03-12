using UnityEngine;

public class StashBehavior : MonoBehaviour, IInteractable
{
    private Animator animator;
    private GameObject player;
    public float maxDistance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;
    }

    private void Update()
    {
        animator.SetBool("Opened", HudManager.Instance.openedStash);
    }

    public void Interact()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.warnBtn.SetActive(true);
            HudManager.Instance.OpenStashUI();
        }
        else
        {
            HudManager.Instance.warnBtn.SetActive(false);
            HudManager.Instance.itemShow.SetActive(false);
        }
    }
}


