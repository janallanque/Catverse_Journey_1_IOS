using UnityEngine;

public class PortalDaFase : MonoBehaviour
{
    public Transform destino;
    public float alturaExtra = 2.0f;
    public string nomeDoTrofeuDestaFase = "12";
    public AudioClip musicaDestaArea;

    private void OnTriggerEnter(Collider other)
    {
        // Garante que é o Player (ou filho do Player)
        if (!other.CompareTag("Player") &&
            (other.transform.parent == null || !other.transform.parent.CompareTag("Player")))
            return;

        GameObject player = other.CompareTag("Player") ? other.gameObject : other.transform.parent.gameObject;
        if (player == null || destino == null) return;

        // Componentes
        CharacterController cc = player.GetComponent<CharacterController>();
        PlayerManager pm = player.GetComponent<PlayerManager>();
        PlayerMoviment pmov = player.GetComponent<PlayerMoviment>();

        // Marca que está teleportando (para o PlayerMoviment parar de aplicar gravidade/movimento)
        PlayerManager.estaTeleportando = true;

        // Lógica do troféu
        if (pm != null)
            pm.AtivarFasePorTrofeu(nomeDoTrofeuDestaFase, destino);

        // Desliga o CharacterController para poder mover o transform
        if (cc != null)
            cc.enabled = false;

        // Zera a força vertical do script de movimento
        if (pmov != null)
            pmov.ResetarForcaVertical();

        // Teleporta (igual ao seu antigo, só com altura extra e rotação alinhada)
        Vector3 novaPos = destino.position + Vector3.up * alturaExtra;
        player.transform.position = novaPos;
        player.transform.rotation = Quaternion.Euler(0f, destino.eulerAngles.y, 0f);

        // Reativa o CharacterController
        if (cc != null)
            cc.enabled = true;

        // Libera o movimento novamente
        PlayerManager.estaTeleportando = false;

        // Música da nova área
        if (AudioManager.instance != null && musicaDestaArea != null)
            AudioManager.instance.PlayMusicWithFade(musicaDestaArea, 1.5f);
    }
}