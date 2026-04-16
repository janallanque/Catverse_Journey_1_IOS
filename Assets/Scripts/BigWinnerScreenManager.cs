using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class BigWinnerScreenManager : MonoBehaviour
{
    public AudioClip victoryMusic;

    [Range(0f, 1f)]
    public float volumeMusica = 0.5f;

    private AudioSource audioSource;
    public int catsHouseSceneIndex = 1;
    private bool isLoading = false;

    public string saveFileName = "trophies.dat";
    public string[] trophyIDsToReset;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource)
            {
                source.Stop();
            }
        }

        if (victoryMusic != null)
        {
            audioSource.clip = victoryMusic;
            audioSource.loop = true;
            audioSource.volume = volumeMusica;
            audioSource.Play();
        }
    }

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

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        StartCoroutine(LoadSceneAsyncAndActivate());
    }

    private IEnumerator LoadSceneAsyncAndActivate()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(catsHouseSceneIndex);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;
    }

    public void ResetGameData()
    {
        // 1. Apagar TODOS os PlayerPrefs (não só alguns)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Todos os PlayerPrefs foram apagados");

        // 2. Apagar TODA a pasta persistentDataPath (limpeza completa de DADOS)
        string persistentPath = Application.persistentDataPath;

        if (Directory.Exists(persistentPath))
        {
            try
            {
                // Deleta a pasta inteira e todo seu conteúdo
                Directory.Delete(persistentPath, true);
                Debug.Log("Pasta de dados persistente apagada: " + persistentPath);

                // Recria a pasta vazia para evitar erros futuros
                Directory.CreateDirectory(persistentPath);
                Debug.Log("Pasta de dados persistente recriada vazia");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Erro ao apagar dados persistentes: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Pasta persistentDataPath não encontrada: " + persistentPath);
        }

        // 3. Limpar o cache do Unity (AssetBundles, etc.)
        if (Caching.ClearCache())
        {
            Debug.Log("Cache do Unity limpo com sucesso!");
        }
        else
        {
            Debug.LogWarning("Falha ao limpar o cache. Pode não haver cache para limpar ou o cache está em uso.");
        }

        Debug.Log("Reset completo dos DADOS e CACHE finalizado! Reiniciando o jogo...");

        isLoading = true;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        SceneManager.LoadScene(catsHouseSceneIndex);
    }
}