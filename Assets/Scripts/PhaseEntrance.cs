using UnityEngine;

public class PhaseEntrance : MonoBehaviour
{

    public string nomeDoTrofeuDestaFase;

    public Transform spawnPointDestaFase;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (PlayerManager.instance != null)
            {
                PlayerManager.instance.AtivarFasePorTrofeu(nomeDoTrofeuDestaFase, spawnPointDestaFase);

            }
            else
            {
                Debug.LogError("PlayerManager.instance não encontrado. Verifique se o PlayerManager está ativo na cena.");
            }
        }
    }
}