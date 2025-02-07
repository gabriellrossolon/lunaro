using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using System.Resources;

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

    [Header("Scripts/Components References")]
    public CinemachineVirtualCamera virtualCamera;
    public PlayerInput playerInput;
    private PlayerStatus playerStatus;
    private LevelBehavior levelBehavior;
    public GameObject globalVolume;
    public GameObject canvasObj;

    [Header("Misc References")]
    public GameObject warnBtn;
    public Texture2D cursorPointer;
    public Texture2D cursorClicker;
    public GameObject teleportsPage;
    public GameObject deathScreen;

    [Header("Inventory")]
    public GameObject inventory;
    public GameObject stash;
    public TextMeshProUGUI playerGoldText;

    [Header("Esc | Options")]
    public GameObject pauseUI;
    public GameObject optionsUI;
    public Toggle fullScreenCheckbox;

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

    [Header("Hotbar References")]
    public TextMeshProUGUI potionCount;
    public GameObject shortcuts;
    [HideInInspector] public Animator shortcutsAnim;

    public Slider dashCooldownSlider;
    public Image dashImage;
    public Image dashSliderImage;
    public Slider skillCooldownSlider;
    public Image skillImage;
    public Image skillSliderImage;
    public Slider magicCooldownSlider;
    public Image magicImage;
    public Image magicSliderImage;

    [Header("Controllers")]
    private bool openedInventory = false;
    private bool hidedCanvas = false;
    private bool openedOptionsUI = false;
    [HideInInspector] public bool openedStash = false;
    private bool isFullScreen = true;
    private bool openedTeleportPage = false;

    [Header("Player Status")]
    public Slider hpSlider;
    public Slider hpLateSlider;
    public float lateSliderSpeed = 0.5f;
    public Slider staminaSlider;
    public Slider manaSlider;

    [Header("Transitions Effects")]
    private Color _targetVignetteColor = Color.black;
    private float _targetSmoothness = 0.1f;
    private float _transitionDuration = 0.5f;
    private float _transitionTime = 0f;

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        levelBehavior = PlayerManager.Instance.GetComponent<LevelBehavior>();
        shortcutsAnim = shortcuts.GetComponent<Animator>();

        UpdateHUDLevel();
        UpdateHUDStats();

        skillImage.enabled = false;
        skillSliderImage.enabled = false;
        magicImage.enabled = false;
        magicSliderImage.enabled = false;
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
            switch (weapon)
            {
                case Sword sword:
                    itemType.text = "Sword";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case Staff staff:
                    itemType.text = "Staff";
                    itemResourceCost.text = resourceText + " Mana to Cast";
                    break;
                case Hammer hammer:
                    itemType.text = "Hammer";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case Bow bow:
                    itemType.text = "Bow (2Hand)";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case GreatHammer greatHammer:
                    itemType.text = "Great Hammer (2Hand)";
                    itemResourceCost.text = resourceText + " Stamina to Attack";
                    break;
                case GreatSword greatSword:
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
            string resourceText = null;
            itemDmg.text = null;

            switch (secondary)
            {
                case Shield shield:
                    itemType.text = "Shield";
                    resourceText = shield.blockResourceCost.ToString();
                    itemResourceCost.text = resourceText + " Stamina to Block";
                    break;
                case Orb orb:
                    itemType.text = "Magical Orb";
                    resourceText = orb.spell.spellResourceCost.ToString();
                    itemResourceCost.text = resourceText + " Mana to Cast Spell";
                    break;
                case Book book:
                    itemType.text = "Magical Book";
                    resourceText = book.spell.spellResourceCost.ToString();
                    itemResourceCost.text = resourceText + " Mana to Cast Spell";
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
            string statModified = jewel.statToModify.ToString();
            string effectAdded()
            {
                if (jewel.effectType is Jewel.EffectType.IncreaseBase)
                    return "Base";
                else if (jewel.effectType is Jewel.EffectType.IncreaseRegeneration)
                    return "Regeneration";
                else if (jewel.effectType is Jewel.EffectType.IncreasePercent)
                    return "Percent";
                else return null;
            }
            string valueChanged = jewel.valueToAdd.ToString();

            itemDmg.text = "Add " + valueChanged + " to " + statModified + " " + effectAdded();

            //itemDmg.text = null;
            itemResourceCost.text = null;

            if (jewel is Ring)
            {
                itemType.text = "Ring";
            }
            else if (jewel is Necklace)
            {
                itemType.text = "Necklace";
            }
            else if (jewel is Amulet)
            {
                itemType.text = "Amulet";
            }
            else
            {
                itemType.text = "Unknown Jewel";
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

    public void ChangeToDeathUI()
    {
        Volume volume = globalVolume.GetComponent<Volume>();

        if (volume != null && volume.profile != null)
        {
            if (volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustments))
            {
                if (volume.profile.TryGet<Vignette>(out Vignette vignette))
                {
                    StartCoroutine(FadeEffects(colorAdjustments, vignette, -100f, 1f, 3f));
                }
            }
        }
    }

    private IEnumerator FadeEffects(ColorAdjustments colorAdjustments, Vignette vignette, float targetSaturation, float targetVignette, float duration)
    {
        var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        pov.m_HorizontalAxis.m_MaxSpeed = 0;
        pov.m_VerticalAxis.m_MaxSpeed = 0;

        float startSaturation = colorAdjustments.saturation.value;
        float startVignette = vignette.intensity.value;
        float startAimValue = pov.m_VerticalAxis.Value;
        float targetAimValue = -80f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            colorAdjustments.saturation.value = Mathf.Lerp(startSaturation, targetSaturation, t);
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, t);
            pov.m_VerticalAxis.Value = Mathf.Lerp(startAimValue, targetAimValue, t);

            yield return null;
        }
        deathScreen.SetActive(true);

        colorAdjustments.saturation.value = targetSaturation;
        vignette.intensity.value = targetVignette;
        pov.m_VerticalAxis.Value = targetAimValue;
    }


    public void ChangeToLiveUI()
    {
        Volume volume = globalVolume.GetComponent<Volume>();

        if (volume != null && volume.profile != null)
        {
            if (volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdjustments))
            {
                colorAdjustments.saturation.value = 0f;
            }

            if (volume.profile.TryGet<Vignette>(out Vignette vignette))
            {
                vignette.intensity.value = 0.45f; 
            }
        }

        deathScreen.SetActive(false);

        var pov = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
        pov.m_VerticalAxis.Value = 0f;
        pov.m_HorizontalAxis.m_MaxSpeed = 2f;
        pov.m_VerticalAxis.m_MaxSpeed = 2f;
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
