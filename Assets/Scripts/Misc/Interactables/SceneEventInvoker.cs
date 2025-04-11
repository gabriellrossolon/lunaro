using UnityEngine;

public class SceneEventInvoker : MonoBehaviour, IInteractable
{
    public SceneEvent sceneEvent;
    public float maxDistance = 2f;

    private AudioSource audioSouce;
    [SerializeField] private AudioClip eventTriggerSfx;

    private void Awake()
    {
        audioSouce = GetComponent<AudioSource>();
    }

    public void InvokeEvent()
    {
        if (sceneEvent != null)
            sceneEvent.TriggerEvent();

        if (audioSouce != null || eventTriggerSfx != null)
            audioSouce.PlayOneShot(eventTriggerSfx);
    }

    public void Interact()
    {
        float distance = Vector3.Distance(transform.position, PlayerComponents.Instance.playerObject.transform.position);

        if (distance <= maxDistance)
        {
            InvokeEvent();
            DisableBehavior();
        }
    }
    private void DisableBehavior()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

        if (GetComponent<Outline>() is Outline outline)
            outline.enabled = false;

        enabled = false;
    }

}