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

    public AudioClip goblinGrunt;

    private void Start()
    {
        _enemySoundSource = GetComponent<AudioSource>();
        _originalPitch = _enemySoundSource.pitch;
        if (goblinGrunt != null) { StartCoroutine(PlayGoblinGruntRandomly()); }
    }

    private System.Collections.IEnumerator PlayGoblinGruntRandomly()
    {
        while (true)
        {
            float randomInterval = Random.Range(5f, 15f);
            yield return new WaitForSeconds(randomInterval);

            _enemySoundSource.pitch = Random.Range(gruntPitchRange.x, gruntPitchRange.y);
            _enemySoundSource.PlayOneShot(goblinGrunt);
            yield return new WaitForSeconds(goblinGrunt.length);

            _enemySoundSource.pitch = _originalPitch;
        }
    }
}
