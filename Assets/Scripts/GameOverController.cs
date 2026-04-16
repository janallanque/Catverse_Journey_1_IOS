using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public float delay = 2f;
    public AudioSource gameOverSoundSource;
    public AudioClip gameOverClip;
    public int nextSceneIndex = 0;

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverSoundSource == null)
        {
            gameOverSoundSource = gameObject.AddComponent<AudioSource>();
        }
        gameOverSoundSource.playOnAwake = false;
        gameOverSoundSource.loop = false;

        if (gameOverClip != null)
        {
            gameOverSoundSource.clip = gameOverClip;
            gameOverSoundSource.volume = 1f;
            gameOverSoundSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip de Game Over não atribuído!");
        }

        StartCoroutine(LoadAfterSeconds());
    }

    IEnumerator LoadAfterSeconds()
    {
        yield return new WaitForSeconds(delay);

        if (gameOverSoundSource != null && gameOverSoundSource.isPlaying)
        {
            gameOverSoundSource.Stop();
        }

        SceneManager.LoadScene(nextSceneIndex);
    }
}