using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance { get; private set; }

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
    }

    public void GoToScene(string sceneName, bool pauseGame = false)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithLoading(sceneName));
        SFXManager.Instance.miscSoundSource.PlayOneShot(SFXManager.Instance.teleportSound);

        if (pauseGame)
            GameManager.Instance.PauseGame();
    }

    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        if (HudManager.Instance.loadingScreen != null)
            HudManager.Instance.loadingScreen.SetActive(true);

        float elapsedTime = 0f;
        float fakeProgress = 0f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        if (PlayerComponents.Instance != null)
            PlayerComponents.Instance.characterController.enabled = false;

        while (elapsedTime < minLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            fakeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime);
            if (HudManager.Instance.loadingBar != null)
                HudManager.Instance.loadingBar.value = fakeProgress;
            yield return null;
        }

        while (!asyncLoad.isDone)
        {
            fakeProgress = Mathf.MoveTowards(fakeProgress, Mathf.Clamp01(asyncLoad.progress / 0.9f), Time.deltaTime);
            if (HudManager.Instance.loadingBar != null)
                HudManager.Instance.loadingBar.value = fakeProgress;

            if (asyncLoad.progress >= 0.9f && fakeProgress >= 1f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (scene.name != "MainMenu" && scene.name != "LoaderScene")
        {
            Instantiate(GameManager.Instance.playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
        if (scene.name == "MainMenu")
        {
            Cursor.lockState = CursorLockMode.None; 
            Cursor.visible = true;
        }

        if (PlayerComponents.Instance != null)
            PlayerComponents.Instance.characterController.enabled = true;

        if (HudManager.Instance.loadingScreen != null)
            HudManager.Instance.loadingScreen.SetActive(false);


        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Chamadas simplificadas para UI
    public void GoToHUB() => GoToScene("HUB");
    public void GoToForgottenDungeons() => GoToScene("ForgottenDungeons");
    public void GoToMenu() => GoToScene("MainMenu", pauseGame: true);
}
