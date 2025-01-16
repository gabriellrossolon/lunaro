using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    private GameObject transition;
    private GameObject player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        transition = GameObject.FindWithTag("Transition");
        player = GameObject.FindWithTag("Player");
    }

    public void GoToDarkChambers()
    {
        HudManager.Instance.OpenTeleportPage();
        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithDelay("DungeonA"));
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);
    }
    public void GoToMenu()
    {
        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithDelay("MainMenu"));
        GameManager.Instance.PauseGame();
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);

        if(player != null)
            player.GetComponent<CharacterController>().enabled = false;

        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");

        if (transition != null)
            transition.GetComponent<Animator>().SetTrigger("end");

        if(spawnPoint != null)
            player.transform.position = spawnPoint.transform.position;

        if (player != null)
            player.GetComponent<CharacterController>().enabled = true;

        if(InventoryManager.Instance != null)
            InventoryManager.Instance.potionsCount = InventoryManager.Instance.maxPotionsCount;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
