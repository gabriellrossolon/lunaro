using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Potion;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Player References")]
    public GameObject handsPOV;
    public GameObject rightHandHolder;
    public GameObject leftHandHolder;
    private PlayerStatus playerStatus;
    private InputHandler _inputHandler;
    public WeaponBehavior rightWeaponBehavior;
    public SecondaryBehavior secondaryBehavior;

    [Header("Inventory References")]
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;

    [Header("Stash References")]
    public Transform stashGrid;

    [Header("Inventory")]
    public List<ItemBase> inventoryItems = new List<ItemBase>();

    [Header("Stash")]
    public List<ItemBase> stashItems = new List<ItemBase>();

    [Header("Equipped Items")]
    [SerializeField] public Weapon equippedWeapon;
    [SerializeField] public Secondary equippedSecondary;
    [SerializeField] private Jewel equippedNecklace;
    [SerializeField] private Jewel equippedRing;
    [SerializeField] private Jewel equippedAmulet;

    public Potion equippedPotion;
    public int potionsCount = 3;
    public int maxPotionsCount = 3;
    private float potionRegenerationTimer = 0f;
    [SerializeField] private float potionRegenerationCooldown = 60f; 

    private GameObject lastInstantiatedWeapon;
    private GameObject lastInstantiatedSecondary;
    private Jewel lastNecklaceEquipped;
    private Jewel lastRingEquipped;
    private Jewel lastAmuletEquipped;

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

    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        _inputHandler = PlayerManager.Instance.GetComponent<InputHandler>();
        GenerateInventorySlots();
        GenerateStashSlots();
    }

    private void Update()
    {
        PotionControll();
    }

    //CONTROLADORES DE ADIÇÂO OU REMOÇÂO DE ITENS DA LIST INVENTORY
    public void AddItem(ItemBase item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] == null) 
            {
                inventoryItems[i] = item;
                GenerateInventorySlots();
                GenerateStashSlots();
                return;
            }
        }

        inventoryItems.Add(item);
        GenerateInventorySlots(); 
        GenerateStashSlots();
    }
    public void RemoveItem(ItemBase item)
    {
        int index = inventoryItems.IndexOf(item);
        if (index >= 0)
        {
            inventoryItems[index] = null; 
            GenerateInventorySlots(); 
        }
    }

    //CONTROLADORES DE VINCULO DE ITEM AO SLOT DE ITEM EQUIPADO
    public void EquipWeaponFromInventory(Weapon weapon)
    {
        equippedWeapon = weapon;
        EquipWeapon();
    }
    public void EquipSecondaryFromInventory(Secondary secondary)
    {
        equippedSecondary = secondary;
        EquipSecondary();
    }
    public void EquipJewelFromInventory(Jewel jewel, Jewel.JewelType jewelType)
    {
        if (jewel == null)
        {
            switch (jewelType)
            {
                case Jewel.JewelType.Ring:
                    RemoveJewelFunction(lastRingEquipped);
                    equippedRing = null; 
                    lastRingEquipped = null; 
                    break;

                case Jewel.JewelType.Necklace:
                    RemoveJewelFunction(lastNecklaceEquipped);
                    equippedNecklace = null; 
                    lastNecklaceEquipped = null; 
                    break;

                case Jewel.JewelType.Amulet:
                    RemoveJewelFunction(lastAmuletEquipped);
                    equippedAmulet = null; 
                    lastAmuletEquipped = null;
                    break;

                default:
                    break;
            }
        }
        else
        {
            switch (jewel.jewelType)
            {
                case Jewel.JewelType.Ring:
                    if (lastRingEquipped != null) { RemoveJewelFunction(lastRingEquipped); }
                    equippedRing = jewel;
                    lastRingEquipped = jewel; 
                    break;

                case Jewel.JewelType.Necklace:
                    if (lastNecklaceEquipped != null) { RemoveJewelFunction(lastNecklaceEquipped); }
                    equippedNecklace = jewel;
                    lastNecklaceEquipped = jewel; 
                    break;

                case Jewel.JewelType.Amulet:
                    if (lastAmuletEquipped != null) { RemoveJewelFunction(lastAmuletEquipped); }
                    equippedAmulet = jewel;
                    lastAmuletEquipped = jewel; 
                    break;

                default:
                    break;
            }

            AddJewelFunction(jewel);
        }
    }
    public void EquipPotionFromInventory(Potion potion)
    {
        equippedPotion = potion;
    }

    //CONTROLADORES DE INSTANCIAMENTO DO ITEM NA MÂO DO JOGADOR
    public void EquipWeapon()
    {
        if (lastInstantiatedWeapon != null)
            Destroy(lastInstantiatedWeapon);

        if (equippedWeapon == null || equippedWeapon.weaponPrefab == null)
        {
            ResetWeaponBehavior();
            rightWeaponBehavior.SetWeaponType(Weapon.WeaponType.Empty);
            return;
        }

        InstantiateAndSetupWeapon();
    }
    public void EquipSecondary()
    {
        if (lastInstantiatedSecondary != null)
            Destroy(lastInstantiatedSecondary);

        if (equippedSecondary == null || equippedSecondary.secondaryPrefab == null)
        {
            ResetSecondaryBehavior();
            return;
        }

        InstantiateAndSetupSecondary();
    }

    //CONTROLADORES DE APLICAÇÂO DO EFEITO PASSIVO DAS JEWELS AO JOGADOR
    private void ApplyJewelEffect(Jewel jewel, float value)
    {
        Jewel.StatType statToModify = jewel.statToModify;
        Jewel.EffectType effectType = jewel.effectType;

        switch (statToModify)
        {
            case Jewel.StatType.Damage:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    playerStatus.ControlBaseDamage(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    playerStatus.ControlDamagePercent(value * jewel.valueToAdd);
                break;

            case Jewel.StatType.HP:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    playerStatus.ControlBaseHealth(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreaseRegeneration)
                    playerStatus.ControlBaseHealthRegeneration(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    playerStatus.ControlHealthPercent(value * jewel.valueToAdd);
                break;

            case Jewel.StatType.Stamina:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    playerStatus.ControlBaseStamina(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreaseRegeneration)
                    playerStatus.ControlBaseStaminaRegeneration(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    playerStatus.ControlStaminaPercent(value * jewel.valueToAdd);
                break;

            case Jewel.StatType.Mana:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    playerStatus.ControlBaseMana(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreaseRegeneration)
                    playerStatus.ControlBaseManaRegeneration(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    playerStatus.ControlManaPercent(value * jewel.valueToAdd);
                break;
        }
    }
    public void AddJewelFunction(Jewel jewel)
    {
        ApplyJewelEffect(jewel, 1); 
    }
    public void RemoveJewelFunction(Jewel jewel)
    {
        ApplyJewelEffect(jewel, -1); 
    }

    //RESETS DOS ENUMS DE ITEM EQUIPADO NOS WEAPON E SECONDARY SCRIPTS
    private void ResetWeaponBehavior()
    {
        rightWeaponBehavior.weaponType = Weapon.WeaponType.Empty;
    }
    private void ResetSecondaryBehavior()
    {
        secondaryBehavior.secondaryType = Secondary.SecondaryType.Empty;
    }

    //SETUP DAS ARMAS E SECUNDARIOS
    private void InstantiateAndSetupWeapon()
    {
        lastInstantiatedWeapon = Instantiate(equippedWeapon.weaponPrefab, rightHandHolder.transform);
        rightWeaponBehavior.resourceCost = equippedWeapon.resourceCost;

        rightWeaponBehavior.SetWeaponType(equippedWeapon.weaponType);


        if (lastInstantiatedWeapon.TryGetComponent<SwordBehavior>(out var swordBehavior))
        {
            swordBehavior.damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedWeapon.TryGetComponent<HammerBehavior>(out var hammerBehavior))
        {
            hammerBehavior.damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedWeapon.TryGetComponent<StaffBehavior>(out var staffBehavior))
        {
            staffBehavior.beamPrefab = equippedWeapon.beam;
            staffBehavior.staffProjectilePrefab = equippedWeapon.projectile;
            staffBehavior.damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedWeapon.TryGetComponent<BowBehavior>(out var bowBehavior))
        {
            bowBehavior.bowProjectilePrefab = equippedWeapon.projectile;
            bowBehavior.damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedWeapon.TryGetComponent<GreatHammerBehavior>(out var greatHammerBehavior))
        {
            greatHammerBehavior.earthquakePrefab = equippedWeapon.earthquake;
            greatHammerBehavior.damage = equippedWeapon.damage; 
        }
        else if (lastInstantiatedWeapon.TryGetComponent<GreatSwordBehavior>(out var greatSwordBehavior))
        {
            greatSwordBehavior.damage = equippedWeapon.damage;
        }

    }
    private void InstantiateAndSetupSecondary()
    {
        lastInstantiatedSecondary = Instantiate(equippedSecondary.secondaryPrefab, leftHandHolder.transform);
        secondaryBehavior.resourceCost = equippedSecondary.resourceCost;

        secondaryBehavior.SetSecondaryType(equippedSecondary.secondaryType);

        if(lastInstantiatedSecondary.TryGetComponent<ShieldBehavior>(out var shieldBehavior))
        {
            shieldBehavior.blockedAttackSound = equippedSecondary.blockSound;
        }
        else if (lastInstantiatedSecondary.TryGetComponent<MagicBookBehavior>(out var magicBookBehavior))
        {
            magicBookBehavior.skillPrefab = equippedSecondary.skillPrefab;
            magicBookBehavior.damage = equippedSecondary.damage;
        }
        else if (lastInstantiatedSecondary.TryGetComponent<MagicOrbBehavior>(out var magicOrbBehavior))
        {
            magicOrbBehavior.skillPrefab = equippedSecondary.skillPrefab;
            magicOrbBehavior.damage = equippedSecondary.damage;
        }
    }

    //GERADORES DOS PREFABS DE SLOT NO JOGO
    public void GenerateInventorySlots()
    {
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventoryGrid);

            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.slotIndex = i; 
            }

            if (inventoryItems[i] != null)
            {
                GameObject inventoryItemObject = Instantiate(inventoryItemPrefab, slotObject.transform);

                InventoryItem inventoryItem = inventoryItemObject.GetComponent<InventoryItem>();
                if (inventoryItem != null)
                {
                    inventoryItem.item = inventoryItems[i];

                    Image inventoryItemImage = inventoryItem.GetComponent<Image>();

                    var iconField = inventoryItems[i].GetType().GetField("icon");
                    if (iconField != null)
                    {
                        Sprite icon = iconField.GetValue(inventoryItems[i]) as Sprite;
                        if (inventoryItemImage != null && icon != null)
                        {
                            inventoryItemImage.sprite = icon; 
                        }
                    }
                }
            }
        }
    }
    public void GenerateStashSlots()
    {
        foreach (Transform child in stashGrid)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < stashItems.Count; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, stashGrid);

            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.slotIndex = i + 33; 
                slot.slotType = InventorySlot.SlotType.Stash;
            }

            if (stashItems[i] != null)
            {
                GameObject stashItemObject = Instantiate(inventoryItemPrefab, slotObject.transform);

                InventoryItem stashItem = stashItemObject.GetComponent<InventoryItem>();
                if (stashItem != null)
                {
                    stashItem.item = stashItems[i];

                    Image stashItemImage = stashItem.GetComponent<Image>();

                    var iconField = stashItems[i].GetType().GetField("icon");
                    if (iconField != null)
                    {
                        Sprite icon = iconField.GetValue(stashItems[i]) as Sprite;
                        if (stashItemImage != null && icon != null)
                        {
                            stashItemImage.sprite = icon; 
                        }
                    }
                }
            }
        }
    }

    //AVULSOS
    public void PotionControll()
    {
        if (equippedPotion != null && _inputHandler.potionInput)
        {
            if (potionsCount <= 0)
                return;

            switch (equippedPotion.potionType)
            {
                case PotionType.Health:
                    playerStatus.currentHp += equippedPotion.valueToRecover;
                    break;

                case PotionType.Stamina:
                    playerStatus.currentStamina += equippedPotion.valueToRecover;
                    break;

                case PotionType.Mana:
                    playerStatus.currentMana += equippedPotion.valueToRecover;
                    break;

                case PotionType.Empty:
                    break;

                default:
                    break;
            }
            SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.drinkSound);
            potionsCount--;

            _inputHandler.potionInput = false;

        }
        if (potionsCount < maxPotionsCount)
        {
            potionRegenerationTimer += Time.deltaTime;

            if (potionRegenerationTimer >= potionRegenerationCooldown)
            {
                potionsCount++;
                potionRegenerationTimer = 0f;
            }
        }
    }
}