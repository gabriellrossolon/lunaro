using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public GameObject handsPOV;
    public GameObject rightHandHolder;
    public WeaponBehavior rightWeaponBehavior;

    private GameObject lastInstantiatedObject;

    public List<ScriptableObject> inventoryItems = new List<ScriptableObject>();

    [Header("Equipped Weapon")]
    public Weapon equippedWeapon;

    private void Start()
    {
        EquipWeapon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) EquipWeapon();
    }

    public void EquipWeapon()
    {
        if (equippedWeapon == null || equippedWeapon.weaponPrefab == null) return;

        if (lastInstantiatedObject != null) Destroy(lastInstantiatedObject);

        lastInstantiatedObject = Instantiate(equippedWeapon.weaponPrefab, rightHandHolder.transform);

        rightWeaponBehavior.sword = equippedWeapon.sword;
        rightWeaponBehavior.staff = equippedWeapon.staff;

        if (lastInstantiatedObject.GetComponent<SwordBehavior>() != null)
        {
            lastInstantiatedObject.GetComponent<SwordBehavior>().damage = equippedWeapon.damage;
        }
        else if (lastInstantiatedObject.GetComponent<StaffBehavior>() != null)
        {
            rightWeaponBehavior.projectilePrefab = equippedWeapon.projectile;
            lastInstantiatedObject.GetComponent<StaffBehavior>().damage = equippedWeapon.damage;

            rightWeaponBehavior.projectileExitPoint = lastInstantiatedObject.GetComponent<StaffBehavior>().projectileExit;
        }
    }

    public void AddItem(ScriptableObject item)
    {
        if (!inventoryItems.Contains(item))
        {
            inventoryItems.Add(item);
        }
    }

    public void RemoveItem(ScriptableObject item)
    {
        if (inventoryItems.Contains(item))
        {
            inventoryItems.Remove(item);
        }
    }

    public void EquipWeaponFromInventory(Weapon weapon)
    {
        equippedWeapon = weapon;
        EquipWeapon();
    }
}
