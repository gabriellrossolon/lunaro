using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    public float bobbingSpeed = 8.0f;
    public float bobbingAmount = 0.05f;
    public float sprintBobbingMultiplier = 1.5f;
    public float smoothFactor = 0.1f;
    private float timer = 0.0f;
    private float defaultY;
    private InputHandler inputHandler;

    void Start()
    {
        defaultY = transform.localPosition.y;
        inputHandler = GetComponentInParent<InputHandler>();
    }

    void Update()
    {
        MovingCameraBobbing();
    }
    private void MovingCameraBobbing()
    {
        if (inputHandler != null && inputHandler.moveInput != Vector2.zero)
        {
            float currentBobbingAmount = inputHandler.sprintInput ? bobbingAmount * sprintBobbingMultiplier : bobbingAmount;
            float currentBobbingSpeed = inputHandler.sprintInput ? bobbingSpeed * sprintBobbingMultiplier : bobbingSpeed;

            timer += Time.deltaTime * currentBobbingSpeed * Mathf.PI;
            float newY = defaultY + Mathf.Sin(timer) * currentBobbingAmount;
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, newY, transform.localPosition.z), smoothFactor);
        }
        else
        {
            timer = 0.0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, defaultY, transform.localPosition.z), smoothFactor);
        }
    }
}
