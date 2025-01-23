using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashBehavior : MonoBehaviour
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

    public void OpenStash()
    {
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.chestOpeningSound);
        HudManager.Instance.OpenStashUI();
    }

    private void OnMouseOver()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.warnBtn.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                OpenStash();
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
}


