using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

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

    private bool openedPatchNotes = false;
    private bool openedOptionsUI = false;

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

    public void GoToHUBFromMenu()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneWithDelay("HUB"));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
}
