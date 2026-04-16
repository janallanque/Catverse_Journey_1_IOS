using UnityEngine;

public class HeartCollectible : MonoBehaviour
{
    public AudioClip coletarSom;
    [Range(0f, 1f)]
    public float volumeSom = 1f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();

            if (playerManager != null)
            {
                playerManager.vidaJogador = playerManager.vidaMaxima;
                playerManager.AtualizarBarraVida();

                if (coletarSom != null && audioSource != null)
                {
                    audioSource.PlayOneShot(coletarSom, volumeSom);
                }

                if (coletarSom != null)
                {
                    if (volumeSom > 0)
                    {
                        GetComponent<Collider>().enabled = false;
                        if (GetComponent<Renderer>() != null) GetComponent<Renderer>().enabled = false;
                        Destroy(gameObject, coletarSom.length);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}