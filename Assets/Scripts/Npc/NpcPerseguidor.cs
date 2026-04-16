using UnityEngine;
using ithappy.Animals_FREE;

public class NpcPerseguidor : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public float distanciaDeAtaque = 2.0f;
    public float danoPorSegundo = 10f;
    public float tempoEntreDano = 1.0f;

    [Header("Referências")]
    public string playerTag = "Player";

    private Transform jogadorTransform;
    private PlayerManager playerManager;
    private CreatureMover npcMover;

    private float proximoDanoTempo;

    private bool inicializado = false;

    void Awake()
    {
        npcMover = GetComponent<CreatureMover>();

        if (npcMover == null)
        {
            Debug.LogError("CreatureMover não encontrado neste NPC!", this);
        }
    }

    void Start()
    {
        // continua tentando encontrar o jogador até conseguir
        InvokeRepeating(nameof(EncontrarJogador), 0f, 0.5f);
    }

    void EncontrarJogador()
    {
        if (jogadorTransform != null) return;

        GameObject playerGameObject = GameObject.FindWithTag(playerTag);

        if (playerGameObject != null)
        {
            jogadorTransform = playerGameObject.transform;

            playerManager = PlayerManager.instance;

            if (playerManager != null)
            {
                inicializado = true;
                proximoDanoTempo = Time.time;

                CancelInvoke(nameof(EncontrarJogador));

                Debug.Log("NPC encontrou o jogador: " + jogadorTransform.name);
            }
            else
            {
                Debug.LogWarning("PlayerManager.instance não encontrado.");
            }
        }
    }

    void Update()
    {
        if (!inicializado || npcMover == null) return;

        float distanciaAoJogador = Vector3.Distance(transform.position, jogadorTransform.position);

        if (distanciaAoJogador > distanciaDeAtaque)
        {
            PerseguirJogador();
        }
        else
        {
            PararMovimento();
            AtacarJogador();
        }
    }

    void PerseguirJogador()
    {
        npcMover.SetInput(new Vector2(0, 1), jogadorTransform.position, true, false);
    }

    void PararMovimento()
    {
        npcMover.SetInput(Vector2.zero, jogadorTransform.position, false, false);
    }

    void AtacarJogador()
    {
        if (Time.time >= proximoDanoTempo)
        {
            if (playerManager != null)
            {
                playerManager.ReceberDano(danoPorSegundo);
            }

            proximoDanoTempo = Time.time + tempoEntreDano;
        }
    }
}