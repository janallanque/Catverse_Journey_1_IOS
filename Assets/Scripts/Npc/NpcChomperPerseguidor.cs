using UnityEngine;

public class NpcChomperPerseguidor : MonoBehaviour
{
    public float distanciaDeAtaque = 2.0f;
    public float danoPorSegundo = 10f;
    public float tempoEntreDano = 1.0f;

    public string playerTag = "Player";

    private Transform jogadorTransform;
    private PlayerManager playerManager;
    private NpcMovement npcMovement;

    private float proximoDanoTempo;
    private bool inicializado = false;

    void Start()
    {
        npcMovement = GetComponent<NpcMovement>();

        if (npcMovement == null)
        {
            Debug.LogError("NpcMovement não encontrado no NPC.");
            enabled = false;
            return;
        }

        InvokeRepeating(nameof(EncontrarJogador), 0f, 0.5f);
    }

    void EncontrarJogador()
    {
        if (jogadorTransform != null) return;

        GameObject playerGameObject = GameObject.FindWithTag(playerTag);

        if (playerGameObject != null)
        {
            jogadorTransform = playerGameObject.transform;
            playerManager = playerGameObject.GetComponent<PlayerManager>();

            if (playerManager != null)
            {
                inicializado = true;
                proximoDanoTempo = Time.time;

                CancelInvoke(nameof(EncontrarJogador));

                Debug.Log("NPC Chomper encontrou o jogador: " + jogadorTransform.name);
            }
        }
    }

    void Update()
    {
        if (!inicializado || jogadorTransform == null || playerManager == null)
            return;

        float distanciaAoJogador = Vector3.Distance(transform.position, jogadorTransform.position);

        if (distanciaAoJogador > distanciaDeAtaque)
        {
            PerseguirJogador();
        }
        else
        {
            npcMovement.SetMoving(false);
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

    public void PlayStep() { }

    public void Grunt() { }
}