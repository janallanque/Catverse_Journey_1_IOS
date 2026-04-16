using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    [Header("Text Objects")]
    public GameObject brasilTextObject;
    public GameObject euaTextObject;
    public GameObject espanhaTextObject;
    public GameObject chinaTextObject;

    void Start()
    {
        // Garante que comece apenas com o Inglês (ou o que você preferir)
        ActivateEUAText();
    }

    private void DisableAllTexts()
    {
        // Dica: Verifique se os nomes no Inspector estão atribuídos!
        if (brasilTextObject) brasilTextObject.SetActive(false);
        if (euaTextObject) euaTextObject.SetActive(false);
        if (espanhaTextObject) espanhaTextObject.SetActive(false);
        if (chinaTextObject) chinaTextObject.SetActive(false);
    }

    public void ActivateBrazilText()
    {
        DisableAllTexts();
        if (brasilTextObject) brasilTextObject.SetActive(true);
    }

    public void ActivateEUAText()
    {
        DisableAllTexts();
        if (euaTextObject) euaTextObject.SetActive(true);
    }

    public void ActivateEspanhaText()
    {
        DisableAllTexts();
        if (espanhaTextObject) espanhaTextObject.SetActive(true);
    }

    public void ActivateChinaText()
    {
        DisableAllTexts();
        if (chinaTextObject) chinaTextObject.SetActive(true);
    }
}