using UnityEngine;

public class NpcPerseguidorPirata : MonoBehaviour
{
    public float distanciaDeAtaque = 2.0f;
    public float danoPorSegundo = 10f;
    public float tempoEntreDano = 1.0f;

    public string playerTag = "Player";
    private Transform jogadorTransform;
    private PlayerManager playerManager;
    private NpcMovement npcMovement;

    private float proximoDanoTempo;

    void Start()
    {
        npcMovement = GetComponent<NpcMovement>();

        if (npcMovement == null)
        {
            Debug.LogError("NpcMovement não encontrado neste NPC! Adicione o script NpcMovement ao NPC.", this);
            enabled = false;
            return;
        }

        ProcurarJogador();

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
                Debug.LogError("PlayerManager.instance não encontrado!");
            }
        }
    }

    void Update()
    {
        // Caso o jogador ainda não tenha sido encontrado (comum no build)
        if (jogadorTransform == null || playerManager == null)
        {
            ProcurarJogador();
            return;
        }

        float distanciaAoJogador = Vector3.Distance(transform.position, jogadorTransform.position);

        if (distanciaAoJogador > distanciaDeAtaque)
        {
            PerseguirJogador();
        }
        else
        {
            npcMovement.SetMoving(false);
            npcMovement.SetRunning(false);
            npcMovement.LookAtTarget(jogadorTransform.position);
            AtacarJogador();
        }
    }

    void PerseguirJogador()
    {
        npcMovement.SetMoving(true);
        npcMovement.SetRunning(true);
        npcMovement.LookAtTarget(jogadorTransform.position);
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