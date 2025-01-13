using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Player References")]
    public GameObject handsPOV;
    public GameObject rightHandHolder;
    public GameObject leftHandHolder;
    public WeaponBehavior rightWeaponBehavior;
    public SecondaryBehavior secondaryBehavior;

    [Header("Inventory References")]
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;

    [Header("Inventory")]
    public List<ItemBase> inventoryItems = new List<ItemBase>();

    [Header("Equipped Items")]
    public Weapon equippedWeapon;
    public Secondary equippedSecondary;

    private GameObject lastInstantiatedWeapon;
    private GameObject lastInstantiatedSecondary;

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
        GenerateInventorySlots();
    }

    // Adiciona um item ao inventário e atualiza a UI
    public void AddItem(ItemBase item)
    {
        if (!inventoryItems.Contains(item))
        {
            // Se o item não estiver no inventário, buscamos o primeiro índice vazio e colocamos o item lá
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i] == null) // Encontrar um índice vazio
                {
                    inventoryItems[i] = item;
                    GenerateInventorySlots(); // Atualiza a UI
                    return;
                }
            }

            // Se não houver índices vazios, adicionamos o item ao final da lista
            inventoryItems.Add(item);
            GenerateInventorySlots(); // Atualiza a UI
        }
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

    // Equipar a arma a partir do inventário
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

    // Equipar a arma atual, instanciando no personagem
    public void EquipWeapon()
    {
        ClearPreviousWeapon();

        if (equippedWeapon == null || equippedWeapon.weaponPrefab == null)
        {
            ResetWeaponBehavior();
            return;
        }

        InstantiateAndSetupWeapon();
    }

    public void EquipSecondary()
    {
        ClearPreviousSecondary();

        if (equippedSecondary == null || equippedSecondary.secondaryPrefab == null)
        {
            ResetSecondaryBehavior();
            return;
        }

        InstantiateAndSetupSecondary();
    }

    // Limpa a arma anterior
    private void ClearPreviousWeapon()
    {
        if (lastInstantiatedWeapon != null)
            Destroy(lastInstantiatedWeapon);
    }
    private void ClearPreviousSecondary()
    {
        if (lastInstantiatedSecondary != null)
            Destroy(lastInstantiatedSecondary);
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

            case Weapon.WeaponType.Bow:
                rightWeaponBehavior.weaponType = Weapon.WeaponType.Bow; // Definindo o tipo de arma
                break;
        }

        if (lastInstantiatedWeapon.TryGetComponent<SwordBehavior>(out var swordBehavior))
        {
            swordBehavior.damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedWeapon.TryGetComponent<StaffBehavior>(out var staffBehavior))
        {
            rightWeaponBehavior.projectilePrefab = equippedWeapon.projectile;
            rightWeaponBehavior.beamPrefab = equippedWeapon.beam;
            staffBehavior.damage = equippedWeapon.damage;
            rightWeaponBehavior.projectileExitPoint = staffBehavior.projectileExit;
        }
    }

    private void InstantiateAndSetupSecondary()
    {
        lastInstantiatedSecondary = Instantiate(equippedSecondary.secondaryPrefab, leftHandHolder.transform);

        switch (equippedSecondary.secondaryType)
        {
            case Secondary.SecondaryType.Shield:
                secondaryBehavior.secondaryType = Secondary.SecondaryType.Shield;
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
}