using UnityEngine;

public class BirdsSoundIsland : MonoBehaviour
{

    public AudioClip birdsSound; 
    public float detectionRadius = 5f; 
    public float soundCooldown = 2f; 

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
            PlayBirdsSound();
        }
    }

    void PlayBirdsSound()
    {
        if (birdsSound != null)
        {
            audioSource.PlayOneShot(birdsSound);
            lastSoundTime = Time.time; 
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}