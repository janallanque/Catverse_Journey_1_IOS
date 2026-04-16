using UnityEngine;

public class MusicaAmbienteInicial : MonoBehaviour
{
    public AudioClip musicaDeInicio;
    public float fadeDuration = 1.5f;

    void Start()
    {
        // Toca a música assim que a cena abre
        if (AudioManager.instance != null && musicaDeInicio != null)
        {
            AudioManager.instance.PlayMusicWithFade(musicaDeInicio, fadeDuration);
        }
    }
}