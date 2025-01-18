using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

    private GameObject transition;
    private GameObject player;

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

        transition = GameObject.FindWithTag("Transition");
        player = GameObject.FindWithTag("Player");
    }

    public void GoToHUBFromMenu()
    {
        if(transition != null)
            transition.GetComponent<Animator>().SetTrigger("start");
        SceneManager.sceneLoaded += OnSceneLoaded;
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
            VolumeManager.Instance.InitializeVolumeSlider();
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
