using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    public float bobbingSpeed = 8.0f;
    public float bobbingAmount = 0.05f;
    public float horizontalBobbingAmount = 0.03f; // Movimento lateral alternado
    public float sprintBobbingMultiplier = 1.5f;
    public float smoothFactor = 0.1f;
    private float timer = 0.0f;
    private float defaultY;
    private float defaultX;
    private InputHandler inputHandler;

    void Start()
    {
        defaultY = transform.localPosition.y;
        defaultX = transform.localPosition.x;
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
            float currentHorizontalBobbingAmount = inputHandler.sprintInput ? horizontalBobbingAmount * sprintBobbingMultiplier : horizontalBobbingAmount;
            float currentBobbingSpeed = inputHandler.sprintInput ? bobbingSpeed * sprintBobbingMultiplier : bobbingSpeed;

            timer += Time.deltaTime * currentBobbingSpeed * Mathf.PI;

            // Movimento vertical (subindo e voltando ao centro)
            float newY = defaultY + Mathf.Abs(Mathf.Sin(timer)) * currentBobbingAmount;

            // Movimento horizontal (indo para um lado e depois para o outro)
            float newX = defaultX + Mathf.Sign(Mathf.Sin(timer)) * currentHorizontalBobbingAmount;

            // Aplicando o movimento suavemente
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(newX, newY, transform.localPosition.z), smoothFactor);
        }
        else
        {
            timer = 0.0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(defaultX, defaultY, transform.localPosition.z), smoothFactor);
        }
    }
}
