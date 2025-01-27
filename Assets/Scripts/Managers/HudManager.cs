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
    public Texture2D cursorPointer;
    public Texture2D cursorClicker;
    public GameObject teleportsPage;
    public TextMeshProUGUI potionCount;
    public GameObject canvasObj;
    public GameObject pauseUI;
    public GameObject optionsUI;
    public GameObject stash;
    public Toggle fullScreenCheckbox;
    public TextMeshProUGUI playerGoldText;
    public GameObject shortcuts;
    [HideInInspector] public Animator shortcutsAnim;
    private PlayerStatus playerStatus;
    private LevelBehavior levelBehavior;

    [Header("ItemShowUI")]
    public GameObject itemShow;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemDmg;
    public TextMeshProUGUI itemResourceCost;
    public TextMeshProUGUI itemDesc;

    [Header("Player Level References")]
    public TextMeshProUGUI playerLevel;
    public Slider playerExperience;

    [Header("Controllers")]
    private bool openedInventory = false;
    private bool hidedCanvas = false;
    private bool openedOptionsUI = false;
    [HideInInspector] public bool openedStash = false;
    private bool isFullScreen = true;

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
        levelBehavior = PlayerManager.Instance.GetComponent<LevelBehavior>();
        shortcutsAnim = shortcuts.GetComponent<Animator>();

        UpdateHUDLevel();
        UpdateHUDStats();
    }

    private void Update()
    {
        UpdateCursorState();

        if(Input.GetKeyDown(KeyCode.B))
            OpenInventory();

        HideCanvas();
        SmoothVignetteTransition();

        UpdateHUDStats();
        potionCount.enabled = InventoryManager.Instance.equippedPotion != null;
        potionCount.text = InventoryManager.Instance.potionsCount.ToString();

        playerGoldText.text = playerStatus.playerGold.ToString();
    }

    private void UpdateCursorState()
    {
        bool hudIsOpen = openedInventory || 
            openedTeleportPage || 
            GameManager.Instance.gamePaused || 
            (MenuManager.Instance != null && MenuManager.Instance.isActiveAndEnabled || 
            openedStash);

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
            hpLateSlider.value = Mathf.Lerp(hpLateSlider.value, playerStatus.currentHp, Time.deltaTime * lateSliderSpeed);
    }

    public void UpdateHUDLevel()
    {
        playerExperience.maxValue = levelBehavior.experienceToUp;
        playerExperience.value = levelBehavior.experience;

        playerLevel.text = levelBehavior.level.ToString();
    }

    //OPEN UI FUNCTIONS
    private void OpenInventory()
    {
        openedInventory = !openedInventory;
        inventory.GetComponent<Animator>().SetBool("openedInventory", openedInventory);

        itemShow.SetActive(false);
    }

    public void OpenStashUI()
    {
        openedStash = !openedStash;
        stash.SetActive(openedStash);

        itemShow.SetActive(false);
    }

    public void OpenTeleportPage()
    {
        openedTeleportPage = !openedTeleportPage;
        teleportsPage.SetActive(openedTeleportPage);
    }

    public void OpenOption()
    {
        openedOptionsUI = !openedOptionsUI;

        optionsUI.SetActive(openedOptionsUI);
        if (openedOptionsUI)
        {
            VolumeManager.Instance.InitializeVolumeSlider();
            VolumeManager.Instance.InitializeScreenCheckbox();
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

    //---

    public void RefreshItemShow(ItemBase item)
    {
        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;
        itemDesc.text = item.description;

        if (item is Weapon weapon)
        {
            itemDmg.text = "Damage  " + weapon.damage.ToString();

            string resourceText = weapon.resourceCost.ToString();
            switch (weapon.weaponType)
            {
                case Weapon.WeaponType.Sword:
                    itemType.text = "Sword";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case Weapon.WeaponType.Staff:
                    itemType.text = "Staff";
                    itemResourceCost.text = resourceText + " Mana to Cast";
                    break;
                case Weapon.WeaponType.Hammer:
                    itemType.text = "Hammer";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case Weapon.WeaponType.Bow:
                    itemType.text = "Bow (2Hand)";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case Weapon.WeaponType.GreatHammer:
                    itemType.text = "Great Hammer (2Hand)";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case Weapon.WeaponType.GreatSword:
                    itemType.text = "Great Sword (2Hand)";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                default:
                    itemType.text = "Unknown Weapon";
                    itemResourceCost.text = null;
                    break;
            }
        }
        else if (item is Secondary secondary)
        {
            itemDmg.text = null;
            string resourceText = secondary.resourceCost.ToString();

            switch (secondary.secondaryType)
            {
                case Secondary.SecondaryType.Shield:
                    itemType.text = "Shield";
                    itemResourceCost.text = resourceText + " Stamina to Block";
                    break;
                case Secondary.SecondaryType.Orb:
                    itemType.text = "Magical Orb";
                    itemResourceCost.text = null;
                    break;
                case Secondary.SecondaryType.Book:
                    itemType.text = "Magical Book";
                    itemResourceCost.text = null;
                    break;
                default:
                    itemType.text = "Unknown Secondary";
                    itemResourceCost.text = null;
                    break;
            }
        }
        else if (item is Potion potion)
        {
            itemType.text = "Potion";
            itemDmg.text = "Recover " + potion.valueToRecover.ToString() + " " + potion.potionType.ToString() + " points";
            itemResourceCost.text = null;
        }
        else if (item is Jewel jewel)
        {
            itemDmg.text = null;
            itemResourceCost.text = null;

            switch (jewel.jewelType)
            {
                case Jewel.JewelType.Ring:
                    itemType.text = "Ring";
                    break;
                case Jewel.JewelType.Necklace:
                    itemType.text = "Necklace";
                    break;
                case Jewel.JewelType.Amulet:
                    itemType.text = "Amulet";
                    break;
                default:
                    itemType.text = "Unknown Jewel";
                    break;
            }
        }
        else
        {
            itemType.text = "Unknown Item";
            itemDmg.text = "Unknown Item";
            itemResourceCost.text = null;
        }
    }


    //CONTROLE DA VIGNETTE (EFEITO AO LEVAR DANO)
    public void ChangeVignetteColorToRed()
    {
        _targetVignetteColor = Color.red;
        _targetSmoothness = 0.6f; 
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

    //ELEMENTOS AVULSOS
    public void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OnFullScreenToggleChanged()
    {
        isFullScreen = !isFullScreen;

        Screen.fullScreen = isFullScreen;

        if (isFullScreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            Screen.SetResolution(1280, 720, false);
        }

        VolumeManager.Instance.isFullScreen = isFullScreen;
    }
}
