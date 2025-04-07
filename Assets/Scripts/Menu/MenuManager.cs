using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;

public class MenuManager : MonoBehaviour
{
    public AudioSource menuSoundSource;
    public AudioClip btnHoverSound;
    public AudioClip btnClickSound;

    public Texture2D cursorPointer;
    public Texture2D cursorClicker;

    public GameObject eventSys;
    public GameObject cam;

    public GameObject optionsUI;

    public GameObject loadingScreen;
    public Slider loadingBar;
    public float minLoadingTime = 2f;

    private bool openedPatchNotes = false;
    private bool openedOptionsUI = false;

    public Toggle fullScreenCheckbox;
    private bool isFullScreen = true;

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void GoToLastScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        string sceneToLoad = SaveManager.Instance.lastPlayerScene;
        if (!string.IsNullOrEmpty(sceneToLoad))
            StartCoroutine(LoadSceneWithLoading(sceneToLoad));
        else
            StartCoroutine(LoadSceneWithLoading("HUB"));

    }

    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        float elapsedTime = 0f;
        float fakeProgress = 0f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;


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
                asyncLoad.allowSceneActivation = true;

            yield return null;
        }
    }

    public void SetCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, new Vector2(16, 16), CursorMode.Auto);
    }

    public void OpenPatchNotes(Object buttonObject)
    {
        GameObject buttonGameObject = buttonObject as GameObject;
        Animator anim = buttonGameObject.GetComponentInParent<Animator>();
        openedPatchNotes = !openedPatchNotes;
        anim.SetTrigger(openedPatchNotes ? "open" : "close");
    }

    public void OpenOption()
    {
        openedOptionsUI = !openedOptionsUI;

        optionsUI.SetActive(openedOptionsUI);
        if (openedOptionsUI)
        {
            VolumeManager.Instance.InitializeVolumeSlider();
            VolumeManager.Instance.InitializeScreenCheckbox();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnFullScreenToggleChanged()
    {
        isFullScreen = !isFullScreen;

        Screen.fullScreen = isFullScreen;

        if (isFullScreen)
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        else
            Screen.SetResolution(1280, 720, false);

        VolumeManager.Instance.isFullScreen = isFullScreen;
        ES3.Save("isFullScreen", isFullScreen);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "LoaderSceme") return;
        InstantiatePlayer();

        HudManager.Instance?.loadingScreen?.SetActive(false);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InstantiatePlayer()
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");

        GameObject[] bonfires = GameObject.FindGameObjectsWithTag("Checkpoint");
        if(bonfires != null && bonfires.Length > 0)
        {
            foreach (GameObject bf in bonfires)
            {
                if (bf.GetComponent<Checkpoint>().checkpointActived)
                {
                    spawnPoint = bf;
                    break;
                }
            }
        }
        


        Vector3 randomOffset = new Vector3(Random.Range(2f, 2), 0f, Random.Range(-2, 2));
        Vector3 spawnPosition = spawnPoint.transform.position + randomOffset;
        Instantiate(GameManager.Instance.playerPrefab, spawnPosition, Quaternion.identity);
    }
}
