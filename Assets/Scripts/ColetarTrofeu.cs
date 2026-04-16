using UnityEngine;
using UnityEngine.SceneManagement;

public class ColetarTrofeu : MonoBehaviour
{
    public AudioSource somTrofeu;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se é o player e se tem as 400 moedas (ajuste o valor se necessário)
        if (other.CompareTag("Player") && ColetavelItem.totalColetaveis >= 400)
        {
            PlayerManager.saldoTotalTrofeus++;
            ColetavelItem.totalColetaveis = 0;
            if (somTrofeu != null) somTrofeu.Play();

            // Espera meio segundo para o som tocar e muda para a cena Winner (Index 3)
            Invoke("IrParaWinner", 0.5f);
        }
    }

    void IrParaWinner()
    {
        SceneManager.LoadScene(3);
    }
}