using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public ScriptableObject item;
    public float maxDistance = 2f;
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnMouseOver()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= maxDistance)
        {
            HudManager.Instance.warnBtn.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                InventoryManager.Instance.AddItem(item);
                HudManager.Instance.warnBtn.SetActive(false);
                Destroy(gameObject);
            }
        }
        else
        {
            HudManager.Instance.warnBtn.SetActive(false);
        }      
    }

    private void OnMouseExit()
    {
        HudManager.Instance.warnBtn.SetActive(false);
    }
}



