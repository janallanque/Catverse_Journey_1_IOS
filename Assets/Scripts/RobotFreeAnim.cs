using UnityEngine;

public class RobotFreeAnim : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float rotSpeed = 400f;

    private Animator anim;
    private readonly int walkAnimHash = Animator.StringToHash("Walk_Anim");
    private readonly int rollAnimHash = Animator.StringToHash("Roll_Anim");
    private readonly int openAnimHash = Animator.StringToHash("Open_Anim");

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Removido: HandleMovement();
        // Removido: HandleAnimations();
        // A lógica de movimento e animação será controlada pelo NpcPerseguidorRobot
        // e por métodos públicos aqui, se necessário.
    }

    // NOVO MÉTODO: Para que outros scripts possam controlar a animação de andar
    public void SetWalkAnimation(bool isWalking)
    {
        if (anim != null)
        {
            anim.SetBool(walkAnimHash, isWalking);
        }
    }

    // Se você precisar que o robô faça outras animações específicas (rolar, abrir)
    // controladas por este NPC Perseguidor, adicione métodos similares:
    public void SetRollAnimation(bool isRolling)
    {
        if (anim != null)
        {
            anim.SetBool(rollAnimHash, isRolling);
        }
    }

    public void SetOpenAnimation(bool isOpen)
    {
        if (anim != null)
        {
            anim.SetBool(openAnimHash, isOpen);
        }
    }

}