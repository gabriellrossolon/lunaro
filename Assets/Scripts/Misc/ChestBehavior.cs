using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehavior : MonoBehaviour
{
    private Animator animator;
    public GameObject dropBag;
    public GameObject dropPoint;
    private GameObject player;
    public List<ItemBase> chestItems = new List<ItemBase>();
    public float maxDistance;

    private Collider chestCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        chestCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;
    }

    public void OpenChest()
    {
        animator.SetTrigger("Open");
        StartCoroutine(InstantiateItemsWithDelay());
    }

    private IEnumerator InstantiateItemsWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (ItemBase item in chestItems)
        {
            GameObject newDropBag = Instantiate(dropBag, dropPoint.transform.position, Quaternion.identity);
            DroppedItem droppedItem = newDropBag.GetComponent<DroppedItem>();

            if (droppedItem != null)
            {
                droppedItem.item = item;
            }

            yield return new WaitForSeconds(0.5f);
        }

        DisableChestBehavior();  // Desativa o script e o collider após instanciar os itens
    }

    private void DisableChestBehavior()
    {
        enabled = false;  // Desativa o script ChestBehavior
        if (chestCollider != null)
        {
            chestCollider.enabled = false;  // Desativa o Collider, impedindo a detecção de OnMouseOver e OnMouseExit
        }
    }

    private void OnMouseOver()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.warnBtn.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenChest();
            }
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



