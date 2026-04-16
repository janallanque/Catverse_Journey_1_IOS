using UnityEngine;
using UnityEngine.UI;

public class ColetavelItem : MonoBehaviour
{
    public static int totalColetaveis = 0;
    public static event System.Action<int> OnPontuacaoChanged;

    [SerializeField] private Text textoColetavel;
    [SerializeField] private string nomeDoObjetoDeTexto = "CoinText";
    [SerializeField] private AudioClip somMoeda;
    [Range(0f, 2f)][SerializeField] private float volumeMoeda = 1.5f;

    void Start()
    {
        if (textoColetavel == null)
        {
            GameObject uiTexto = GameObject.Find(nomeDoObjetoDeTexto);
            if (uiTexto != null)
                textoColetavel = uiTexto.GetComponent<Text>();
        }
        AtualizarTextoDaUI(totalColetaveis);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Coletar();
        }
    }

    private void Coletar()
    {
        totalColetaveis += 25;

        if (somMoeda != null && AudioManager.instance != null)
        {
            AudioSource audioGlobal = AudioManager.instance.GetComponent<AudioSource>();
            audioGlobal.PlayOneShot(somMoeda, volumeMoeda);
        }
        else if (somMoeda != null)
        {
            AudioSource.PlayClipAtPoint(somMoeda, Camera.main.transform.position, volumeMoeda);
        }

        if (OnPontuacaoChanged != null)
        {
            OnPontuacaoChanged.Invoke(totalColetaveis);
        }

        Destroy(gameObject);
    }

    public void AtualizarTextoDaUI(int valor)
    {
        if (textoColetavel != null)
        {
            textoColetavel.text = valor.ToString();
        }
        else
        {
            GameObject uiTexto = GameObject.Find(nomeDoObjetoDeTexto);
            if (uiTexto != null)
            {
                textoColetavel = uiTexto.GetComponent<Text>();
                if (textoColetavel != null)
                {
                    textoColetavel.text = valor.ToString();
                }
            }
        }
    }

    public static void ResetTotalColetaveis()
    {
        totalColetaveis = 0;
        if (OnPontuacaoChanged != null)
        {
            OnPontuacaoChanged.Invoke(totalColetaveis);
        }
    }
}