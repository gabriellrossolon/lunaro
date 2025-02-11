using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    [Header("Master Volume")]
    private GameObject masterSliderObj;
    private Slider masterVolumeSlider;
    [SerializeField] private float masterVolumeValue = 1f;

    [Header("Music Volume")]
    public AudioMixer musicVolumeMixer;
    private GameObject musicSliderObj;
    private Slider musicVolumeSlider;
    [SerializeField] private float musicVolumeValue = 1f;

    [Header("Effects Volume")]
    public AudioMixer effectsVolumeMixer;
    private GameObject effectsSliderObj;
    private Slider effectsVolumeSlider;
    [SerializeField] private float effectsVolumeValue = 1f;

    private GameObject screenCheckboxObj;
    private Toggle screenCheckbox;
    public bool isFullScreen = true;

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
        AudioListener.volume = masterVolumeValue;
        musicVolumeMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(musicVolumeValue, 0.0001f)) * 20);
        effectsVolumeMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Max(effectsVolumeValue, 0.0001f)) * 20);

        ES3AutoSaveMgr.Current.Save();
    }

    private void Start()
    {
        AudioListener.volume = masterVolumeValue;
        musicVolumeMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(musicVolumeValue, 0.0001f)) * 20);
        effectsVolumeMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Max(effectsVolumeValue, 0.0001f)) * 20);

        ES3AutoSaveMgr.Current.Save();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void InitializeVolumeSlider()
    {
        masterSliderObj = GameObject.FindGameObjectWithTag("MasterVolumeSlider");
        if (masterSliderObj != null)
        {
            masterVolumeSlider = masterSliderObj.GetComponent<Slider>();
            masterVolumeSlider.value = masterVolumeValue;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        musicSliderObj = GameObject.FindGameObjectWithTag("MusicVolumeSlider");
        if (musicSliderObj != null)
        {
            musicVolumeSlider = musicSliderObj.GetComponent<Slider>();
            musicVolumeSlider.value = musicVolumeValue;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        effectsSliderObj = GameObject.FindGameObjectWithTag("EffectsVolumeSlider");
        if (effectsSliderObj != null)
        {
            effectsVolumeSlider = effectsSliderObj.GetComponent<Slider>();
            effectsVolumeSlider.value = effectsVolumeValue;
            effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeChanged);
        }
    }

    public void InitializeScreenCheckbox()
    {
        screenCheckboxObj = GameObject.FindGameObjectWithTag("ScreenCheckBox");
        if(screenCheckboxObj != null)
        {
            screenCheckbox = screenCheckboxObj.GetComponent<Toggle>();
            screenCheckbox.isOn = isFullScreen;
        }
    }

    public void OnMasterVolumeChanged(float value)
    {
        masterVolumeValue = value;
        AudioListener.volume = masterVolumeValue;
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicVolumeValue = value;
        musicVolumeMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(musicVolumeValue, 0.0001f)) * 20);
    }

    public void OnEffectsVolumeChanged(float value)
    {
        effectsVolumeValue = value;
        effectsVolumeMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Max(effectsVolumeValue, 0.0001f)) * 20);
    }
}
