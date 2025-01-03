using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Player References")]
    public GameObject handsPOV;
    public GameObject rightHandHolder;
    public WeaponBehavior rightWeaponBehavior;

    [Header("Inventory References")]
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;

    [Header("Inventory")]
    public List<ItemBase> inventoryItems = new List<ItemBase>();

    [Header("Equipped Items")]
    public Weapon equippedWeapon;

    private GameObject lastInstantiatedObject;

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

    // Limpa a arma anterior
    private void ClearPreviousWeapon()
    {
        if (lastInstantiatedObject != null)
            Destroy(lastInstantiatedObject);
    }

    // Reseta os comportamentos da arma
    private void ResetWeaponBehavior()
    {
        rightWeaponBehavior.weaponType = Weapon.WeaponType.Empty;
    }

    // Instancia e configura a nova arma
    private void InstantiateAndSetupWeapon()
    {
        lastInstantiatedObject = Instantiate(equippedWeapon.weaponPrefab, rightHandHolder.transform);

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

        if (lastInstantiatedObject.TryGetComponent<SwordBehavior>(out var swordBehavior))
        {
            swordBehavior.damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedObject.TryGetComponent<StaffBehavior>(out var staffBehavior))
        {
            rightWeaponBehavior.projectilePrefab = equippedWeapon.projectile;
            staffBehavior.damage = equippedWeapon.damage;
            rightWeaponBehavior.projectileExitPoint = staffBehavior.projectileExit;
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