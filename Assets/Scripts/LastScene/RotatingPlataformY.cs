using UnityEngine;
using System.Collections;

public class RotatingPlatformY : MonoBehaviour
{
    public float rotationSpeed = 50f;

    [Header("Configurações de Dano")]
    public float quantidadeDano = 10f;
    public float tempoEntreDano = 1.0f;
    private float proximoDanoTempo;

    [Header("Componentes de áudio")]
    public AudioSource platformAudioSource;
    public AudioClip rotationSound;

    private bool playerIsInSoundTriggerZone = false;

    void Start()
    {
        proximoDanoTempo = Time.time;

        if (platformAudioSource == null)
            Debug.LogWarning("platformAudioSource não atribuído na RotatingPlatformY.");

        if (rotationSound == null)
            Debug.LogWarning("rotationSound não atribuído na RotatingPlatformY.");

        if (platformAudioSource != null && rotationSound != null)
        {
            platformAudioSource.clip = rotationSound;
            platformAudioSource.loop = true;
            platformAudioSource.playOnAwake = false;
        }
    }

    public void SetPlayerNearStatus(bool status)
    {
        playerIsInSoundTriggerZone = status;

        if (platformAudioSource != null && rotationSound != null)
        {
            if (playerIsInSoundTriggerZone && !platformAudioSource.isPlaying)
                platformAudioSource.Play();
            else if (!playerIsInSoundTriggerZone && platformAudioSource.isPlaying)
                platformAudioSource.Stop();
        }
    }

    void Update()
    {
        transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
    }

    // --- NOVA LÓGICA DE DANO ---

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ProcessarDano(collision.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProcessarDano(other.gameObject);
        }
    }

    private void ProcessarDano(GameObject player)
    {
        if (Time.time >= proximoDanoTempo)
        {
            PlayerManager pm = player.GetComponent<PlayerManager>();
            if (pm != null)
            {
                pm.ReceberDano(quantidadeDano);
                proximoDanoTempo = Time.time + tempoEntreDano;
                Debug.Log("Plataforma Rotatória causou dano!");
            }
        }
    }
}