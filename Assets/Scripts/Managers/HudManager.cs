using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

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
    }

    [Header("References")]
    public GameObject inventory;
    public CinemachineVirtualCamera virtualCamera;
    public PlayerInput playerInput;
    public GameObject warnBtn;

    [Header("Controllers")]
    private bool openedInventory = false;

    private void Update()
    {
        UpdateCursorState();
        OpenInventory();
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
        }
    }
}
