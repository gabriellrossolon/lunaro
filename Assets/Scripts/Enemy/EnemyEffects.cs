using UnityEngine;
using System.Collections;

public class EnemyEffects : MonoBehaviour
{
    private EnemyStats enemyStats;

    private AudioSource _enemySoundSource;
    [SerializeField] private AudioSource _gruntSourceSource;
    [SerializeField] private AudioSource _stepSoundSource;
    [SerializeField] private AudioSource _specialsSoundSource;

    public Vector2 gruntPitchRange;
    public Vector2 stepPitchRange;

    [Header("VFX")]
    public GameObject attackImpactFx;
    public Material glowShader;
    
    [Header("SFX")]
    private float _originalPitch;
    public AudioClip attackSound;
    public AudioClip attackImpactSound;
    public AudioClip enemyGrunt;
    public AudioClip enemyStep;

    [Header("XP")]
    public GameObject xpOrb;

    [Header("Takenned Attacks")]
    public AudioClip attackTakenSound;
    public GameObject attackTakenParticle;
    public GameObject deathParticle;

    [Header("Special Hability")]
    public GameObject specialFx;
    public GameObject specialExitPoint;
    public AudioClip specialSfx;

    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();

        _enemySoundSource = GetComponent<AudioSource>();

        if(_enemySoundSource != null)
            _originalPitch = _enemySoundSource.pitch;

        if (enemyGrunt != null) { StartCoroutine(PlayEnemyGruntRandomly()); }
    }

    private IEnumerator PlayEnemyGruntRandomly()
    {
        while (true)
        {
            float randomInterval = Random.Range(4f, 10f);
            yield return new WaitForSeconds(randomInterval);

            if (enemyStats.currentHp <= 0) yield break;
            _gruntSourceSource.pitch = Random.Range(gruntPitchRange.x, gruntPitchRange.y);
            _gruntSourceSource.PlayOneShot(enemyGrunt);
            yield return new WaitForSeconds(enemyGrunt.length * 3);
        }
    }
    public void CallStep()
    {
        if (enemyStep == null) return;

        _stepSoundSource.pitch = Random.Range(stepPitchRange.x, stepPitchRange.y);
        _stepSoundSource.PlayOneShot(enemyStep);
    }

    public void CallSpecialAttack()
    {
        GameObject specialPrefab = Instantiate(specialFx, specialExitPoint.transform.position, Quaternion.identity);
        specialPrefab.GetComponent<EnemyExplosionBehavior>().explosionDamage = enemyStats.attackDamage;
        _specialsSoundSource.PlayOneShot(specialSfx);
    }

    public void CallDeathParticle()
    {
        if(deathParticle == null) return;

        Instantiate(deathParticle, transform.position, Quaternion.identity);
    }
}
