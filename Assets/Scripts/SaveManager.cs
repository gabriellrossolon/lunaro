using UnityEngine;

public class SaveManager : MonoBehaviour
{
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
            Debug.LogWarning("S A L V A N D O . . .");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            LoadGame();
            Debug.LogWarning("C A R R E G A N D O . . .");
        }
    }
    public void SaveGame()
    {
        ES3AutoSaveMgr.Current.Save();
    }

    public void LoadGame()
    {
        ES3AutoSaveMgr.Current.Load();
    }

    public void DeleteSave()
    {

    }
}