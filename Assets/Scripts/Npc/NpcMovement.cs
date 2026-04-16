using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float walkSpeed = 3f; 
    public float runSpeed = 6f;  
    public float rotationSpeed = 180f;
    public float gravity = -9.81f; 

    [Header("Controle de Estado")]
    public bool isMoving = false; 
    public bool isRunning = false; 

    private CharacterController characterController;
    private Animator animator;
    private Vector3 moveDirection; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController não encontrado no " + gameObject.name + ". Por favor, adicione um CharacterController.");
            enabled = false; // Desativa o script se não houver CharacterController
            return;
        }

        if (animator == null)
        {
            Debug.LogWarning("Animator não encontrado no " + gameObject.name + ". As animações não serão controladas.");
        }
    }

    void Update()
    {
        // --- 1. Aplica Gravidade ---
        if (characterController.isGrounded)
        {
            moveDirection.y = 0f; 
        }
        else
        {
            moveDirection.y += gravity * Time.deltaTime; 
        }

        // --- 2. Determina a Velocidade Atual ---
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isMoving)
        {

            // Movimento para frente
            Vector3 forwardMovement = transform.forward * currentSpeed;
            moveDirection.x = forwardMovement.x;
            moveDirection.z = forwardMovement.z;

        
        }
        else
        {
            // Se não estiver se movendo, zera o movimento horizontal
            moveDirection.x = 0f;
            moveDirection.z = 0f;
        }

        // --- 4. Move o CharacterController ---
        characterController.Move(moveDirection * Time.deltaTime);

        // --- 5. Atualiza o Animator (se houver) ---
        if (animator != null)
        {
        
            float horizontalSpeed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;

            if (horizontalSpeed > 0.1f) 
            {
                animator.SetFloat("Vert", isRunning ? 1f : 0.5f); 
            }
            else 
            {
                animator.SetFloat("Vert", 0f); 
            }

            // Se você tiver um parâmetro "isMoving" booleano no Animator:
            animator.SetBool("isMoving", horizontalSpeed > 0.1f);

        }
    }

    // Método público para mudar o estado de movimento, se necessário por outra IA ou evento
    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    // Método público para mudar o estado de corrida
    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    // Método para fazer o NPC olhar para uma direção (pode ser usado por IA)
    public void LookAtTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // Mantém a rotação apenas no eixo Y (horizontal)

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}