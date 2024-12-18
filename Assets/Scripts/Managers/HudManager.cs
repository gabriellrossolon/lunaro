using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Header("References")]
    public GameObject inventory;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerInput playerInput;
    public GameObject warnBtn;

    [Header("ItemShowUI")]
    public GameObject itemShow;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemDmg;
    public TextMeshProUGUI itemDesc;

    [Header("Controllers")]
    private bool openedInventory = false;

    private void Update()
    {
        UpdateCursorState();
        OpenInventory();
    }

    private void UpdateCursorState()
    {
        bool hudIsOpen = openedInventory;

        if (hudIsOpen)
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
            virtualCamera.enabled = false;
            playerInput.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            virtualCamera.enabled = true;
            playerInput.enabled = true;
        }
    }

    private void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            openedInventory = !openedInventory;
            inventory.SetActive(openedInventory);

            itemShow.SetActive(false);
        }
    }

    public void RefreshItemShow(ItemBase item)
    {
        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDesc.text = item.description;
        if (item is Weapon weapon)
        {
            itemDmg.text = "Damage  " + weapon.damage.ToString();

            if (weapon.sword)
            {
                itemType.text = "Sword";
            }
            else if (weapon.staff)
            {
                itemType.text = "Staff";
            }
            else
            {
                itemType.text = "Unknown Weapon";
            }
        }
        else if (item is Potion potion)
        {
            itemType.text = "Potion";

            itemDmg.text = null;
        }
        else
        {
            itemType.text = "Unknown Item";
            itemDmg.text = "Unknown Item";
        }
    }
}
