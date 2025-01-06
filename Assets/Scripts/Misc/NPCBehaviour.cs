using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    private GameObject player;
    private AudioSource audioSource;
    public AudioClip helloVoice;

    private void Start()
    {
        player = PlayerManager.Instance.gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(helloVoice);
        }
    }
}
