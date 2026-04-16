using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSFXManager : MonoBehaviour
{
    public static PlayerSFXManager Instance;

    private AudioSource sfxSource;

    void Awake()
    {
        Debug.Log("PlayerSFXManager Awake");

        if (Instance != null && Instance != this)
        {
            Debug.Log("PlayerSFXManager duplicado destruído");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = GetComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f; // 2D
        sfxSource.volume = 1f;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (!sfxSource.enabled)
            sfxSource.enabled = true;

        sfxSource.PlayOneShot(clip);
    }
}