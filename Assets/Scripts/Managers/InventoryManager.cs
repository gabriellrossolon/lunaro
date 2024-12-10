using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject handsPOV;
    public GameObject rightHandHolder;
    public WeaponBehavior rightWeaponBehavior;
    public List<GameObject> items;

    private GameObject lastInstantiatedObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            handsPOV.SetActive(false);
            handsPOV.SetActive(true); EquipItem(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            handsPOV.SetActive(false);
            handsPOV.SetActive(true); EquipItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            handsPOV.SetActive(false);
            handsPOV.SetActive(true);
            EquipItem(2);
        }
    }

    private void EquipItem(int index)
    {
        if (lastInstantiatedObject != null)
            Destroy(lastInstantiatedObject);

        if (index >= 0 && index < items.Count)
        {
            lastInstantiatedObject = Instantiate(items[index], rightHandHolder.transform);

            if (lastInstantiatedObject.GetComponent<SwordBehavior>() != null)
            {
                rightWeaponBehavior.sword = true;
                rightWeaponBehavior.staff = false;
            }
            else if (lastInstantiatedObject.GetComponent<StaffBehavior>() != null)
            {
                rightWeaponBehavior.staff = true;
                rightWeaponBehavior.sword = false;

                rightWeaponBehavior.projectilePrefab = lastInstantiatedObject.GetComponent<StaffBehavior>().projectile;
                rightWeaponBehavior.projectileExitPoint = lastInstantiatedObject.GetComponent<StaffBehavior>().projectileExit;
            }
            else
            {
                rightWeaponBehavior.sword = false;
                rightWeaponBehavior.staff = false;
            }
        }
    }
}
