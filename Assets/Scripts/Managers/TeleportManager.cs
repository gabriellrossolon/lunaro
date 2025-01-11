using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    private GameObject transition;

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
    }

    public void GoToDarkChambers()
    {
        HudManager.Instance.OpenTeleportPage();
        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithDelay("DungeonA"));
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        transition.GetComponent<Animator>().SetTrigger("end");
        player.transform.position = spawnPoint.transform.position;
        player.GetComponent<CharacterController>().enabled = true;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
