using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public SceneEvent sceneEvent;
    private Animator doorAnimator;
    private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenSfx;

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        doorAudioSource = GetComponent<AudioSource>();
        sceneEvent.OnEventTriggered += OpenDoor;
    }

    private void OnDisable()
    {
        sceneEvent.OnEventTriggered -= OpenDoor;
    }

    void OpenDoor()
    {
        doorAnimator.SetTrigger("Open");
        doorAudioSource.PlayOneShot(doorOpenSfx);
    }
}
