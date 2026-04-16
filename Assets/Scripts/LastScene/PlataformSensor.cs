using UnityEngine;

public class PlatformSensor : MonoBehaviour
{
    public RotatingPlatformY targetPlatform; // Arraste o GameObject da plataforma aqui
    public string playerTag = "Player"; // A tag do seu jogador

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && targetPlatform != null)
        {
            targetPlatform.SetPlayerNearStatus(true);
            Debug.Log("Player entrou na zona da plataforma.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && targetPlatform != null)
        {
            targetPlatform.SetPlayerNearStatus(false);
            Debug.Log("Player saiu da zona da plataforma.");
        }
    }
}