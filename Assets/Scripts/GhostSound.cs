using UnityEngine;

public class GhostSound : MonoBehaviour
{
    public AudioClip ghostSound;
    public float detectionRadius = 5f;
    public float soundCooldown = 2f;
    public float maxVolume = 1f; // Variável para o volume máximo

    private AudioSource audioSource;
    private Transform playerTransform;
    private float lastSoundTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player não encontrado! Certifique-se de que o player tem a tag 'Player'.");
            enabled = false;
        }

        lastSoundTime = -soundCooldown;
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRadius && Time.time >= lastSoundTime + soundCooldown)
        {
            // Calcula o volume baseado na distância para o player.
            // Quanto mais perto, maior o volume, até o maxVolume.
            float currentVolume = Mathf.Lerp(maxVolume, 0f, distanceToPlayer / detectionRadius);
            audioSource.volume = currentVolume; // Define o volume do AudioSource
            PlayGhostSound();
        }
    }

    void PlayGhostSound()
    {
        if (ghostSound != null)
        {
            audioSource.PlayOneShot(ghostSound);
            lastSoundTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}