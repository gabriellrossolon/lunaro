using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

    private GameObject transition;
    private GameObject player;

    public GameObject loadingScreen; 
    public Slider loadingBar;
    public float minLoadingTime = 2f;

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
    public void GoToHUB()
    {
        if(HudManager.Instance.teleportsPage.activeSelf)
            HudManager.Instance.OpenTeleportPage();

        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithLoading("HUB"));
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);
    }

    public void GoToDarkChambers()
    {
        HudManager.Instance.OpenTeleportPage();
        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithLoading("DarkChambers"));
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);
    }

    public void GoToForgottenDungeons()
    {
        HudManager.Instance.OpenTeleportPage();
        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithLoading("ForgottenDungeons"));
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);
    }

    public void GoToMenu()
    {
        transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithLoading("MainMenu"));
        GameManager.Instance.PauseGame();
    }

    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        SaveManager.Instance.SaveGame();

        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        float elapsedTime = 0f; 
        float fakeProgress = 0f; 

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        if (player != null)
            player.GetComponent<CharacterController>().enabled = false;

        while (elapsedTime < minLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            fakeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime); 
            if (loadingBar != null)
                loadingBar.value = fakeProgress;
            yield return null;
        }

        while (!asyncLoad.isDone)
        {
            fakeProgress = Mathf.MoveTowards(fakeProgress, Mathf.Clamp01(asyncLoad.progress / 0.9f), Time.deltaTime);
            if (loadingBar != null)
                loadingBar.value = fakeProgress;

            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");

        if (transition != null)
            transition.GetComponent<Animator>().SetTrigger("end");

        if (spawnPoint != null && player != null)
            player.transform.position = spawnPoint.transform.position;

        if (player != null)
            player.GetComponent<CharacterController>().enabled = true;

        if (InventoryManager.Instance != null)
            InventoryManager.Instance.potionsCount = InventoryManager.Instance.maxPotionsCount;

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
