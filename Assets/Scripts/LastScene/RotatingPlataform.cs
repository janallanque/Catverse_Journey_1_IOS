using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed = 90f; // Velocidade de rotação em graus por segundo
    public float horizontalWaitTime = 10f; // Tempo de espera na posição inicial (horizontal)

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        StartCoroutine(RotateAndPause());
    }

    IEnumerator RotateAndPause()
    {
        while (true)
        {
            // Rotacionar 360 graus em torno do eixo X (vertical)
            float currentRotation = 0f;
            while (currentRotation < 360f)
            {
                float rotationStep = rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.right, rotationStep); // Mudado para Vector3.right
                currentRotation += rotationStep;
                yield return null;
            }

            // Garantir que esteja exatamente na rotação inicial (horizontal)
            transform.rotation = initialRotation;

            // Esperar na horizontal
            yield return new WaitForSeconds(horizontalWaitTime);
        }
    }
}