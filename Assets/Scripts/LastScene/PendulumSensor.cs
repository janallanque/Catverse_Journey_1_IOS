// Exemplo de como um Sensor poderia se comunicar com o HammerPendulum
using UnityEngine;

public class PendulumSensor : MonoBehaviour
{
    public HammerPendulum targetPendulum; // Arraste o GameObject do pêndulo aqui no Inspector
    public string playerTag = "Player"; // A tag do seu jogador

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && targetPendulum != null)
        {
            targetPendulum.SetPlayerNearStatus(true);
            Debug.Log("Player entrou na zona do pêndulo.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && targetPendulum != null)
        {
            targetPendulum.SetPlayerNearStatus(false);
            Debug.Log("Player saiu da zona do pêndulo.");
        }
    }
}