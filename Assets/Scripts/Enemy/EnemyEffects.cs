using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    private AudioSource _enemySoundSource;
    public Vector2 gruntPitchRange;

    [Header("VFX")]
    public GameObject attackImpactFx;
    private float _originalPitch;

    [Header("SFX")]
    public AudioClip attackSound;
    public AudioClip attackImpactSound;

    public AudioClip enemyGrunt;

    [Header("Takenned Attacks")]
    public AudioClip attackTakenSound;
    public GameObject attackTakenParticle;

    private void Start()
    {
        _enemySoundSource = GetComponent<AudioSource>();
        _originalPitch = _enemySoundSource.pitch;
        if (enemyGrunt != null) { StartCoroutine(PlayEnemyGruntRandomly()); }
    }

    private System.Collections.IEnumerator PlayEnemyGruntRandomly()
    {
        while (true)
        {
            float randomInterval = Random.Range(4f, 10f);
            yield return new WaitForSeconds(randomInterval);

            _enemySoundSource.pitch = Random.Range(gruntPitchRange.x, gruntPitchRange.y);
            _enemySoundSource.PlayOneShot(enemyGrunt);
            yield return new WaitForSeconds(enemyGrunt.length);

            _enemySoundSource.pitch = _originalPitch;
        }
    }
}
