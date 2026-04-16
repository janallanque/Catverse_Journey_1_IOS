using UnityEngine;
using System.Collections;

public class CatAmbientSounds : MonoBehaviour
{
    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip meowClip;
    [SerializeField] private AudioClip purringClip;

    // NOVAS VARIÁVEIS PARA CONTROLAR O VOLUME INDIVIDUAL
    [Header("Volumes Individuais (0 a 1)")]
    [Range(0f, 1f)][SerializeField] private float volumeMiau = 0.5f;
    [Range(0f, 1f)][SerializeField] private float volumePurring = 1.0f; // Purring mais alto por padrão

    [Header("Temporizador")]
    [SerializeField] private float intervaloEmSegundos = 60f;

    private bool ehVezDoMiau = true;
    private Coroutine rotinaSons;

    void OnEnable()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        PararRotina();
        rotinaSons = StartCoroutine(RotinaSonsGato());
    }

    void OnDisable()
    {
        PararRotina();
    }

    void PararRotina()
    {
        if (rotinaSons != null)
        {
            StopCoroutine(rotinaSons);
            rotinaSons = null;
        }
    }

    IEnumerator RotinaSonsGato()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloEmSegundos);

            if (ehVezDoMiau)
            {
                // Passa o clipe e o volume específico do miau
                TocarSom(meowClip, volumeMiau);
            }
            else
            {
                // Passa o clipe e o volume específico do purring
                TocarSom(purringClip, volumePurring);
            }

            ehVezDoMiau = !ehVezDoMiau;
        }
    }

    // Função atualizada para aceitar um parâmetro de volume
    private void TocarSom(AudioClip clip, float volume)
    {
        if (audioSource != null && clip != null)
        {
            // O PlayOneShot aceita um segundo parâmetro: o volume scale (0.0 a 1.0)
            audioSource.PlayOneShot(clip, volume);
        }
    }
}