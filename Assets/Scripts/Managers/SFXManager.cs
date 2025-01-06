using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public AudioClip actualStepSound;
    public static SFXManager Instance { get; private set; }

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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "HUB")
        {
            ambientSource.clip = HUBAmbientSound;
            musicSource.clip = HUBMusicSound;
        }
        else if (scene.name == "DungeonA")
        {
            ambientSource.clip = dungeonAmbientSound;
            musicSource.clip = dungeonMusicSound;
        }
        ambientSource.Play();
        musicSource.Play();
    }

    [Header("Ambient Sounds")]
    public AudioSource ambientSource;
    public AudioClip HUBAmbientSound;
    public AudioClip dungeonAmbientSound;

    [Header("Scene Music")]
    public AudioSource musicSource;
    public AudioClip HUBMusicSound;
    public AudioClip dungeonMusicSound;

    [Header("Player SFX")]
    public AudioSource playerSource;
    public AudioClip stepGrassSound;
    public AudioClip stepStoneSound;
    public AudioClip stepSandSound;
    public AudioClip stepWoodSound;
    public AudioClip defaultStepSound;

    // Layers for detection
    [Header("Layer Mapping")]
    public LayerMask grassLayer;
    public LayerMask stoneLayer;
    public LayerMask sandLayer;
    public LayerMask woodLayer;
    public void UpdateStepSound(Vector3 playerPosition)
    {
        RaycastHit hit;

        // Lança um Raycast para baixo a partir da posição do jogador
        if (Physics.Raycast(playerPosition, Vector3.down, out hit, 1.5f))
        {
            // Verifica se a camada detectada corresponde a alguma das superfícies mapeadas
            if ((grassLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                actualStepSound = stepGrassSound; // Som para grama
            }
            else if ((stoneLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                actualStepSound = stepStoneSound; // Som para pedra
            }
            else if ((sandLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                actualStepSound = stepSandSound; // Som para areia
            }
            else if ((woodLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
            {
                actualStepSound = stepWoodSound; // Som para madeira
            }
            else
            {
                actualStepSound = defaultStepSound; // Som padrão caso não seja nenhum dos anteriores
            }
        }
        else
        {
            actualStepSound = defaultStepSound; // Som padrão se o Raycast não atingir nada
        }
    }
}

