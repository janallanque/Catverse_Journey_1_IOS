using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class WinnerScreenManager : MonoBehaviour
{
    public AudioClip victoryMusic;

    [Range(0f, 1f)] // Cria a barrinha no Inspector
    public float volumeMusica = 0.5f; // Valor padrão em 50%

    private AudioSource audioSource;
    public int catsHouseSceneIndex = 1;
    private bool isLoading = false;

    void Awake()
    {
        // Garante que não há PlayerManager tentando atualizar UI
        if (PlayerManager.instance != null)
        {
            // Força uma verificação de cena no PlayerManager
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.buildIndex == 3 || currentScene.buildIndex == 5) // Winner ou BigWinner
            {
                // O PlayerManager já deve ignorar atualizações nesta cena
                Debug.Log("Winner scene loaded, PlayerManager UI updates disabled");
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (victoryMusic != null)
        {
            audioSource.clip = victoryMusic;
            audioSource.loop = true;
            audioSource.volume = volumeMusica;
            audioSource.Play();
        }
    }

    // Caso você mude o volume no Inspector com o jogo rodando, isso aqui atualiza em tempo real
    void Update()
    {
        if (audioSource != null && audioSource.volume != volumeMusica)
        {
            audioSource.volume = volumeMusica;
        }
    }

    public void ReturnToCatsHouse()
    {
        if (isLoading) return;
        isLoading = true;
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
        StartCoroutine(LoadSceneAsyncAndActivate());
    }

    private IEnumerator LoadSceneAsyncAndActivate()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(catsHouseSceneIndex);
        operation.allowSceneActivation = true;
        while (!operation.isDone) yield return null;
    }

}

[System.Serializable]
public class PlayerData
{
    public string achievements;
    public int lastSceneIndex;
}