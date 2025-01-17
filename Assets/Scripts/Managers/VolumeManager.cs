using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VolumeManager : MonoBehaviour
{
    private GameObject sliderObj;
    private Slider volumeSlider;
    [SerializeField] private float volumeValue = 1f;

    public static VolumeManager Instance { get; private set; }

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioListener.volume = volumeValue;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void InitializeVolumeSlider()
    {
        sliderObj = GameObject.FindGameObjectWithTag("VolumeSlider");

        if (sliderObj != null)
        {
            volumeSlider = sliderObj.GetComponent<Slider>();
            volumeSlider.value = volumeValue;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    public void OnVolumeChanged(float value)
    {
        volumeValue = value;
        AudioListener.volume = volumeValue;
        PlayerPrefs.SetFloat("Volume", volumeValue);
    }
}
