using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ItemBase;
using static Potion;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private int stashIndexStartAt = 49;

    [Header("Player References")]
    public GameObject handsPOV;
    public GameObject rightHandHolder;
    public GameObject leftHandHolder;

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

    [Header("All Inventory Slots (Inventory/Equips and Stash")]
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();

    [Header("Equipped Items")]
    [SerializeField] public Weapon equippedWeapon;
    [SerializeField] public Secondary equippedSecondary;
    [SerializeField] private Jewel equippedNecklace;
    [SerializeField] private Jewel equippedRing;
    [SerializeField] private Jewel equippedAmulet;
    [SerializeField] public Potion equippedPotion;

    [Header("Equippament Slots")]
    public GameObject weaponSlot;
    public GameObject secondarySlot;
    public GameObject neckSlot;
    public GameObject ringSlot;
    public GameObject amuletSlot;
    public GameObject potionSlot;

    [Header("Potion Controller")]
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

        LoadInventory();
    }

    private void Start()
    {
        GenerateInventorySlots();
        GenerateStashSlots();
        inventorySlots = new List<InventorySlot>(FindObjectsOfType<InventorySlot>());

        InstantiateEquippedItem(equippedWeapon, weaponSlot);
        InstantiateEquippedItem(equippedSecondary, secondarySlot);

        InstantiateEquippedItem(equippedNecklace, neckSlot);
        InstantiateEquippedItem(equippedRing, ringSlot);
        InstantiateEquippedItem(equippedAmulet, amuletSlot);

        InstantiateEquippedItem(equippedPotion, potionSlot);
    }

    private void InstantiateEquippedItem(ItemBase item, GameObject slot)
    {
        if (item != null && slot != null)
        {
            var instantiatedItem = Instantiate(inventoryItemPrefab, slot.transform).GetComponent<InventoryItem>();

            if (instantiatedItem != null)
            {
                instantiatedItem.item = item;

                var icon = item.GetType().GetField("icon")?.GetValue(item) as Sprite;
                var inventoryItemImage = instantiatedItem.transform.Find("Icon")?.GetComponent<Image>();

                if (inventoryItemImage != null && icon != null)
                    inventoryItemImage.sprite = icon;

                var backgrounds = instantiatedItem.transform.Find("Backgrounds");
                if (backgrounds != null)
                    SetRarityColor(instantiatedItem.item.itemRarity, backgrounds);
            }
        }
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
    public void EquipJewelFromInventory(Jewel jewel, string jewelType)
    {
        if (jewel == null)
        {
            switch (jewelType)
            {
                case "Ring":
                    RemoveJewelFunction(lastRingEquipped);
                    equippedRing = null;
                    lastRingEquipped = null;
                    break;

                case "Necklace":
                    RemoveJewelFunction(lastNecklaceEquipped);
                    equippedNecklace = null;
                    lastNecklaceEquipped = null;
                    break;

                case "Amulet":
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
            switch (jewelType)
            {
                case "Ring":
                    if (lastRingEquipped != null) { RemoveJewelFunction(lastRingEquipped); }
                    equippedRing = jewel;
                    lastRingEquipped = jewel;
                    break;

                case "Necklace":
                    if (lastNecklaceEquipped != null) { RemoveJewelFunction(lastNecklaceEquipped); }
                    equippedNecklace = jewel;
                    lastNecklaceEquipped = jewel;
                    break;

                case "Amulet":
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
            //rightWeaponBehavior.SetWeaponType(null);
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
                    PlayerComponents.Instance.playerStatus.ControlBaseDamage(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    PlayerComponents.Instance.playerStatus.ControlDamagePercent(value * jewel.valueToAdd);
                break;

            case Jewel.StatType.HP:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    PlayerComponents.Instance.playerStatus.ControlBaseHealth(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreaseRegeneration)
                    PlayerComponents.Instance.playerStatus.ControlBaseHealthRegeneration(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    PlayerComponents.Instance.playerStatus.ControlHealthPercent(value * jewel.valueToAdd);
                break;

            case Jewel.StatType.Stamina:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    PlayerComponents.Instance.playerStatus.ControlBaseStamina(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreaseRegeneration)
                    PlayerComponents.Instance.playerStatus.ControlBaseStaminaRegeneration(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    PlayerComponents.Instance.playerStatus.ControlStaminaPercent(value * jewel.valueToAdd);
                break;

            case Jewel.StatType.Mana:
                if (effectType == Jewel.EffectType.IncreaseBase)
                    PlayerComponents.Instance.playerStatus.ControlBaseMana(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreaseRegeneration)
                    PlayerComponents.Instance.playerStatus.ControlBaseManaRegeneration(value * jewel.valueToAdd);
                else if (effectType == Jewel.EffectType.IncreasePercent)
                    PlayerComponents.Instance.playerStatus.ControlManaPercent(value * jewel.valueToAdd);
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
        PlayerComponents.Instance.weaponBehavior.SetWeaponType(null);
        HudManager.Instance.skillImage.enabled = false;
        HudManager.Instance.skillSliderImage.enabled = false;
    }
    private void ResetSecondaryBehavior()
    {
        PlayerComponents.Instance.secondaryBehavior.SetSecondaryType(null);
        HudManager.Instance.magicImage.enabled = false;
        HudManager.Instance.magicSliderImage.enabled = false;
    }

    //SETUP DAS ARMAS E SECUNDARIOS
    private void InstantiateAndSetupWeapon()
    {
        lastInstantiatedWeapon = Instantiate(equippedWeapon.weaponPrefab, rightHandHolder.transform);
        PlayerComponents.Instance.weaponBehavior.resourceCost = equippedWeapon.resourceCost;

        PlayerComponents.Instance.weaponBehavior.SetWeaponType(equippedWeapon);
        PlayerComponents.Instance.weaponBehavior.skillType = null;

        HudManager.Instance.skillImage.enabled = false;
        HudManager.Instance.skillSliderImage.enabled = false;

        if (lastInstantiatedWeapon.TryGetComponent<SwordBehavior>(out var swordBehavior))
        {
            if (equippedWeapon.weaponSkill != null)
            {
                swordBehavior.weaponSkillPrefab = equippedWeapon.weaponSkill.skillPrefab;
                swordBehavior.skillDamageMultiplier = equippedWeapon.weaponSkill.damageMultiplier;
            }

            swordBehavior.damage = equippedWeapon.damage;

            if (equippedWeapon is Sword sword)
            {
                if (equippedWeapon.weaponSkill == null) return;

                HudManager.Instance.skillImage.enabled = true;
                HudManager.Instance.skillSliderImage.enabled = true;

                HudManager.Instance.skillImage.sprite = sword.weaponSkill.skillIcon;
                HudManager.Instance.skillSliderImage.sprite = sword.weaponSkill.skillIcon;

                PlayerComponents.Instance.weaponBehavior.skillType = sword.weaponSkill.skillType.ToString();
            }
        }
        else if (lastInstantiatedWeapon.TryGetComponent<HammerBehavior>(out var hammerBehavior))
        {
            hammerBehavior.damage = equippedWeapon.damage;
            if (equippedWeapon is Hammer hammer)
            {
                // Lógica do Hammer, se necessário
            }
        }
        else if (lastInstantiatedWeapon.TryGetComponent<StaffBehavior>(out var staffBehavior))
        {
            if (equippedWeapon.weaponSkill != null)
            {
                staffBehavior.beamPrefab = equippedWeapon.weaponSkill.skillPrefab;
                staffBehavior.skillDamageMultiplier = equippedWeapon.weaponSkill.damageMultiplier;
            }

            staffBehavior.damage = equippedWeapon.damage;

            if (equippedWeapon is Staff staff)
            {
                staffBehavior.staffProjectilePrefab = staff.projectile;

                if (staff.weaponSkill != null)
                {
                    if (equippedWeapon.weaponSkill == null) return;

                    HudManager.Instance.skillImage.enabled = true;
                    HudManager.Instance.skillSliderImage.enabled = true;

                    HudManager.Instance.skillImage.sprite = staff.weaponSkill.skillIcon;
                    HudManager.Instance.skillSliderImage.sprite = staff.weaponSkill.skillIcon;

                    PlayerComponents.Instance.weaponBehavior.skillType = staff.weaponSkill.skillType.ToString();
                }
            }
        }
        else if (lastInstantiatedWeapon.TryGetComponent<BowBehavior>(out var bowBehavior))
        {
            bowBehavior.damage = equippedWeapon.damage;

            if (equippedWeapon is Bow bow)
            {
                bowBehavior.bowProjectilePrefab = bow.projectile;

                if (bow.weaponSkill != null)
                {
                    if (equippedWeapon.weaponSkill == null) return;

                    HudManager.Instance.skillImage.enabled = true;
                    HudManager.Instance.skillSliderImage.enabled = true;

                    HudManager.Instance.skillImage.sprite = bow.weaponSkill.skillIcon;
                    HudManager.Instance.skillSliderImage.sprite = bow.weaponSkill.skillIcon;

                    PlayerComponents.Instance.weaponBehavior.skillType = bow.weaponSkill.skillType.ToString();
                }
            }
        }
        else if (lastInstantiatedWeapon.TryGetComponent<GreatHammerBehavior>(out var greatHammerBehavior))
        {
            if (equippedWeapon.weaponSkill != null)
            {
                greatHammerBehavior.weaponSkillPrefab = equippedWeapon.weaponSkill.skillPrefab;
                greatHammerBehavior.skillDamageMultiplier = equippedWeapon.weaponSkill.damageMultiplier;
            }

            greatHammerBehavior.damage = equippedWeapon.damage;
            
            if (equippedWeapon is GreatHammer greatHammer)
            {
                if (greatHammer.weaponSkill != null)
                {
                    if (equippedWeapon.weaponSkill == null) return;

                    HudManager.Instance.skillImage.enabled = true;
                    HudManager.Instance.skillSliderImage.enabled = true;

                    HudManager.Instance.skillImage.sprite = greatHammer.weaponSkill.skillIcon;
                    HudManager.Instance.skillSliderImage.sprite = greatHammer.weaponSkill.skillIcon;

                    PlayerComponents.Instance.weaponBehavior.skillType = greatHammer.weaponSkill.skillType.ToString();
                }
            }
        }
        else if (lastInstantiatedWeapon.TryGetComponent<GreatSwordBehavior>(out var greatSwordBehavior))
        {
            if(equippedWeapon.weaponSkill != null)
            {
                greatSwordBehavior.weaponSkillPrefab = equippedWeapon.weaponSkill.skillPrefab;
                greatSwordBehavior.skillDamageMultiplier = equippedWeapon.weaponSkill.damageMultiplier;
            }

            greatSwordBehavior.damage = equippedWeapon.damage;
           
            if (equippedWeapon is GreatSword greatSword)
            {
                if (greatSword.weaponSkill != null)
                {
                    if (equippedWeapon.weaponSkill == null) return;

                    HudManager.Instance.skillImage.enabled = true;
                    HudManager.Instance.skillSliderImage.enabled = true;

                    HudManager.Instance.skillImage.sprite = greatSword.weaponSkill.skillIcon;
                    HudManager.Instance.skillSliderImage.sprite = greatSword.weaponSkill.skillIcon;

                    PlayerComponents.Instance.weaponBehavior.skillType = greatSword.weaponSkill.skillType.ToString();
                }
            }
        }
        else if (lastInstantiatedWeapon.TryGetComponent<GreatAxeBehavior>(out var greatAxeBehavior))
        {
            if (equippedWeapon.weaponSkill != null)
            {
                greatAxeBehavior.weaponSkillPrefab = equippedWeapon.weaponSkill.skillPrefab;
                greatAxeBehavior.skillDamageMultiplier = equippedWeapon.weaponSkill.damageMultiplier;
            }

            greatAxeBehavior.damage = equippedWeapon.damage;

            if (equippedWeapon is GreatAxe greatAxe)
            {
                if (greatAxe.weaponSkill != null)
                {
                    if (equippedWeapon.weaponSkill == null) return;

                    HudManager.Instance.skillImage.enabled = true;
                    HudManager.Instance.skillSliderImage.enabled = true;

                    HudManager.Instance.skillImage.sprite = greatAxe.weaponSkill.skillIcon;
                    HudManager.Instance.skillSliderImage.sprite = greatAxe.weaponSkill.skillIcon;

                    PlayerComponents.Instance.weaponBehavior.skillType = greatAxe.weaponSkill.skillType.ToString();
                }
            }
        }
    }



    private void InstantiateAndSetupSecondary()
    {
        lastInstantiatedSecondary = Instantiate(equippedSecondary.secondaryPrefab, leftHandHolder.transform);

        PlayerComponents.Instance.secondaryBehavior.SetSecondaryType(equippedSecondary);
        PlayerComponents.Instance.secondaryBehavior.spellType = null; //Apenas reseta o spellType se nao tiver nenhum quando instancia um Secondary

        HudManager.Instance.magicImage.enabled = false;
        HudManager.Instance.magicSliderImage.enabled = false;

        if (lastInstantiatedSecondary.TryGetComponent<ShieldBehavior>(out var shieldBehavior))
        {
            if (equippedSecondary is Shield shield)
            {
                PlayerComponents.Instance.secondaryBehavior.resourceCost = shield.blockResourceCost;
                shieldBehavior.blockPercent = shield.blockPercentValue;
                shieldBehavior.blockedAttackSound = shield.blockSound;
            }
        }
        else if (lastInstantiatedSecondary.TryGetComponent<MagicBookBehavior>(out var magicBookBehavior))
        {
            if (equippedSecondary is Book magicBook)
            {
                magicBookBehavior.skillPrefab = magicBook.spell.spellPrefab;
                magicBookBehavior.damage = magicBook.spell.spellDamage;
                PlayerComponents.Instance.secondaryBehavior.resourceCost = magicBook.spell.spellResourceCost;
                PlayerComponents.Instance.secondaryBehavior.spellType = magicBook.spell.spellType.ToString();

                if (magicBook.spell != null)
                {
                    HudManager.Instance.magicImage.enabled = true;
                    HudManager.Instance.magicSliderImage.enabled = true;

                    HudManager.Instance.magicImage.sprite = magicBook.spell.spellIcon;
                    HudManager.Instance.magicSliderImage.sprite = magicBook.spell.spellIcon;
                }
            }
        }
        else if (lastInstantiatedSecondary.TryGetComponent<MagicOrbBehavior>(out var magicOrbBehavior))
        {
            if (equippedSecondary is Orb magicOrb)
            {
                magicOrbBehavior.skillPrefab = magicOrb.spell.spellPrefab;
                magicOrbBehavior.damage = magicOrb.spell.spellDamage;
                PlayerComponents.Instance.secondaryBehavior.resourceCost = magicOrb.spell.spellResourceCost;
                PlayerComponents.Instance.secondaryBehavior.spellType = magicOrb.spell.spellType.ToString();

                if (magicOrb.spell != null)
                {
                    HudManager.Instance.magicImage.enabled = true;
                    HudManager.Instance.magicSliderImage.enabled = true;

                    HudManager.Instance.magicImage.sprite = magicOrb.spell.spellIcon;
                    HudManager.Instance.magicSliderImage.sprite = magicOrb.spell.spellIcon;
                }
            }
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

            //if (inventoryItems[i] != null) //Função Chamada caso seja o InventoryItem antigo
            //{
            //    GameObject inventoryItemObject = Instantiate(inventoryItemPrefab, slotObject.transform);

            //    InventoryItem inventoryItem = inventoryItemObject.GetComponent<InventoryItem>();
            //    if (inventoryItem != null)
            //    {
            //        inventoryItem.item = inventoryItems[i];

            //        Image inventoryItemImage = inventoryItem.GetComponentInChildren<Image>();

            //        var iconField = inventoryItems[i].GetType().GetField("icon");
            //        if (iconField != null)
            //        {
            //            Sprite icon = iconField.GetValue(inventoryItems[i]) as Sprite;
            //            if (inventoryItemImage != null && icon != null)
            //            {
            //                inventoryItemImage.sprite = icon; 
            //            }
            //        }
            //    }
            //}

            if (inventoryItems[i] != null)
            {
                var inventoryItem = Instantiate(inventoryItemPrefab, slotObject.transform).GetComponent<InventoryItem>();
                if (inventoryItem != null)
                {
                    inventoryItem.item = inventoryItems[i];
                    var icon = inventoryItems[i].GetType().GetField("icon")?.GetValue(inventoryItems[i]) as Sprite;
                    var inventoryItemImage = inventoryItem.transform.Find("Icon")?.GetComponent<Image>();

                    if (inventoryItemImage != null && icon != null)
                        inventoryItemImage.sprite = icon;

                    var backgrounds = inventoryItem.transform.Find("Backgrounds");
                    if (backgrounds != null)
                        SetRarityColor(inventoryItem.item.itemRarity, backgrounds);
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
                slot.slotIndex = i + stashIndexStartAt; 
                slot.slotType = InventorySlot.SlotType.Stash;
            }

            //if (stashItems[i] != null) //Função antiga
            //{
            //    GameObject stashItemObject = Instantiate(inventoryItemPrefab, slotObject.transform);

            //    InventoryItem stashItem = stashItemObject.GetComponent<InventoryItem>();
            //    if (stashItem != null)
            //    {
            //        stashItem.item = stashItems[i];

            //        Image stashItemImage = stashItem.GetComponent<Image>();

            //        var iconField = stashItems[i].GetType().GetField("icon");
            //        if (iconField != null)
            //        {
            //            Sprite icon = iconField.GetValue(stashItems[i]) as Sprite;
            //            if (stashItemImage != null && icon != null)
            //            {
            //                stashItemImage.sprite = icon; 
            //            }
            //        }
            //    }
            //}

            if (stashItems[i] != null)
            {
                var stashItem = Instantiate(inventoryItemPrefab, slotObject.transform).GetComponent<InventoryItem>();
                if (stashItem != null)
                {
                    stashItem.item = stashItems[i];
                    var icon = stashItems[i].GetType().GetField("icon")?.GetValue(stashItems[i]) as Sprite;
                    var stashItemImage = stashItem.transform.Find("Icon")?.GetComponent<Image>();

                    if (stashItemImage != null && icon != null)
                        stashItemImage.sprite = icon;

                    var backgrounds = stashItem.transform.Find("Backgrounds");
                    if (backgrounds != null)
                        SetRarityColor(stashItem.item.itemRarity, backgrounds);
                }
            }
        }
    }

    private void SetRarityColor(ItemRarity rarity, Transform backgrounds)
    {
        Color rarityColor = Color.white;
        switch (rarity)
        {
            case ItemRarity.Uncommon: rarityColor = Color.green; break;
            case ItemRarity.Rare: rarityColor = Color.blue; break;
            case ItemRarity.Epic: rarityColor = Color.magenta; break;
            case ItemRarity.Legendary: rarityColor = Color.yellow; break;
        }

        var backgroundImages = backgrounds.GetComponentsInChildren<Image>();
        foreach (var bgImage in backgroundImages)
        {
            Color originalColor = bgImage.color;
            bgImage.color = new Color(rarityColor.r, rarityColor.g, rarityColor.b, originalColor.a); // Mantém a opacidade original
        }
    }


    //AVULSOS
    public void PotionControll()
    {
        if (equippedPotion != null && PlayerComponents.Instance.inputHandler.potionInput)
        {
            if (potionsCount <= 0)
                return;

            switch (equippedPotion.potionType)
            {
                case PotionType.Health:
                    PlayerComponents.Instance.playerStatus.currentHp += equippedPotion.valueToRecover;
                    break;

                case PotionType.Stamina:
                    PlayerComponents.Instance.playerStatus.currentStamina += equippedPotion.valueToRecover;
                    break;

                case PotionType.Mana:
                    PlayerComponents.Instance.playerStatus.currentMana += equippedPotion.valueToRecover;
                    break;

                case PotionType.Empty:
                    break;

                default:
                    break;
            }
            SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.drinkSound);
            potionsCount--;

            PlayerComponents.Instance.inputHandler.potionInput = false;

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

    //Save Controll
    public void SaveInventory()
    {
        ES3.Save("inventoryItems", inventoryItems);
        ES3.Save("stashItems", stashItems);
        ES3.Save("equippedWeapon", equippedWeapon);
        ES3.Save("equippedSecondary", equippedSecondary);
        ES3.Save("equippedNecklace", equippedNecklace);
        ES3.Save("equippedRing", equippedRing);
        ES3.Save("equippedAmulet", equippedAmulet);
        ES3.Save("equippedPotion", equippedPotion);
    }

    public void LoadInventory()
    {
        if (ES3.KeyExists("inventoryItems"))
            inventoryItems = ES3.Load<List<ItemBase>>("inventoryItems");


        if (ES3.KeyExists("stashItems"))
            stashItems = ES3.Load<List<ItemBase>>("stashItems");


        if (ES3.KeyExists("equippedWeapon"))
            equippedWeapon = ES3.Load<Weapon>("equippedWeapon");


        if (ES3.KeyExists("equippedSecondary"))
            equippedSecondary = ES3.Load<Secondary>("equippedSecondary");


        if (ES3.KeyExists("equippedNecklace"))
            equippedNecklace = ES3.Load<Jewel>("equippedNecklace");


        if (ES3.KeyExists("equippedRing"))
            equippedRing = ES3.Load<Jewel>("equippedRing");


        if (ES3.KeyExists("equippedAmulet"))
            equippedAmulet = ES3.Load<Jewel>("equippedAmulet");


        if (ES3.KeyExists("equippedPotion"))
            equippedPotion = ES3.Load<Potion>("equippedPotion");

    }
}