using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public PlayerManager playerManager;
    private AudioSource audioSource;
    private float targetVolume;

    [Range(0f, 1f)]
    [SerializeField] private float volumeAmbienteMaximo = 0.5f;

    public AudioClip defaultBackgroundMusic;

    public int gameOverSceneIndex = 2;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Alterado de FindObjectOfType para FindAnyObjectByType
        playerManager = FindAnyObjectByType<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("PlayerManager não encontrado na cena! O AudioManager pode não funcionar corretamente.");
        }

        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        targetVolume = volumeAmbienteMaximo;
        audioSource.volume = targetVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (defaultBackgroundMusic != null && !audioSource.isPlaying)
        {
            audioSource.clip = defaultBackgroundMusic;
            audioSource.Play();
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerManager == null)
        {
            // Alterado de FindObjectOfType para FindAnyObjectByType
            playerManager = FindAnyObjectByType<PlayerManager>();
        }

        if (scene.buildIndex == gameOverSceneIndex || (playerManager != null && scene.buildIndex == playerManager.winnerSceneIndex))
        {
            StopMusic();
        }
        else
        {
            if (defaultBackgroundMusic != null)
            {
                if (!audioSource.isPlaying || audioSource.clip != defaultBackgroundMusic)
                {
                    PlayMusicWithFade(defaultBackgroundMusic);
                }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void StopMusicImmediate()
    {
        StopAllCoroutines();
        audioSource.Stop();
        audioSource.clip = null;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.volume = targetVolume;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void PlayMusicWithFade(AudioClip newClip, float duration = 1.2f)
    {
        if (audioSource.clip == newClip && audioSource.isPlaying) return;
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newClip, duration));
    }

    private IEnumerator FadeTrack(AudioClip newClip, float duration)
    {
        float currentTime = 0;
        float startVolume = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;

        if (newClip != null)
        {
            audioSource.loop = true;
            audioSource.Play();

            currentTime = 0;
            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0, targetVolume, currentTime / duration);
                yield return null;
            }

            audioSource.volume = targetVolume;
        }
    }
}