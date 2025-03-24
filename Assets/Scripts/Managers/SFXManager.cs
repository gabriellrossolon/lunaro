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
        else if (scene.name == "DarkChambers")
        {
            ambientSource.clip = darkChambersAmbientSound;
            musicSource.clip = darkChambersMusicSound; 
        }
        else if (scene.name == "FrozenRuins")
        {
            ambientSource.clip = frozenRuinsAmbientSound;
            musicSource.clip = frozenRuinsAmbientSound;
        }
        else if (scene.name == "HollowbarkDephts")
        {
            ambientSource.clip = hollowbarkDephtsAmbientSound;
            musicSource.clip = hollowbarkDephtsAmbientSound;
        }
        else if (scene.name == "ForsakenHamlet")
        {
            ambientSource.clip = forsakenHamletAmbientSound;
            musicSource.clip = forsakenHamletAmbientSound;
        }

        ambientSource.Play();
        musicSource.Play();
    }

    [Header("Ambient Sounds")]
    public AudioSource ambientSource;
    public AudioClip HUBAmbientSound;
    public AudioClip darkChambersAmbientSound;
    public AudioClip frozenRuinsAmbientSound;
    public AudioClip hollowbarkDephtsAmbientSound;
    public AudioClip forsakenHamletAmbientSound;

    [Header("Scene Music")]
    public AudioSource musicSource;
    public AudioClip HUBMusicSound;
    public AudioClip darkChambersMusicSound;
    public AudioClip frozenRuinsMusicSound;
    public AudioClip hollowbarkDephtsMusictSound;
    public AudioClip forsakenHamletMusicSound;

    [Header("Player Foot SFX")]
    public AudioSource playerFootSource;
    public AudioClip stepGrassSound;
    public AudioClip stepStoneSound;
    public AudioClip stepSandSound;
    public AudioClip stepWoodSound;
    public AudioClip stepSnowSound;
    public AudioClip defaultStepSound;

    [Header("Player Movement SFX")]
    public AudioSource playerMovementsSource;
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip landingSound;

    [Header("Player Attacks SFX")]
    public AudioSource playerAttacksSource;
    public AudioClip attackSwooshSound;
    public AudioClip attackHeavySwooshSound;
    public AudioClip fleshImpactSound;
    public AudioClip magicCastSound;
    public AudioClip bowSwooshSound;

    [Header("Player Skills SFX")]
    public AudioClip circularCutSound;
    public AudioClip activeSwordAura;
    public AudioClip airCutInstanceSound;
    public AudioClip heavyImpactSound;

    [Header("UI SFX")]
    public AudioSource UIMiscSource;
    public AudioClip btnHoverSound;
    public AudioClip btnClickSound;

    [Header("Misc SFX")]
    public AudioSource miscSoundSource;
    public AudioClip objectThrowSound;
    public AudioClip inventoryEquipSound;
    public AudioClip itemPickupSound;
    public AudioClip teleportSound;
    public AudioClip drinkSound;
    public AudioClip chestOpeningSound;
    public AudioClip xpCollectSound;
    public AudioClip levelUpSound;
    public AudioClip deathSound;
    public AudioClip dropSound;
    public AudioClip interactSound;

    // Layers for detection
    [Header("Layer Mapping")]
    public LayerMask grassLayer;
    public LayerMask stoneLayer;
    public LayerMask sandLayer;
    public LayerMask woodLayer;
    public LayerMask snowLayer;
    public void UpdateStepSound(Vector3 playerPosition)
    {
        RaycastHit hit;

        if (Physics.Raycast(playerPosition, Vector3.down, out hit, 1.5f))
        {
            if ((grassLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
                actualStepSound = stepGrassSound;
            else if ((stoneLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
                actualStepSound = stepStoneSound;
            else if ((sandLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
                actualStepSound = stepSandSound;
            else if ((woodLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
                actualStepSound = stepWoodSound;
            else if ((snowLayer.value & (1 << hit.collider.gameObject.layer)) > 0)
                actualStepSound = stepSnowSound;
            else
                actualStepSound = defaultStepSound;
        }
        else
        {
            actualStepSound = defaultStepSound;
        }
    }
}

