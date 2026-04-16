using UnityEngine;
using System.Collections; // Necessário para usar as Coroutines

public class FishJump : MonoBehaviour
{
    public float jumpHeight = 1.5f; // Altura máxima do salto
    public float jumpUpDuration = 1.0f; // Tempo para subir
    public float jumpDownDuration = 1.0f; // Tempo para cair
    public float waitAtTopDuration = 0.5f; // Tempo para esperar no ponto mais alto
    public float waitAtBottomDuration = 0.5f; // Tempo para esperar no ponto mais baixo antes de pular novamente

    private Vector3 initialPosition; // Posição inicial do peixe
    private bool isJumping = false; // Flag para controlar se já está pulando

    void Start()
    {
        initialPosition = transform.position; // Guarda a posição inicial
        StartCoroutine(JumpRoutine()); // Inicia a rotina de salto
    }

    IEnumerator JumpRoutine()
    {
        while (true) // Loop infinito para repetir o salto
        {
            // Posição inicial (abaixo da água)
            Vector3 startPos = initialPosition;
            // Posição no topo do salto
            Vector3 endPos = initialPosition + Vector3.up * jumpHeight;

            // --- Subida ---
            float timer = 0f;
            while (timer < jumpUpDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, timer / jumpUpDuration);
                timer += Time.deltaTime;
                yield return null; // Espera até o próximo frame
            }
            transform.position = endPos; // Garante que atinja a posição final exata

            // --- Espera no topo ---
            yield return new WaitForSeconds(waitAtTopDuration);

            // --- Descida ---
            timer = 0f;
            while (timer < jumpDownDuration)
            {
                transform.position = Vector3.Lerp(endPos, startPos, timer / jumpDownDuration);
                timer += Time.deltaTime;
                yield return null; // Espera até o próximo frame
            }
            transform.position = startPos; // Garante que atinja a posição inicial exata

            // --- Espera na parte de baixo ---
            yield return new WaitForSeconds(waitAtBottomDuration);
        }
    }
}