using UnityEngine;

public class ChickenSoundOnApproach : MonoBehaviour
{
    public AudioClip chickenSound; // O som que a galinha fará
    public float detectionRadius = 5f; // Raio de detecção para o jogador
    public float soundCooldown = 2f; // Tempo de espera entre os sons da mesma galinha

    private AudioSource audioSource;
    private Transform playerTransform;
    private float lastSoundTime;

    void Start()
    {
        // Garante que a galinha tenha um AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Não tocar o som automaticamente
        audioSource.loop = false; // O som não deve fazer loop

        // Encontra o player pelo nome ou tag (ajuste conforme necessário)
        // É recomendado que seu player tenha a tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Certifique-se de que o player tem a tag 'Player'.");
            enabled = false; // Desativa o script se o player não for encontrado
        }

        lastSoundTime = -soundCooldown; // Inicializa para permitir que o som toque imediatamente
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calcula a distância entre a galinha e o player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Se o player estiver dentro do raio de detecção e o cooldown já passou
        if (distanceToPlayer <= detectionRadius && Time.time >= lastSoundTime + soundCooldown)
        {
            PlayChickenSound();
        }
    }

    void PlayChickenSound()
    {
        if (chickenSound != null)
        {
            audioSource.PlayOneShot(chickenSound);
            lastSoundTime = Time.time; // Atualiza o tempo do último som tocado
        }
    }

    // Opcional: Desenha o raio de detecção na Scene View para visualização
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}