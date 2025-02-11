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

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) // Correção aqui
        {
            SaveGame();
        }
    }

    public void SaveGame()
    {
        Debug.Log("Salvando...");
        ES3AutoSaveMgr.Current.Save();
    }
}

