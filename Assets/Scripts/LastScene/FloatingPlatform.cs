using UnityEngine;
using System.Collections;

public class FloatingPlatform : MonoBehaviour
{
    public float floatHeight = 1f;        // A altura máxima que a plataforma vai flutuar acima da posição inicial
    public float floatSpeed = 1f;         // A velocidade com que a plataforma sobe e desce
    public float waitTimeAtTop = 5f;      // O tempo de espera no ponto mais alto

    private Vector3 startPosition;
    private Vector3 topPosition;
    private bool isWaiting = false;

    void Start()
    {
        startPosition = transform.position; // Armazena a posição inicial da plataforma
        topPosition = new Vector3(startPosition.x, startPosition.y + floatHeight, startPosition.z); // Calcula a posição mais alta
        StartCoroutine(FloatMovement()); // Inicia a corrotina de movimento
    }

    IEnumerator FloatMovement()
    {
        while (true) // Loop infinito para o movimento contínuo
        {
            // Subir
            float timer = 0f;
            Vector3 currentStart = transform.position;
            Vector3 currentTarget = topPosition;

            while (timer < 1f)
            {
                transform.position = Vector3.Lerp(currentStart, currentTarget, timer);
                timer += Time.deltaTime * floatSpeed; // Multiplicar por floatSpeed para controlar a velocidade
                yield return null;
            }
            transform.position = currentTarget; // Garante que a plataforma chegue exatamente ao topo

            // Esperar no topo
            yield return new WaitForSeconds(waitTimeAtTop);

            // Descer
            timer = 0f;
            currentStart = transform.position;
            currentTarget = startPosition;

            while (timer < 1f)
            {
                transform.position = Vector3.Lerp(currentStart, currentTarget, timer);
                timer += Time.deltaTime * floatSpeed; // Multiplicar por floatSpeed para controlar a velocidade
                yield return null;
            }
            transform.position = currentTarget; // Garante que a plataforma chegue exatamente à posição inicial
        }
    }
}