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

    [Header("References")]
    public GameObject inventory;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerInput playerInput;
    public GameObject warnBtn;
    public GameObject globalVolume;
    private PlayerStatus playerStatus;

    [Header("ItemShowUI")]
    public GameObject itemShow;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemType;
    public TextMeshProUGUI itemDmg;
    public TextMeshProUGUI itemDesc;

    [Header("Controllers")]
    private bool openedInventory = false;

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

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();

        hpSlider.maxValue = playerStatus.maxHp;
        hpLateSlider.maxValue = playerStatus.maxHp;
        staminaSlider.maxValue = playerStatus.maxStamina;
        manaSlider.maxValue = playerStatus.maxMana;

        hpSlider.value = playerStatus.currentHp;
        hpLateSlider.value = playerStatus.currentHp;
        staminaSlider.value = playerStatus.currentStamina;
        manaSlider.value = playerStatus.currentMana;
    }

    private void Update()
    {
        UpdateCursorState();
        OpenInventory();
        SmoothVignetteTransition();

        hpSlider.value = playerStatus.currentHp;
        staminaSlider.value = playerStatus.currentStamina;
        manaSlider.value = playerStatus.currentMana;

        // Atualiza o valor do hpLateSlider suavemente
        if (hpLateSlider.value != playerStatus.currentHp)
        {
            hpLateSlider.value = Mathf.Lerp(hpLateSlider.value, playerStatus.currentHp, Time.deltaTime * lateSliderSpeed);
        }
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

            if (weapon.weaponType == Weapon.WeaponType.Sword)
            {
                itemType.text = "Sword";
            }
            else if (weapon.weaponType == Weapon.WeaponType.Staff)
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
        else if (item is Secondary secondary)
        {
            if (secondary.secondaryType == Secondary.SecondaryType.Shield)
            {
                itemType.text = "Shield";
            }
            else if (secondary.secondaryType == Secondary.SecondaryType.Orb)
            {
                itemType.text = "Magical Orb";
            }
            else if (secondary.secondaryType == Secondary.SecondaryType.Book)
            {
                itemType.text = "Magical Book";
            }
            else
            {
                itemType.text = "Unknown Weapon";
            }
            itemDmg.text = null;
        }
        else
        {
            itemType.text = "Unknown Item";
            itemDmg.text = "Unknown Item";
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
}
