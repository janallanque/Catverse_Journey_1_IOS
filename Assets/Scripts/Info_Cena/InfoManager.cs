using UnityEngine;
using UnityEngine.SceneManagement;

public class InfoManager : MonoBehaviour
{
    public void StartGame()
    {
        PrepareForLoad();
        SceneManager.LoadScene(1);
    }

    public void ReturnMenu()
    {
        PrepareForLoad();
        SceneManager.LoadScene(0);
    }

    private void PrepareForLoad()
    {
        // 1. Para todos os efeitos de texto e sons associados a eles
        TypewriterEffect[] effects = FindObjectsByType<TypewriterEffect>(FindObjectsSortMode.None);
        foreach (TypewriterEffect effect in effects)
        {
            effect.StopEffect();
        }

        // 2. Para ABSOLUTAMENTE todos os sons da cena (global)
        // Isso limpa qualquer som de fundo ou clique que ainda esteja processando
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Stop();
        }

        // 3. Cancela qualquer corrotina pendente neste script (Manager)
        StopAllCoroutines();

        // 4. Limpeza de Memória (Garbage Collector)
        // Força o Unity a descarregar assets que não estão mais em uso
        // Isso ajuda muito se a cena de Menu for pesada
        Resources.UnloadUnusedAssets();
    }
}