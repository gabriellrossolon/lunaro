using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isSlowMotionActive = false;

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
}

