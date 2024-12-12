using UnityEngine;

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
    public GameObject playerEquips;
    public GameObject playerInventory;
    public GameObject inventory;

    [Header("Controllers")]
    private bool openedPlayerEquips = false;
    private bool openedPlayerInventory = false;

    private void Update()
    {
        UpdateCursorState();
        OpenEquips();
        OpenInventory();
    }

    private void UpdateCursorState()
    {
        // Verifica se algum elemento do HUD está ativo
        bool hudIsOpen = openedPlayerEquips || openedPlayerInventory;

        // Configura o cursor conforme o estado do HUD
        if (hudIsOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Cursor não confinado
            Cursor.visible = true; // Cursor visível
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Cursor confinado
            Cursor.visible = false; // Cursor invisível
        }
    }

    private void OpenEquips()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            openedPlayerEquips = !openedPlayerEquips;
            playerEquips.SetActive(openedPlayerEquips);
        }
    }

    private void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            openedPlayerInventory = !openedPlayerInventory;
            inventory.SetActive(openedPlayerInventory);
        }
    }

    
}
