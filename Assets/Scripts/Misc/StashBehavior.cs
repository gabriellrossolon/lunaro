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
        HudManager.Instance.OpenStashUI();
    }
}


