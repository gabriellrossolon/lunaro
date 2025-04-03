using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerComponents : MonoBehaviour
{
    public static PlayerComponents Instance { get; private set; }

    [Header("Player Components")]
    public PlayerFSM fsm;
    public PlayerStatus playerStatus;
    public LevelBehavior levelBehavior;
    public CooldownController cooldownController;
    public PlayerManager playerManager;
    public AnimationHandler animationHandler;
    public InputHandler inputHandler;
    public PlayerInput playerInput;
    public PlayerInteractions playerInteractions;
    public CharacterController characterController;
    public GameObject playerObject;
    public WeaponBehavior weaponBehavior;
    public SecondaryBehavior secondaryBehavior;
    public Canvas playerCanvas;
    public Volume playerPostProcessing;
    public Rigidbody playerRb;
    public CapsuleCollider playerCapsuleCollider;

    [Header("Non-Script Components")]
    public Camera playerCamera;
    public CinemachineVirtualCamera virtualCamera;
    public Transform followCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        playerObject = this.gameObject;
    }

    private void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        virtualCamera.Follow = followCamera;
    }
}