using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindFirstObjectByType<PlayerManager>();
            return _instance;
        }
    }

    private bool jaVenceu = false;
    public static bool estaTeleportando = false;
    public static int saldoTotalTrofeus = 0;

    public string nomeTextoTrofeusCanvas = "TrophyText";
    public string nomeObjetoTextoMoedas = "CoinText";
    public string nomeObjetoBarraVida = "BarraDeVidaUI";

    public int catsHouseSceneIndex = 1;
    public int gameOverSceneIndex = 2;
    public int winnerSceneIndex = 3;
    public int bigWinnerSceneIndex = 5;

    public float vidaJogador = 100f;
    public float vidaMaxima = 100f;
    private Image barravida;

    public int pontuacaoMaximaFase = 400;

    [SerializeField] private AudioClip catHissSound;
    [SerializeField] private AudioClip trophyCollectSound;
    [SerializeField] private AudioClip trophyAppearSound;

    private AudioSource managerAudioSource;
    private bool jaAnunciouTrofeu = false;

    private string idTrofeuDaFaseAtual = "";
    private GameObject trofeuObjetoDaFaseAtual = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;

            managerAudioSource = GetComponent<AudioSource>();

            saldoTotalTrofeus = PlayerPrefs.GetInt("SaldoTotalTrofeusGlobal", 0);

            ColetavelItem.OnPontuacaoChanged += (pontos) =>
            {
                Scene currentScene = SceneManager.GetActiveScene();

                if (!IsCenaDeResultado(currentScene))
                {
                    AtualizarTextoMoedas();
                    AtualizarEstadoTrofeu();
                }
            };
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // -------------------------
    // TROFÉU
    // -------------------------

    public void RegistrarTrofeu(GameObject trofeu)
    {
        trofeuObjetoDaFaseAtual = trofeu;
        idTrofeuDaFaseAtual = trofeu.name;

        Debug.Log("Troféu registrado: " + idTrofeuDaFaseAtual);

        AtualizarEstadoTrofeu();
    }

    public void AtualizarEstadoTrofeu()
    {
        if (trofeuObjetoDaFaseAtual == null) return;

        bool deveAparecer =
            (ColetavelItem.totalColetaveis >= pontuacaoMaximaFase) && !jaVenceu;

        if (deveAparecer && !jaAnunciouTrofeu)
        {
            jaAnunciouTrofeu = true;

            if (trophyAppearSound != null && managerAudioSource != null)
                managerAudioSource.PlayOneShot(trophyAppearSound);
        }

        foreach (Renderer r in trofeuObjetoDaFaseAtual.GetComponentsInChildren<Renderer>(true))
            r.enabled = deveAparecer;

        foreach (Collider c in trofeuObjetoDaFaseAtual.GetComponentsInChildren<Collider>(true))
            c.enabled = deveAparecer;
    }

    // -------------------------
    // VIDA
    // -------------------------

    public void AtualizarBarraVida()
    {
        if (barravida == null)
            ReconectarElementosCena();

        if (barravida != null)
            barravida.fillAmount = Mathf.Clamp01(vidaJogador / vidaMaxima);
    }

    public void AtivarFasePorTrofeu(string nomeTrofeuParaAtivar, Transform spawnPointDaFase)
    {
        idTrofeuDaFaseAtual = nomeTrofeuParaAtivar;

        jaVenceu = false;
        jaAnunciouTrofeu = false;

        ColetavelItem.totalColetaveis = 0;

        Debug.Log("Ativando fase para troféu: " + nomeTrofeuParaAtivar);

        StopAllCoroutines();
        StartCoroutine(TeleportarParaSpawnComReset(spawnPointDaFase));
    }

    public void ReceberDano(float dano)
    {
        if (estaTeleportando) return;

        vidaJogador -= dano;
        AtualizarBarraVida();

        if (catHissSound != null && managerAudioSource != null)
            managerAudioSource.PlayOneShot(catHissSound);

        if (vidaJogador <= 0)
            SceneManager.LoadScene(gameOverSceneIndex);
    }

    // -------------------------
    // CENAS
    // -------------------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Cena carregada: " + scene.name);

        ReconectarElementosCena();

        if (scene.buildIndex == catsHouseSceneIndex || scene.buildIndex == 0)
        {
            saldoTotalTrofeus = PlayerPrefs.GetInt("SaldoTotalTrofeusGlobal", 0);

            jaVenceu = false;
            estaTeleportando = false;
            jaAnunciouTrofeu = false;

            if (vidaJogador <= 0)
                vidaJogador = vidaMaxima;

            ColetavelItem.totalColetaveis = 0;

            AtualizarTextoMoedas();

            // 🔥 REENCONTRAR TROFÉU
            StartCoroutine(ReencontrarTrofeu());

            StopAllCoroutines();
            StartCoroutine(TeleportarParaSpawnComReset(null));
        }

        // 🔥 Atualiza com delay (corrige Android)
        StartCoroutine(AtualizarTrofeuDelay());
    }

    private IEnumerator ReencontrarTrofeu()
    {
        yield return null; // espera 1 frame

        if (!string.IsNullOrEmpty(idTrofeuDaFaseAtual))
        {
            GameObject trofeu = GameObject.Find(idTrofeuDaFaseAtual);

            if (trofeu != null)
            {
                trofeuObjetoDaFaseAtual = trofeu;
                Debug.Log("Troféu reencontrado!");
            }
            else
            {
                Debug.LogWarning("Troféu NÃO encontrado: " + idTrofeuDaFaseAtual);
            }
        }
    }

    private IEnumerator AtualizarTrofeuDelay()
    {
        yield return null;
        AtualizarEstadoTrofeu();
    }

    private IEnumerator TeleportarParaSpawnComReset(Transform targetSpawn)
    {
        estaTeleportando = true;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        yield return new WaitForEndOfFrame();

        Transform finalPos = targetSpawn;

        if (finalPos == null)
        {
            GameObject spawnPadrao = GameObject.Find("SpawnPoint");
            if (spawnPadrao != null)
                finalPos = spawnPadrao.transform;
        }

        if (finalPos != null)
        {
            transform.position = finalPos.position;
            transform.rotation = finalPos.rotation;
            Physics.SyncTransforms();
        }

        yield return new WaitForSeconds(0.2f);

        if (cc != null)
            cc.enabled = true;

        estaTeleportando = false;

        EsconderTodosOsTrofeusNaCatsHouse();
    }

    private void EsconderTodosOsTrofeusNaCatsHouse()
    {
        GameObject[] todos = GameObject.FindGameObjectsWithTag("Trophy");

        foreach (GameObject t in todos)
        {
            if (t != trofeuObjetoDaFaseAtual)
            {
                foreach (Renderer r in t.GetComponentsInChildren<Renderer>(true))
                    r.enabled = false;

                foreach (Collider c in t.GetComponentsInChildren<Collider>(true))
                    c.enabled = false;
            }
        }
    }

    // -------------------------
    // COLETA
    // -------------------------

    private void ProcessarColetaTrofeu()
    {
        if (jaVenceu) return;

        jaVenceu = true;

        string chave = "TROFEU_COLETADO_ID_" + idTrofeuDaFaseAtual;

        if (PlayerPrefs.GetInt(chave, 0) == 0)
        {
            saldoTotalTrofeus++;
            PlayerPrefs.SetInt(chave, 1);
            PlayerPrefs.SetInt("SaldoTotalTrofeusGlobal", saldoTotalTrofeus);
            PlayerPrefs.Save();
        }

        if (trophyCollectSound != null && managerAudioSource != null)
            managerAudioSource.PlayOneShot(trophyCollectSound);

        AtualizarEstadoTrofeu();

        Invoke(nameof(IrParaWinner), 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estaTeleportando) return;

        if (CheckTrofeuTag(other.gameObject) &&
            ColetavelItem.totalColetaveis >= pontuacaoMaximaFase)
        {
            ProcessarColetaTrofeu();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (estaTeleportando) return;

        if (CheckTrofeuTag(hit.gameObject) &&
            ColetavelItem.totalColetaveis >= pontuacaoMaximaFase)
        {
            ProcessarColetaTrofeu();
        }
    }

    private bool CheckTrofeuTag(GameObject obj)
    {
        return obj.CompareTag("Trophy") ||
               (obj.transform.parent != null &&
                obj.transform.parent.CompareTag("Trophy"));
    }

    // -------------------------
    // UI
    // -------------------------

    public void ReconectarElementosCena()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (IsCenaDeResultado(currentScene))
            return;

        GameObject objTexto = GameObject.Find(nomeObjetoTextoMoedas);
        if (objTexto != null)
            objTexto.GetComponent<Text>().text = ColetavelItem.totalColetaveis.ToString();

        GameObject objTrophyTxt = GameObject.Find(nomeTextoTrofeusCanvas);
        if (objTrophyTxt != null)
            objTrophyTxt.GetComponent<Text>().text = saldoTotalTrofeus + "/6";

        GameObject objBarra = GameObject.Find(nomeObjetoBarraVida);
        if (objBarra != null)
            barravida = objBarra.GetComponent<Image>();
    }

    public void AtualizarTextoMoedas()
    {
        if (!IsCenaDeResultado(SceneManager.GetActiveScene()))
            ReconectarElementosCena();
    }

    private bool IsCenaDeResultado(Scene scene)
    {
        return scene.buildIndex == winnerSceneIndex ||
               scene.buildIndex == bigWinnerSceneIndex ||
               scene.buildIndex == gameOverSceneIndex;
    }

    // -------------------------
    // FLUXO
    // -------------------------

    public void IrParaWinner()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (!IsCenaDeResultado(currentScene))
        {
            if (saldoTotalTrofeus >= 6)
                SceneManager.LoadScene(bigWinnerSceneIndex);
            else
                SceneManager.LoadScene(winnerSceneIndex);
        }
    }

    public void ResetarEstadoCompleto()
    {
        saldoTotalTrofeus = 0;
        jaVenceu = false;
        estaTeleportando = false;
        jaAnunciouTrofeu = false;

        PlayerPrefs.SetInt("SaldoTotalTrofeusGlobal", 0);
        PlayerPrefs.Save();
    }
}