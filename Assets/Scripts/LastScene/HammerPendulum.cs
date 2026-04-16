using UnityEngine;
using System.Collections;

public class HammerPendulum : MonoBehaviour
{
    public float pendulumAngle = 45f;
    public float cycleTime = 5f;
    public bool invertDirection = false;

    [Header("Configurações de Dano")]
    public float quantidadeDano = 15f; // Pêndulos costumam tirar mais vida!
    public float tempoEntreDano = 0.8f;
    private float proximoDanoTempo;

    [Header("Componentes de áudio")]
    public AudioSource pendulumAudioSource;
    public AudioClip swingSound;

    private bool playerIsInSoundTriggerZone = false;
    private bool hasPlayedSoundForSwing = false;

    void Start()
    {
        proximoDanoTempo = Time.time;

        if (pendulumAudioSource == null)
            Debug.LogWarning("pendulumAudioSource não atribuído no HammerPendulum.");

        if (swingSound == null)
            Debug.LogWarning("swingSound não atribuído no HammerPendulum.");

        StartCoroutine(PendulumMovement());
    }

    public void SetPlayerNearStatus(bool status)
    {
        playerIsInSoundTriggerZone = status;
    }

    IEnumerator PendulumMovement()
    {
        float timer = 0f;
        while (true)
        {
            float normalizedTime = Mathf.PingPong(timer / cycleTime, 1f);

            float currentAngle;
            if (invertDirection)
                currentAngle = Mathf.Lerp(pendulumAngle, -pendulumAngle, normalizedTime);
            else
                currentAngle = Mathf.Lerp(-pendulumAngle, pendulumAngle, normalizedTime);

            transform.localRotation = Quaternion.Euler(0, 0, currentAngle);

            if (playerIsInSoundTriggerZone && pendulumAudioSource != null && swingSound != null)
            {
                if (normalizedTime < 0.05f || normalizedTime > 0.95f)
                {
                    if (!hasPlayedSoundForSwing)
                    {
                        pendulumAudioSource.PlayOneShot(swingSound);
                        hasPlayedSoundForSwing = true;
                    }
                }
                else
                {
                    hasPlayedSoundForSwing = false;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    // --- LÓGICA DE DANO (IGUAL AOS OUTROS) ---

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ProcessarDano(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
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
                Debug.Log("Pêndulo atingiu o jogador!");
            }
        }
    }
}