using UnityEngine;
using System.Collections; // Importante para Coroutines

public class GateSensor : MonoBehaviour
{
    public GateController targetGateController;
    public string playerTag = "Player";
    public float delayBeforeSignalingProximity = 0f; // Novo: Atraso antes de informar o GateController

    private Coroutine proximityCoroutine; // Para controlar o timer

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && targetGateController != null)
        {
            // Se já tem um timer rodando, para ele para não ter múltiplos
            if (proximityCoroutine != null)
            {
                StopCoroutine(proximityCoroutine);
            }
            proximityCoroutine = StartCoroutine(SignalProximityAfterDelay(true));
            Debug.Log("Player entrou no Sensor! Sinalizando proximidade em " + delayBeforeSignalingProximity + "s.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && targetGateController != null)
        {
            // Se o jogador sair, sinaliza imediatamente que não está mais perto
            if (proximityCoroutine != null)
            {
                StopCoroutine(proximityCoroutine);
            }
            targetGateController.SetPlayerNearStatus(false);
            Debug.Log("Player saiu do Sensor! Não está mais próximo.");
        }
    }

    IEnumerator SignalProximityAfterDelay(bool isNear)
    {
        if (delayBeforeSignalingProximity > 0)
        {
            yield return new WaitForSeconds(delayBeforeSignalingProximity);
        }
        targetGateController.SetPlayerNearStatus(isNear);
    }
}