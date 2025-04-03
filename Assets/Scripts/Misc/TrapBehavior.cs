using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    private AudioSource trapSoundSourc;
    [SerializeField] private AudioClip trapImpactSound;
    [SerializeField] private AudioClip slashingSound;
    [SerializeField] private GameObject trapImpactParticle;
    private Animator trapAnimator;
    private Collider trapCollider;

    [SerializeField] private float damage;
    public enum TrapType{ Nothing, Axe}

    [SerializeField] private TrapType trapType;

    private void Awake()
    {
        trapSoundSourc = GetComponent<AudioSource>();
        trapAnimator = GetComponent<Animator>();
        trapCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        trapAnimator.SetBool("AxeTrap", trapType == TrapType.Axe);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            trapSoundSourc.PlayOneShot(trapImpactSound);
            GameManager.Instance.TriggerSlowMotion();
            PlayerComponents.Instance.playerManager.DoDamage(damage);
            Instantiate(trapImpactParticle, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
        }
    }

    public void CallSlashingSound()
    {
        trapSoundSourc.pitch = Random.Range(0.9f, 1.1f);
        trapSoundSourc.PlayOneShot(slashingSound);
    }
}
