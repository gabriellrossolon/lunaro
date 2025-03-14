using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isSlowMotionActive = false;

    public bool gamePaused = false;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private EventSystem mainEventSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (PlayerManager.Instance.inputHandler.pauseInput)
        {
            PauseGame();
            PlayerManager.Instance.inputHandler.pauseInput = false;
        }
        Screenshot();
    }

    public void TriggerSlowMotion(float duration = 0.1f, float targetTimeScale = 0.3f)
    {
        if (isSlowMotionActive) return;
        StartCoroutine(SlowMotionEffect(duration, targetTimeScale));
    }

    private IEnumerator SlowMotionEffect(float duration, float targetTimeScale)
    {
        isSlowMotionActive = true;
        float originalTimeScale = Time.timeScale;
        float smoothTime = 0.1f, t = 0;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / smoothTime;
            Time.timeScale = Mathf.Lerp(originalTimeScale, targetTimeScale, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(duration);

        t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / smoothTime;
            Time.timeScale = Mathf.Lerp(targetTimeScale, originalTimeScale, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = 0.02f;
        isSlowMotionActive = false;
    }

    public void PauseGame()
    {
        if (TeleportManager.Instance.loadingScreen.activeSelf && gamePaused == false) return;

        gamePaused = !gamePaused;

        HudManager.Instance.pauseUI.SetActive(gamePaused);
        Time.timeScale = gamePaused ? 0f : 1f;

        if (HudManager.Instance.optionsUI.activeSelf)
            HudManager.Instance.OpenOption();
    }

    public void TimeStop()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
    }

    private void Screenshot()
    {
        if (Input.GetKey(KeyCode.F3))
        {
            string folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
            string fileName = "screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
            string fullPath = System.IO.Path.Combine(folderPath, fileName);

            ScreenCapture.CaptureScreenshot(fullPath);
            Debug.Log("Screenshot salva em: " + fullPath);
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DestroyDuplicateObjects();
    }

    private void DestroyDuplicateObjects()
    {
        if (mainEventSystem != null)
        {
            EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
            foreach (EventSystem eventSystem in eventSystems)
            {
                if (eventSystem != mainEventSystem)
                {
                    Destroy(eventSystem.gameObject);
                }
            }
        }

        if (mainCamera != null)
        {
            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (Camera camera in cameras)
            {
                if (camera != mainCamera)
                {
                    Destroy(camera.gameObject);
                }
            }
        }
    }

}

