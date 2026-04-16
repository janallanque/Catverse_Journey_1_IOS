using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public float delayBetweenChars = 0.05f;
    [TextArea(3, 10)]
    public string fullText;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public float pitchVariation = 0.1f;

    private TextMeshProUGUI textComponent;
    private Coroutine typingCoroutine;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        // Tenta pegar o AudioSource no próprio objeto se não foi arrastado manualmente
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
            audioSource.loop = true;
    }

    void OnEnable()
    {
        ResetAndStart();
    }

    void OnDisable()
    {
        StopEffect();
    }

    public void ResetAndStart()
    {
        StopEffect();
        if (textComponent != null) textComponent.text = "";
        typingCoroutine = StartCoroutine(ShowText());
    }

    public void StopEffect()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if (audioSource != null)
            audioSource.Stop();

        if (textComponent != null)
            textComponent.text = "";
    }

    IEnumerator ShowText()
    {
        // Agora ele usa o som que já estiver carregado no AudioSource do Inspector
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);
            audioSource.Play();
        }

        foreach (char c in fullText.ToCharArray())
        {
            if (textComponent != null) textComponent.text += c;
            yield return new WaitForSeconds(delayBetweenChars);
        }

        if (audioSource != null)
            audioSource.Stop();
    }
}