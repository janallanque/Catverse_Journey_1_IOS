using UnityEngine;
using System.Collections;

public class Teleporte : MonoBehaviour
{
    public Transform destino;
    public float alturaExtraAoChegar = 0.5f; // Garante que ele não nasça "dentro" do chão

    [Header("Configurações de Áudio da Nova Área")]
    public AudioClip musicaDestaArea;
    public float fadeDuration = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !(other.transform.parent != null && other.transform.parent.CompareTag("Player")))
            return;

        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null && other.transform.parent != null)
            cc = other.transform.parent.GetComponent<CharacterController>();

        if (cc != null && destino != null)
        {
            StartCoroutine(ExecutarTeleporte(cc));
        }
    }

    private IEnumerator ExecutarTeleporte(CharacterController cc)
    {
        // 1. Desliga o controlador
        cc.enabled = false;

        // 2. Se o player tiver Rigidbody, zeramos a velocidade para ele não chegar "rolando"
        Rigidbody rb = cc.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 3. Move o player com uma pequena folga de altura
        Vector3 posicaoFinal = destino.position + (Vector3.up * alturaExtraAoChegar);
        cc.transform.position = posicaoFinal;
        cc.transform.rotation = destino.rotation;

        // 4. Sincroniza a física
        Physics.SyncTransforms();

        // 5. Espera dois frames para garantir estabilidade total
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();

        // 6. Reativa o controle
        cc.enabled = true;

        // 7. Atualiza Manager e Áudio
        PlayerManager pm = cc.GetComponent<PlayerManager>();
        if (pm != null) pm.AtualizarEstadoTrofeu();

        if (AudioManager.instance != null && musicaDestaArea != null)
        {
            AudioManager.instance.PlayMusicWithFade(musicaDestaArea, fadeDuration);
        }
    }
}