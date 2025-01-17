using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
        DontDestroyOnLoad(gameObject);
    }

    [Header("Misc References")]
    public GameObject inventory;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerInput playerInput;
    public GameObject warnBtn;
    public GameObject globalVolume;
    private PlayerStatus playerStatus;
    public Texture2D cursorPointer;
    public Texture2D cursorClicker;
    public GameObject teleportsPage;
    public TextMeshProUGUI potionCount;
    public GameObject canvasObj;
    public GameObject pauseUI;
    public GameObject optionsUI;

    [Header("ItemShowUI")]
    public GameObject itemShow;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemDmg;
    public TextMeshProUGUI itemResourceCost;
    public TextMeshProUGUI itemDesc;

    [Header("Controllers")]
    private bool openedInventory = false;
    private bool hidedCanvas = false;
    private bool openedOptionsUI = false;

    [Header("Player Status")]
    public Slider hpSlider;
    public Slider hpLateSlider;
    public float lateSliderSpeed = 0.5f;
    public Slider staminaSlider;
    public Slider manaSlider;

    private Color _targetVignetteColor = Color.black;
    private float _targetSmoothness = 0.1f;
    private float _transitionDuration = 0.5f;
    private float _transitionTime = 0f;

    private bool openedTeleportPage = false;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();

        UpdateHUDStats();
        //hpLateSlider.value = playerStatus.currentHp;
    }

    private void Update()
    {
        UpdateCursorState();
        OpenInventory();
        HideCanvas();
        SmoothVignetteTransition();

        UpdateHUDStats();
        potionCount.enabled = InventoryManager.Instance.equippedPotion != null;
        potionCount.text = InventoryManager.Instance.potionsCount.ToString();
    }

    private void UpdateCursorState()
    {
        bool hudIsOpen = openedInventory || openedTeleportPage || GameManager.Instance.gamePaused || (MenuManager.Instance != null && MenuManager.Instance.isActiveAndEnabled);

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

    public void UpdateHUDStats()
    {
        hpSlider.maxValue = playerStatus.maxHp;
        hpLateSlider.maxValue = playerStatus.maxHp;
        staminaSlider.maxValue = playerStatus.maxStamina;
        manaSlider.maxValue = playerStatus.maxMana;

        hpSlider.value = playerStatus.currentHp;
        staminaSlider.value = playerStatus.currentStamina;
        manaSlider.value = playerStatus.currentMana;
        if (hpLateSlider.value != playerStatus.currentHp)
        {
            hpLateSlider.value = Mathf.Lerp(hpLateSlider.value, playerStatus.currentHp, Time.deltaTime * lateSliderSpeed);
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

    private void HideCanvas()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            hidedCanvas = !hidedCanvas;
            canvasObj.SetActive(!hidedCanvas);
        }
    }

    public void OpenTeleportPage()
    {
        openedTeleportPage = !openedTeleportPage;
        teleportsPage.SetActive(openedTeleportPage);
    }

    public void RefreshItemShow(ItemBase item)
    {
        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDesc.text = item.description;
        if (item is Weapon weapon)
        {
            itemDmg.text = "Damage  " + weapon.damage.ToString();

            if (weapon.weaponType == Weapon.WeaponType.Sword)
            {
                itemType.text = "Sword";
                itemResourceCost.text = weapon.resourceCost.ToString() + " Stamina to Attack";
            }
            else if (weapon.weaponType == Weapon.WeaponType.Staff)
            {
                itemType.text = "Staff";
                itemResourceCost.text = weapon.resourceCost.ToString() + " Mana to Cast";
            }
            else if (weapon.weaponType == Weapon.WeaponType.Hammer)
            {
                itemType.text = "Hammer";
                itemResourceCost.text = weapon.resourceCost.ToString() + " Stamina to Attack";
            }
            else
            {
                itemType.text = "Unknown Weapon";
                itemResourceCost.text = null;
            }
        }    
        else if (item is Secondary secondary)
        {
            if (secondary.secondaryType == Secondary.SecondaryType.Shield)
            {
                itemType.text = "Shield";
                itemResourceCost.text = secondary.resourceCost.ToString() + " Stamina to Block";
            }
            else if (secondary.secondaryType == Secondary.SecondaryType.Orb)
            {
                itemType.text = "Magical Orb";
                itemResourceCost.text = null;
            }
            else if (secondary.secondaryType == Secondary.SecondaryType.Book)
            {
                itemType.text = "Magical Book";
                itemResourceCost.text = null;
            }
            else
            {
                itemType.text = "Unknown Weapon";
                itemResourceCost.text = null;
            }
            itemDmg.text = null;
            itemResourceCost.text = null;
        }
        else if (item is Potion potion)
        {
            itemType.text = "Potion";

            itemDmg.text = "Recover " + potion.valueToRecover.ToString() + " " + potion.potionType.ToString() + " points";
            itemResourceCost.text = null;
        }
        else if (item is Jewel jewel)
        {
            if (jewel.jewelType == Jewel.JewelType.Ring)
            {
                itemType.text = "Ring";
            }
            else if (jewel.jewelType == Jewel.JewelType.Necklace)
            {
                itemType.text = "Necklace";
            }
            else if (jewel.jewelType == Jewel.JewelType.Amulet)
            {
                itemType.text = "Amulet";
            }
            itemDmg.text = null;
            itemResourceCost.text = null;
        }
        else
        {
            itemType.text = "Unknown Item";
            itemDmg.text = "Unknown Item";
            itemResourceCost.text = null;
        }
    }

    public void ChangeVignetteColorToRed()
    {
        _targetVignetteColor = Color.red;
        _targetSmoothness = 0.6f; // Ajustando o smoothness para 0.6
        _transitionTime = 0f;
    }

    private void SmoothVignetteTransition()
    {
        Volume volume = globalVolume.GetComponent<Volume>();
        if (volume != null)
        {
            Vignette vignette;
            if (volume.profile.TryGet<Vignette>(out vignette))
            {
                _transitionTime += Time.deltaTime / _transitionDuration;
                vignette.color.Override(Color.Lerp(vignette.color.value, _targetVignetteColor, _transitionTime));
                vignette.smoothness.Override(Mathf.Lerp(vignette.smoothness.value, _targetSmoothness, _transitionTime));

                if (_transitionTime >= 0.6f)
                {
                    if (_targetSmoothness == 0.6f)
                    {
                        _targetVignetteColor = Color.black;
                        _targetSmoothness = 0.1f;
                        _transitionTime = 0f;
                    }
                }
            }
        }
    }

    public void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OpenOption()
    {
        openedOptionsUI = !openedOptionsUI;

        optionsUI.SetActive(openedOptionsUI);
        if (openedOptionsUI)
            VolumeManager.Instance.InitializeVolumeSlider();
    }
}
