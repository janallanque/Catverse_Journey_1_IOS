using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 5f;
    public float moveSpeed = 1f;
    public AxisOfMovement axis = AxisOfMovement.X;

    [Header("Settings de Dano")]
    public float quantidadeDano = 10f; // Alterado para float para ser compatível com o robô
    public float tempoEntreDano = 1.0f; // Intervalo para não dar dano infinito
    private float proximoDanoTempo;

    private Vector3 startPosition;

    public enum AxisOfMovement { X, Y, Z }

    void Start()
    {
        startPosition = transform.position;
        proximoDanoTempo = Time.time;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * (moveDistance / 2f) + (moveDistance / 2f);
        Vector3 newPosition = startPosition;

        if (axis == AxisOfMovement.X) newPosition.x = startPosition.x + offset;
        else if (axis == AxisOfMovement.Y) newPosition.y = startPosition.y + offset;
        else if (axis == AxisOfMovement.Z) newPosition.z = startPosition.z + offset;

        transform.position = newPosition;
    }

    // Detecta quando o jogador encosta (Física)
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ProcessarDano(collision.gameObject);
        }
    }

    // Detecta quando o jogador entra (Trigger)
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProcessarDano(other.gameObject);
        }
    }

    private void ProcessarDano(GameObject player)
    {
        // Verifica se já passou o tempo necessário para causar dano novamente
        if (Time.time >= proximoDanoTempo)
        {
            PlayerManager pm = player.GetComponent<PlayerManager>();
            if (pm != null)
            {
                // USANDO O MESMO MÉTODO DO ROBÔ:
                pm.ReceberDano(quantidadeDano);

                proximoDanoTempo = Time.time + tempoEntreDano;
                Debug.Log("Plataforma causou dano! Próximo dano em: " + tempoEntreDano + "s");
            }
        }
    }
}