using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationHandler : MonoBehaviour
{
    private Animator _animator;
    private InputHandler _handler;

    private float velocityZ;
    private float velocityX;
    private float acceleration = 2f;
    private float deceletarion = 4f;
    private float maximumWalkVelocity = 0.5f;
    private float maximumRunVelocity = 2f;
    int VelocityZHash;
    int VelocityXHash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _handler = GetComponent<InputHandler>();
    }

    private void Start()
    {
        VelocityZHash = Animator.StringToHash("velocityZ");
        VelocityXHash = Animator.StringToHash("velocityX");
    }

    private void Update()
    {
        MovementAnimationController();
    }

    private void MovementAnimationController()
    {
        bool forwardPressed = Keyboard.current.wKey.isPressed;
        bool backwardPressed = Keyboard.current.sKey.isPressed;
        bool leftPressed = Keyboard.current.aKey.isPressed;
        bool rightPressed = Keyboard.current.dKey.isPressed;
        bool runPressed = Keyboard.current.shiftKey.isPressed;

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        ChangeVelocity(forwardPressed, backwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        LockOrResetVelocity(forwardPressed, backwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        if(_handler.moveInput != Vector2.zero)
        {
            _animator.SetFloat(VelocityZHash, velocityZ);
            _animator.SetFloat(VelocityXHash, velocityX);
        }
        else
        {
            _animator.SetFloat(VelocityZHash, 0f);
            _animator.SetFloat(VelocityXHash, 0f);
        }
       
    }

    private void ChangeVelocity(bool forwardPressed, bool backwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // Movimento para frente
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        // Movimento para trás
        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        // Movimento para a esquerda
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        // Movimento para a direita
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        // Desaceleração para frente e trás
        if (!forwardPressed && velocityZ > 0f)
        {
            velocityZ -= Time.deltaTime * deceletarion;
        }
        if (!backwardPressed && velocityZ < 0f)
        {
            velocityZ += Time.deltaTime * deceletarion;
        }

        // Desaceleração para esquerda e direita
        if (!leftPressed && velocityX < 0f)
        {
            velocityX += Time.deltaTime * deceletarion;
        }
        if (!rightPressed && velocityX > 0f)
        {
            velocityX -= Time.deltaTime * deceletarion;
        }
    }

    private void LockOrResetVelocity(bool forwardPressed, bool backwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // Resetar velocidade Z quando não houver movimento
        if (!forwardPressed && !backwardPressed && velocityZ != 0f && Mathf.Abs(velocityZ) < 0.05f)
        {
            velocityZ = 0f;
        }

        // Resetar velocidade X quando não houver movimento
        if (!leftPressed && !rightPressed && velocityX != 0f && Mathf.Abs(velocityX) < 0.05f)
        {
            velocityX = 0f;
        }

        // Limitar velocidade máxima para frente
        if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }

        // Limitar velocidade máxima para trás
        if (backwardPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }

        // Limitar velocidade máxima para a esquerda
        if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }

        // Limitar velocidade máxima para a direita
        if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
    }
}

