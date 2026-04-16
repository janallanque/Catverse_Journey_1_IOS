using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMoviment : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    private Animator animator;

    [SerializeField] private Transform foot;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private Transform avatarVisual;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip spinSound;
    [SerializeField] private AudioClip rollSound;

    [Header("Movement")]
    public float velocity = 8f;
    public float rotationSpeed = 180f;

    private bool isGround;
    private float yForce;

    [Header("Jump Settings")]
    public float jumpHeight = 6f;
    public float gravity = -35f;
    public float fallMultiplier = 3f;
    [Range(0, 90)] public float maxGroundAngle = 45f;

    [Header("Queda no Infinito")]
    public float alturaMorte = -50f;
    private bool jaCaiuNoInfinito = false;

    [Header("Proteção Spawn")]
    private float tempoProtecaoQueda = 1.5f;
    private float tempoSpawn;

    [Header("Pirueta no Ar")]
    public float velocidadeGiroNoAr = 720f;

    private float tempoUltimoCliqueForward;
    private const float intervaloDoubleTap = 0.3f;
    private bool especialAtivado;
    private bool jaTocouSomGiro;

    private float tempoUltimoCliqueLeft;
    private float tempoUltimoCliqueRight;

    private bool estaRolando;
    public float rollDuration = 0.6f;
    public float rollVisualRotationAngle = 90f;
    public float rollVisualSpinSpeed = 720f;

    private RaycastHit groundHit;

    private float mobileHorizontal;
    private float mobileVertical;
    private bool mobileJumpPressed;
    private bool mobileForwardPressed;
    private bool mobileLeftPressed;
    private bool mobileRightPressed;

    private bool morreu = false;
    private Coroutine gameOverCoroutine;

    private static bool jogoFinalizado = false;

    public static void FinalizarJogo()
    {
        jogoFinalizado = true;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CatsHouse")
        {
            morreu = false;
            jogoFinalizado = false;
            jaCaiuNoInfinito = false;

            PlayerManager.estaTeleportando = false; // ⭐ ESSENCIAL
        }

        InputSystem.ResetHaptics();

        ResetMobileInputs();

        tempoUltimoCliqueForward = 0f;
        tempoUltimoCliqueLeft = 0f;
        tempoUltimoCliqueRight = 0f;

        especialAtivado = false;
        estaRolando = false;
        jaTocouSomGiro = false;

        mobileHorizontal = 0f;
        mobileVertical = 0f;

        if (controller != null)
            controller.enabled = true;

        yForce = 0f;

        tempoSpawn = Time.time;
    }

    void Awake()
    {
        jogoFinalizado = false;
        morreu = false;

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (avatarVisual == null && animator != null)
            avatarVisual = animator.transform;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        jogoFinalizado = false;
        morreu = false;
        jaCaiuNoInfinito = false;

        tempoSpawn = Time.time;

        PlayerManager.estaTeleportando = false;

        ResetMobileInputs();

        if (controller != null)
            controller.enabled = true;
    }

    void Start()
    {
        StartCoroutine(ForcarLiberacaoTeleport());

        morreu = false;
        jaCaiuNoInfinito = false;
        yForce = 0f;
        jogoFinalizado = false;

        tempoSpawn = Time.time;

        PlayerManager.estaTeleportando = false;

        ResetMobileInputs();

        if (controller != null)
            controller.enabled = true;
    }

    void Update()
    {
        if (morreu || jogoFinalizado)
        {
            ResetMobileInputs();
            return;
        }

        if (PlayerManager.estaTeleportando)
        {
            if (Time.time - tempoSpawn > 2f)
                PlayerManager.estaTeleportando = false;
            else
                return;
        }

        VerificarQuedaInfinita();

        DetectarDoubleTap();
        DetectarDoubleTapRoll();

        Move();
        Jump();
    }

    void ResetMobileInputs()
    {
        mobileHorizontal = 0f;
        mobileVertical = 0f;

        mobileForwardPressed = false;
        mobileLeftPressed = false;
        mobileRightPressed = false;
        mobileJumpPressed = false;
    }

    void VerificarQuedaInfinita()
    {
        if (SceneManager.GetActiveScene().name != "CatsHouse")
            return;

        if (Time.time - tempoSpawn < tempoProtecaoQueda) return;

        if (jaCaiuNoInfinito || PlayerManager.estaTeleportando || jogoFinalizado)
            return;

        if (transform.position.y < alturaMorte)
        {
            jaCaiuNoInfinito = true;
            morreu = true;
            jogoFinalizado = true;

            if (gameOverCoroutine == null)
                gameOverCoroutine = StartCoroutine(GameOverRoutine());
        }
    }

    IEnumerator GameOverRoutine()
    {
        if (controller != null)
            controller.enabled = false;

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(2);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
            gameOverCoroutine = null;
        }
    }

    public void ResetarForcaVertical()
    {
        yForce = 0f;
    }

    public void MobileFrenteDown()
    {
        mobileVertical = 1;
        mobileForwardPressed = true;
    }

    public void MobileFrenteUp()
    {
        mobileVertical = 0;
        mobileForwardPressed = false;
    }

    public void MobileTrasDown() => mobileVertical = -1;
    public void MobileTrasUp() => mobileVertical = 0;

    public void MobileDireitaDown()
    {
        mobileHorizontal = 1;
        mobileRightPressed = true;
    }

    public void MobileDireitaUp()
    {
        mobileHorizontal = 0;
        mobileRightPressed = false;
    }

    public void MobileEsquerdaDown()
    {
        mobileHorizontal = -1;
        mobileLeftPressed = true;
    }

    public void MobileEsquerdaUp()
    {
        mobileHorizontal = 0;
        mobileLeftPressed = false;
    }

    public void JumpMobile()
    {
        mobileJumpPressed = true;
    }

    private void DetectarDoubleTap()
    {
        bool forwardPressed =
            (Keyboard.current != null &&
            (Keyboard.current.wKey.wasPressedThisFrame ||
             Keyboard.current.upArrowKey.wasPressedThisFrame))
             || mobileForwardPressed;

        if (forwardPressed)
        {
            if (Time.time - tempoUltimoCliqueForward <= intervaloDoubleTap)
                especialAtivado = true;

            tempoUltimoCliqueForward = Time.time;
        }

        mobileForwardPressed = false;
    }

    private void DetectarDoubleTapRoll()
    {
        if (!isGround) return;

        bool leftPressed =
            (Keyboard.current != null &&
            (Keyboard.current.aKey.wasPressedThisFrame ||
             Keyboard.current.leftArrowKey.wasPressedThisFrame))
             || mobileLeftPressed;

        bool rightPressed =
            (Keyboard.current != null &&
            (Keyboard.current.dKey.wasPressedThisFrame ||
             Keyboard.current.rightArrowKey.wasPressedThisFrame))
             || mobileRightPressed;

        if (leftPressed)
        {
            if (Time.time - tempoUltimoCliqueLeft <= intervaloDoubleTap)
                IniciarRolamento(Vector3.left);

            tempoUltimoCliqueLeft = Time.time;
        }

        if (rightPressed)
        {
            if (Time.time - tempoUltimoCliqueRight <= intervaloDoubleTap)
                IniciarRolamento(Vector3.right);

            tempoUltimoCliqueRight = Time.time;
        }

        mobileLeftPressed = false;
        mobileRightPressed = false;
    }

    private void IniciarRolamento(Vector3 direction)
    {
        if (estaRolando) return;

        estaRolando = true;

        if (animator) animator.SetTrigger("Roll");

        if (rollSound != null)
            PlayerSFXManager.Instance.PlaySFX(rollSound);

        StartCoroutine(HandleRoll(direction));
    }

    IEnumerator HandleRoll(Vector3 direction)
    {
        float startTime = Time.time;

        Quaternion originalRotation = avatarVisual.localRotation;
        Vector3 originalPos = avatarVisual.localPosition;

        avatarVisual.localPosition += new Vector3(0, 0.3f, 0);

        float rollZ = (direction.x < 0) ? -rollVisualRotationAngle : rollVisualRotationAngle;
        avatarVisual.localRotation *= Quaternion.Euler(0, 0, rollZ);

        while (Time.time < startTime + rollDuration)
        {
            avatarVisual.Rotate(Vector3.up, rollVisualSpinSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }

        avatarVisual.localRotation = originalRotation;
        avatarVisual.localPosition = originalPos;

        estaRolando = false;
        yForce = 0f;
    }

    public void Move()
    {
        if (controller == null || !controller.enabled) return;

        float horizontal = mobileHorizontal;
        float vertical = mobileVertical;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
        }

        if (horizontal != 0)
            transform.Rotate(0, horizontal * rotationSpeed * Time.deltaTime, 0);

        Vector3 moveDirection = transform.forward * vertical * velocity;

        controller.Move(moveDirection * Time.deltaTime);

        CheckIsGround();
    }

    private void CheckIsGround()
    {
        if (Physics.Raycast(
            foot.position + Vector3.up * 0.2f,
            Vector3.down,
            out groundHit,
            0.5f,
            collisionLayer))
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);
            isGround = angle <= maxGroundAngle;
        }
        else
        {
            isGround = false;
        }
    }

    public void Jump()
    {
        if (controller == null || !controller.enabled) return;

        if (isGround && yForce < 0)
        {
            yForce = -2f;
            jaTocouSomGiro = false;

            if (avatarVisual != null)
                avatarVisual.localRotation = Quaternion.identity;
        }

        bool jumpPressed =
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            || mobileJumpPressed;

        if (jumpPressed && isGround)
        {
            yForce = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (jumpSound != null)
                PlayerSFXManager.Instance.PlaySFX(jumpSound);

            if (Time.time - tempoUltimoCliqueForward > intervaloDoubleTap)
                especialAtivado = false;
        }

        if (!isGround && avatarVisual != null && especialAtivado)
        {
            if (!jaTocouSomGiro && spinSound != null)
            {
                PlayerSFXManager.Instance.PlaySFX(spinSound);
                jaTocouSomGiro = true;
            }

            avatarVisual.Rotate(Vector3.up, velocidadeGiroNoAr * Time.deltaTime, Space.Self);
        }

        if (!isGround)
            yForce += gravity * fallMultiplier * Time.deltaTime;

        controller.Move(Vector3.up * yForce * Time.deltaTime);

        mobileJumpPressed = false;
    }

    IEnumerator ForcarLiberacaoTeleport()
    {
        yield return new WaitForSeconds(1.5f);
        PlayerManager.estaTeleportando = false;
    }
}