using UnityEngine;

public class BigVictoryMusicPlayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float volume = 0.6f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource não encontrado no BigVictoryMusicPlayer.");
            return;
        }

        StopPreviousPersistentMusic();

        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }

    private void StopPreviousPersistentMusic()
    {
        AudioSource[] sources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in sources)
        {
            if (source == audioSource)
                continue;

            if (!source.gameObject.scene.isLoaded)
            {
                source.Stop();
            }
        }
    }
}