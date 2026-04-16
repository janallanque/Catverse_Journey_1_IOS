using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [Header("Configurações de Dano")]
    public float quantidadeDano = 15f;
    public float tempoEntreDano = 0.8f; // Cooldown para não matar instantaneamente
    private float proximoDanoTempo;

    [Header("Tag do Jogador")]
    public string playerTag = "Player";

    void Start()
    {
        // Inicializa o tempo para permitir dano imediato no primeiro toque
        proximoDanoTempo = Time.time;
    }

    // Detecta colisão com objetos físicos (com Rigidbody/Collider)
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            TentarCausarDano(collision.gameObject);
        }
    }

    // Detecta colisão com objetos tipo gatilho (Is Trigger marcado)
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            TentarCausarDano(other.gameObject);
        }
    }

    private void TentarCausarDano(GameObject objetoJogador)
    {
        // Verifica se já passou o tempo de espera (cooldown)
        if (Time.time >= proximoDanoTempo)
        {
            PlayerManager pm = objetoJogador.GetComponent<PlayerManager>();

            if (pm != null)
            {
                pm.ReceberDano(quantidadeDano);
                proximoDanoTempo = Time.time + tempoEntreDano;

                Debug.Log($"Dano de {quantidadeDano} aplicado ao Player por {gameObject.name}");
            }
        }
    }
}