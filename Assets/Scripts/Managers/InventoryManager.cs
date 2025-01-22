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

    // Inicializa a instância e configura as referências
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

    // Gera os slots do inventário ao iniciar o jogo
    private void Start()
    {
        playerStatus = PlayerManager.Instance.GetComponent<PlayerStatus>();
        GenerateInventorySlots();
        GenerateStashSlots();
    }

    private void Update()
    {
        if (equippedPotion != null && Input.GetKeyDown(KeyCode.F))
        {
            if (potionsCount <= 0)
                return;

            switch (equippedPotion.potionType)
            {
                case Potion.PotionType.Health:
                    playerStatus.currentHp += equippedPotion.valueToRecover;
                    break;

                case Potion.PotionType.Stamina:
                    playerStatus.currentStamina += equippedPotion.valueToRecover;
                    break;

                case Potion.PotionType.Mana:
                    playerStatus.currentMana += equippedPotion.valueToRecover;
                    break;

                case Potion.PotionType.Empty:
                    break;

                default:
                    break;
            }
            SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.drinkSound);
            potionsCount--;
            
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

    // Adiciona um item ao inventário e atualiza a UI
    public void AddItem(ItemBase item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] == null) // Encontrar um índice vazio
            {
                inventoryItems[i] = item;
                GenerateInventorySlots(); // Atualiza a UI
                GenerateStashSlots();
                return;
            }
        }

        // Se não houver índices vazios, adicionamos o item ao final da lista
        inventoryItems.Add(item);
        GenerateInventorySlots(); // Atualiza a UI
        GenerateStashSlots();
    }

    // Remove um item do inventário e atualiza a UI
    public void RemoveItem(ItemBase item)
    {
        int index = inventoryItems.IndexOf(item);
        if (index >= 0)
        {
            inventoryItems[index] = null; // Limpa o item no índice, sem remover o índice
            GenerateInventorySlots(); // Atualiza a UI
        }
    }

    // Equipar os itens a partir do inventário
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
            // Se a joia for null, significa que queremos remover a joia do tipo específico
            switch (jewelType)
            {
                case Jewel.JewelType.Ring:
                    // Remove a joia e os efeitos
                    RemoveJewelFunction(lastRingEquipped);
                    equippedRing = null; // Remove o Ring
                    lastRingEquipped = null; // Limpa a última joia equipada
                    break;

                case Jewel.JewelType.Necklace:
                    // Remove a joia e os efeitos
                    RemoveJewelFunction(lastNecklaceEquipped);
                    equippedNecklace = null; // Remove o Necklace
                    lastNecklaceEquipped = null; // Limpa a última joia equipada
                    break;

                case Jewel.JewelType.Amulet:
                    // Remove a joia e os efeitos
                    RemoveJewelFunction(lastAmuletEquipped);
                    equippedAmulet = null; // Remove o Amulet
                    lastAmuletEquipped = null; // Limpa a última joia equipada
                    break;

                default:
                    break;
            }
        }
        else
        {
            // Caso contrário, equipamos a joia normalmente
            switch (jewel.jewelType)
            {
                case Jewel.JewelType.Ring:
                    // Se havia uma joia anterior, removemos os efeitos dela
                    if (lastRingEquipped != null) { RemoveJewelFunction(lastRingEquipped); }
                    equippedRing = jewel;
                    lastRingEquipped = jewel;  // Guarda a última joia equipada
                    break;

                case Jewel.JewelType.Necklace:
                    // Se havia uma joia anterior, removemos os efeitos dela
                    if (lastNecklaceEquipped != null) { RemoveJewelFunction(lastNecklaceEquipped); }
                    equippedNecklace = jewel;
                    lastNecklaceEquipped = jewel;  // Guarda a última joia equipada
                    break;

                case Jewel.JewelType.Amulet:
                    // Se havia uma joia anterior, removemos os efeitos dela
                    if (lastAmuletEquipped != null) { RemoveJewelFunction(lastAmuletEquipped); }
                    equippedAmulet = jewel;
                    lastAmuletEquipped = jewel;  // Guarda a última joia equipada
                    break;

                default:
                    break;
            }

            // Agora aplica os efeitos da nova joia equipada
            AddJewelFunction(jewel);
        }
    }

    public void EquipPotionFromInventory(Potion potion)
    {
        equippedPotion = potion;
    }


    // Equipar a arma atual, instanciando no personagem
    public void EquipWeapon()
    {
        if (lastInstantiatedWeapon != null)
            Destroy(lastInstantiatedWeapon);

        if (equippedWeapon == null || equippedWeapon.weaponPrefab == null)
        {
            ResetWeaponBehavior();
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
        ApplyJewelEffect(jewel, 1); // Passa o valor positivo para adicionar o efeito
    }

    public void RemoveJewelFunction(Jewel jewel)
    {
        ApplyJewelEffect(jewel, -1); // Passa o valor negativo para remover o efeito
    }


    // Reseta os comportamentos da arma
    private void ResetWeaponBehavior()
    {
        rightWeaponBehavior.weaponType = Weapon.WeaponType.Empty;
    }

    private void ResetSecondaryBehavior()
    {
        secondaryBehavior.secondaryType = Secondary.SecondaryType.Empty;
    }

    // Instancia e configura a nova arma
    private void InstantiateAndSetupWeapon()
    {
        lastInstantiatedWeapon = Instantiate(equippedWeapon.weaponPrefab, rightHandHolder.transform);
        rightWeaponBehavior.resourceCost = equippedWeapon.resourceCost;

        switch (equippedWeapon.weaponType)
        {
            case Weapon.WeaponType.Sword:
                rightWeaponBehavior.weaponType = Weapon.WeaponType.Sword; // Definindo o tipo de arma
                break;

            case Weapon.WeaponType.Staff:
                rightWeaponBehavior.weaponType = Weapon.WeaponType.Staff; // Definindo o tipo de arma
                break;

            case Weapon.WeaponType.Hammer:
                rightWeaponBehavior.weaponType = Weapon.WeaponType.Hammer; // Definindo o tipo de arma
                break;

            case Weapon.WeaponType.Bow:
                rightWeaponBehavior.weaponType = Weapon.WeaponType.Bow; // Definindo o tipo de arma
                break;
        }

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
            rightWeaponBehavior.projectilePrefab = equippedWeapon.projectile;
            rightWeaponBehavior.beamPrefab = equippedWeapon.beam;
            staffBehavior.damage = equippedWeapon.damage;
            rightWeaponBehavior.projectileExitPoint = staffBehavior.projectileExit;
        }
        else if (lastInstantiatedWeapon.TryGetComponent<BowBehavior>(out var bowBehavior))
        {
            rightWeaponBehavior.projectilePrefab = equippedWeapon.projectile;
            bowBehavior.damage = equippedWeapon.damage;
            rightWeaponBehavior.projectileExitPoint = bowBehavior.projectileExit;
        }

    }

    private void InstantiateAndSetupSecondary()
    {
        lastInstantiatedSecondary = Instantiate(equippedSecondary.secondaryPrefab, leftHandHolder.transform);

        switch (equippedSecondary.secondaryType)
        {
            case Secondary.SecondaryType.Shield:
                secondaryBehavior.secondaryType = Secondary.SecondaryType.Shield;
                secondaryBehavior.blockSound = equippedSecondary.blockSound;
                secondaryBehavior.resourceCost = equippedSecondary.resourceCost;
                break;

            case Secondary.SecondaryType.Orb:
                secondaryBehavior.secondaryType = Secondary.SecondaryType.Orb;
                break;

            case Secondary.SecondaryType.Book:
                secondaryBehavior.secondaryType = Secondary.SecondaryType.Book;
                break;

        }
    }

    // Gera os slots de inventário com base nos itens do inventário
    public void GenerateInventorySlots()
    {
        // Limpa os slots existentes (caso tenha algum)
        foreach (Transform child in inventoryGrid)
        {
            Destroy(child.gameObject);
        }

        // Gera os slots de inventário com base nos itens no inventário
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            // Instancia o prefab de InventorySlot
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventoryGrid);

            // Define o índice do slot
            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.slotIndex = i; // Atribui o índice ao slot
            }

            // Verifica se o item não é nulo antes de criar o InventoryItem
            if (inventoryItems[i] != null)
            {
                // Instancia o prefab de InventoryItem dentro do InventorySlot
                GameObject inventoryItemObject = Instantiate(inventoryItemPrefab, slotObject.transform);

                // Associa o item ScriptableObject ao InventoryItem
                InventoryItem inventoryItem = inventoryItemObject.GetComponent<InventoryItem>();
                if (inventoryItem != null)
                {
                    inventoryItem.item = inventoryItems[i];

                    // Obtém o componente Image do InventoryItem
                    Image inventoryItemImage = inventoryItem.GetComponent<Image>();

                    // Usando reflexão para procurar o campo "icon" no ScriptableObject
                    var iconField = inventoryItems[i].GetType().GetField("icon");
                    if (iconField != null)
                    {
                        Sprite icon = iconField.GetValue(inventoryItems[i]) as Sprite;
                        if (inventoryItemImage != null && icon != null)
                        {
                            inventoryItemImage.sprite = icon; // Atribui o sprite
                        }
                    }
                }
            }
        }
    }

    public void GenerateStashSlots()
    {
        // Limpa os slots existentes (caso tenha algum)
        foreach (Transform child in stashGrid)
        {
            Destroy(child.gameObject);
        }

        // Gera os slots de inventário com base nos itens no inventário
        for (int i = 0; i < stashItems.Count; i++)
        {
            // Instancia o prefab de InventorySlot
            GameObject slotObject = Instantiate(inventorySlotPrefab, stashGrid);

            // Define o índice do slot
            InventorySlot slot = slotObject.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.slotIndex = i + 33; // Atribui o índice ao slot
                slot.slotType = InventorySlot.SlotType.Stash;
            }

            // Verifica se o item não é nulo antes de criar o InventoryItem
            if (stashItems[i] != null)
            {
                // Instancia o prefab de InventoryItem dentro do InventorySlot
                GameObject stashItemObject = Instantiate(inventoryItemPrefab, slotObject.transform);

                // Associa o item ScriptableObject ao InventoryItem
                InventoryItem stashItem = stashItemObject.GetComponent<InventoryItem>();
                if (stashItem != null)
                {
                    stashItem.item = stashItems[i];

                    // Obtém o componente Image do InventoryItem
                    Image stashItemImage = stashItem.GetComponent<Image>();

                    // Usando reflexão para procurar o campo "icon" no ScriptableObject
                    var iconField = stashItems[i].GetType().GetField("icon");
                    if (iconField != null)
                    {
                        Sprite icon = iconField.GetValue(stashItems[i]) as Sprite;
                        if (stashItemImage != null && icon != null)
                        {
                            stashItemImage.sprite = icon; // Atribui o sprite
                        }
                    }
                }
            }
        }
    }
}