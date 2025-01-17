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
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
    }

    public void TriggerSlowMotion(float duration = 0.05f, float targetTimeScale = 0.4f)
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
        gamePaused = !gamePaused;

        HudManager.Instance.pauseUI.SetActive(gamePaused);
        Time.timeScale = gamePaused ? 0f : 1f;
    }

    public void TimeStop()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DestroyDuplicateEventSystems();
        DestroyDuplicateCameras();
    }

    private void DestroyDuplicateEventSystems()
    {
        if (mainEventSystem == null)
        {
            return;
        }

        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (eventSystems.Length > 1)
        {
            foreach (EventSystem eventSystem in eventSystems)
            {
                if (eventSystem != mainEventSystem)
                {
                    Destroy(eventSystem.gameObject);
                }
            }
        }
    }

    private void DestroyDuplicateCameras()
    {
        if (mainCamera == null)
        {
            return;
        }

        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        if (cameras.Length > 1)
        {
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

