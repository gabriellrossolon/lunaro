using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public string lastPlayerScene;

    public static SaveManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Instance = this;

        if (ES3.KeyExists("lastPlayerScene"))
            lastPlayerScene = ES3.Load<string>("lastPlayerScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
            Debug.LogWarning("S A L V A N D O . . .");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            DeleteSave();
            Debug.LogWarning("D E L E T A N D O . . .");
        }
    }
    public void SaveGame()
    {
        PlayerComponents.Instance.playerStatus.SavePlayerStatus();
        InventoryManager.Instance.SaveInventory();
    }

    public void DeleteSave()
    {
        ES3.DeleteFile("EasySave3.es3");
    }
}