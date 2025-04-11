using UnityEngine;

public class StashBehavior : MonoBehaviour, IInteractable
{
    private Animator animator;
    public float maxDistance;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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


