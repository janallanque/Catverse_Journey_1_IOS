using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
    public Transform gatePart1;
    public Transform gatePart2;

    public float openDistance = 2f; // Distância que cada parte se move para "abrir"
    public float moveSpeed = 1f;
    public float waitTimeAtClosed = 2f; // Tempo de espera quando os portões estão fechados (azul)
    public float waitTimeAtOpen = 1f; // Tempo de espera quando os portões estão abertos (setas vermelhas)

    public AudioSource gateAudioSource;
    public AudioClip closeSound; // Som que toca APENAS quando os portões chegam na posição FECHADA (azul)

    private bool playerIsInSoundTriggerZone = false;
    private bool hasPlayedCloseSound = false; // Controla o som de fechamento

    private Vector3 initialPos1; // Posição inicial (FECHADA - central azul) para gatePart1
    private Vector3 initialPos2; // Posição inicial (FECHADA - central azul) para gatePart2

    private Vector3 openPos1; // Posição para gatePart1 quando ABERTO (setas vermelhas)
    private Vector3 openPos2; // Posição para gatePart2 quando ABERTO (setas vermelhas)

    void Start()
    {
        if (gatePart1 != null && gatePart2 != null)
        {
            // As posições iniciais (FECHADAS - central azul) são as que você define no editor.
            // Posicione seus prefabs JUNTOS no editor para que esta seja a posição inicial.
            initialPos1 = gatePart1.localPosition;
            initialPos2 = gatePart2.localPosition;

            // Calculamos as posições "abertas" a partir das posições iniciais (fechadas).
            // **IMPORTANTE:** Ajuste a direção e o eixo conforme a sua configuração no Unity!
            // Exemplo: se `openDistance` faz os portões se moverem em Z para abrir:
            openPos1 = initialPos1;
            openPos2 = initialPos2;
            openPos1.z = initialPos1.z - openDistance; // Move gatePart1 para a esquerda em Z para abrir
            openPos2.z = initialPos2.z + openDistance; // Move gatePart2 para a direita em Z para abrir

            // Se eles se movem em X, você ajustaria:
            // openPos1.x = initialPos1.x - openDistance;
            // openPos2.x = initialPos2.x + openDistance;

            // Garantir que os portões comecem na posição FECHADA (juntos - central azul)
            gatePart1.localPosition = initialPos1;
            gatePart2.localPosition = initialPos2;

            StartCoroutine(GateMovement());
        }
        else
        {
            Debug.LogError("Por favor, atribua gatePart1 e gatePart2 no Inspector!");
            enabled = false;
            return;
        }

        if (gateAudioSource == null)
        {
            Debug.LogWarning("gateAudioSource não atribuído no GateController. O som não será reproduzido.");
        }
    }

    public void SetPlayerNearStatus(bool status)
    {
        playerIsInSoundTriggerZone = status;
    }

    IEnumerator GateMovement()
    {
        while (true)
        {
            // --- FASE 1: Espera inicial quando FECHADO (central azul) ---
            // Toca o som de fechamento AQUI, porque esta é a posição inicial e final fechada.
            if (playerIsInSoundTriggerZone && !hasPlayedCloseSound && gateAudioSource != null && closeSound != null)
            {
                gateAudioSource.PlayOneShot(closeSound);
                hasPlayedCloseSound = true;
                Debug.Log("Som de fechamento disparado quando os portões estão fechados (central azul)! " + Time.time);
            }
            yield return new WaitForSeconds(waitTimeAtClosed);

            // --- FASE 2: Abertura (MOVIMENTO: de FECHADO para ABERTO - setas vermelhas) ---
            float openTimer = 0f;
            Vector3 startPosOpening1 = gatePart1.localPosition;
            Vector3 startPosOpening2 = gatePart2.localPosition;

            hasPlayedCloseSound = false; // Resetar o flag do som para o próximo fechamento

            while (openTimer < 1f)
            {
                gatePart1.localPosition = Vector3.Lerp(startPosOpening1, openPos1, openTimer);
                gatePart2.localPosition = Vector3.Lerp(startPosOpening2, openPos2, openTimer);
                openTimer += Time.deltaTime * moveSpeed;
                yield return null;
            }
            gatePart1.localPosition = openPos1;
            gatePart2.localPosition = openPos2;

            // --- FASE 3: Espera quando ABERTO (setas vermelhas) ---
            // NENHUM SOM É TOCADO NESTA FASE.
            yield return new WaitForSeconds(waitTimeAtOpen);

            // --- FASE 4: Fechamento (MOVIMENTO: de ABERTO para FECHADO - retorno à central azul) ---
            float closeTimer = 0f;
            Vector3 startPosClosing1 = gatePart1.localPosition;
            Vector3 startPosClosing2 = gatePart2.localPosition;

            while (closeTimer < 1f)
            {
                gatePart1.localPosition = Vector3.Lerp(startPosClosing1, initialPos1, closeTimer);
                gatePart2.localPosition = Vector3.Lerp(startPosClosing2, initialPos2, closeTimer);
                closeTimer += Time.deltaTime * moveSpeed;
                yield return null;
            }
            gatePart1.localPosition = initialPos1;
            gatePart2.localPosition = initialPos2;
            // Após este movimento, os portões estão de volta à posição inicial (FECHADA - central azul)
            // e o ciclo vai recomeçar na FASE 1, onde o som será disparado.
        }
    }
}