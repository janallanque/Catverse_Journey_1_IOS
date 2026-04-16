using UnityEngine;

public class NpcPerseguidorRobot : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public float distanciaDeAtaque = 2.0f;
    public float danoPorSegundo = 10f;
    public float tempoEntreDano = 1.0f;
    public float velocidadeDePerseguicao = 3f;
    public float velocidadeDeRotacao = 100f;
    public float gravidade = -9.81f;

    [Header("Referências")]
    public string playerTag = "Player";

    private Transform jogadorTransform;
    private PlayerManager playerManager;
    private RobotFreeAnim npcMoverAnim;
    private CharacterController controller;

    private float proximoDanoTempo;
    private bool estaPerseguindo = false;
    private Vector3 velocidadeVertical;

    void Start()
    {
        npcMoverAnim = GetComponent<RobotFreeAnim>();
        controller = GetComponent<CharacterController>();

        if (npcMoverAnim == null)
        {
            Debug.LogError("NpcPerseguidorRobot: RobotFreeAnim não encontrado neste NPC!", this);
        }

        if (controller == null)
        {
            Debug.LogError("NpcPerseguidorRobot: CharacterController não encontrado neste NPC!", this);
        }

        ProcurarJogador();

        if (npcMoverAnim != null)
        {
            npcMoverAnim.enabled = false;
        }

        proximoDanoTempo = Time.time;
    }

    void ProcurarJogador()
    {
        GameObject playerGameObject = GameObject.FindWithTag(playerTag);

        if (playerGameObject != null)
        {
            jogadorTransform = playerGameObject.transform;
            playerManager = PlayerManager.instance;

            if (playerManager == null)
            {
                Debug.LogError("NpcPerseguidorRobot: PlayerManager.instance não encontrado!");
            }
        }
    }

    void Update()
    {
        if (jogadorTransform == null || playerManager == null)
        {
            ProcurarJogador();
            return;
        }

        if (npcMoverAnim == null || controller == null)
            return;

        float distanciaAoJogador = Vector3.Distance(transform.position, jogadorTransform.position);

        if (controller.isGrounded)
        {
            velocidadeVertical.y = 0f;
        }

        velocidadeVertical.y += gravidade * Time.deltaTime;

        if (distanciaAoJogador > distanciaDeAtaque)
        {
            PerseguirJogador();
            estaPerseguindo = true;
        }
        else
        {
            PararMovimento();
            estaPerseguindo = false;
            AtacarJogador();
        }

        npcMoverAnim.SetWalkAnimation(estaPerseguindo);

        controller.Move(velocidadeVertical * Time.deltaTime);
    }

    void PerseguirJogador()
    {
        Vector3 direcaoAoJogador = (jogadorTransform.position - transform.position).normalized;
        direcaoAoJogador.y = 0;

        if (direcaoAoJogador.sqrMagnitude <= 0.001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(direcaoAoJogador);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, velocidadeDeRotacao * Time.deltaTime);

        Vector3 movimentoHorizontal = transform.forward * velocidadeDePerseguicao;
        controller.Move(movimentoHorizontal * Time.deltaTime);
    }

    void PararMovimento()
    {
        // mantém lógica original (não move)
    }

    void AtacarJogador()
    {
        if (Time.time >= proximoDanoTempo)
        {
            playerManager.ReceberDano(danoPorSegundo);
            proximoDanoTempo = Time.time + tempoEntreDano;
        }
    }
}