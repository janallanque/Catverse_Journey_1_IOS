using UnityEngine;

public class TeleporteCompleto : MonoBehaviour
{
    [Header("Destino")]
    public Transform destino;

    [Header("Configurações de Luz Condicional")]
    public Light luzDaCasa;
    public Light luzDoEldorado;
    public bool eATeleporteParaEldorado;

    [Header("Configurações de Áudio")]
    public AudioClip musicaDestaArea;
    public float fadeDuration = 1.5f;

    [Header("Skybox Casa")]
    public Material skyboxDaCasa;
    [Range(0f, 2f)]
    public float ambientIntensityDaCasa = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Transform player = other.transform;
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        // Teleporta diretamente (SEM desativar GameObject)
        player.position = destino.position;
        player.rotation = destino.rotation;

        Physics.SyncTransforms();

        if (cc != null)
            cc.enabled = true;

        GerenciarLuzes();
        TrocarMusica();
    }

    private void GerenciarLuzes()
    {
        if (eATeleporteParaEldorado)
        {
            if (luzDoEldorado != null)
            {
                luzDoEldorado.gameObject.SetActive(true);
                luzDoEldorado.enabled = true;
                RenderSettings.sun = luzDoEldorado;
            }

            if (luzDaCasa != null)
            {
                luzDaCasa.enabled = false;
                luzDaCasa.gameObject.SetActive(false);
            }
        }
        else
        {
            if (luzDaCasa != null)
            {
                luzDaCasa.gameObject.SetActive(true);
                luzDaCasa.enabled = true;
                RenderSettings.sun = luzDaCasa;
            }

            if (luzDoEldorado != null)
            {
                luzDoEldorado.enabled = false;
                luzDoEldorado.gameObject.SetActive(false);
            }

            if (skyboxDaCasa != null)
            {
                RenderSettings.skybox = skyboxDaCasa;
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            }
            else
            {
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                RenderSettings.ambientLight = Color.grey;
            }

            RenderSettings.ambientIntensity = ambientIntensityDaCasa;
        }

        DynamicGI.UpdateEnvironment();
    }

    private void TrocarMusica()
    {
        if (AudioManager.instance != null && musicaDestaArea != null)
        {
            AudioManager.instance.PlayMusicWithFade(musicaDestaArea, fadeDuration);
        }
    }
}