using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GravitationalPlatform : MonoBehaviour
{
    public float shrinkScale = 0.1f;
    public float shrinkDuration = 0.5f;
    public float expandDuration = 0.5f;
    public float waitAtShrunk = 0.5f;
    public float waitAtExpanded = 1f;

    public float moveHeight = 1f;
    public float moveSpeed = 1f;
    public float expandForce = 500f;
    public float forceRadius = 2f;

    public AudioClip expandSound;
    private AudioSource audioSource;

    private Rigidbody platformRigidbody;
    private Vector3 originalLocalScale;
    private Vector3 originalPosition;
    private bool isExpanding = false;
    // private bool cycleRunning = false; // Não precisamos mais desta flag externa

    // NOVO: Referência ao Coroutine para poder pará-lo
    private Coroutine platformCycleCoroutine;

    void Awake()
    {
        platformRigidbody = GetComponent<Rigidbody>();
        if (platformRigidbody == null)
        {
            Debug.LogError("GravitationalPlatform requires a Rigidbody component!");
            enabled = false;
            return;
        }

        platformRigidbody.isKinematic = true;

        originalLocalScale = transform.localScale;
        originalPosition = transform.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // REMOVIDO: StartCoroutine(PlatformCycle());
        // O ciclo agora será iniciado pelo sensor.
    }

    // NOVO: Método público para iniciar o ciclo da plataforma
    public void StartPlatformCycle()
    {
        // Se o ciclo já está rodando, não faça nada
        if (platformCycleCoroutine != null) return;

        Debug.Log("Iniciando ciclo da plataforma.", this);
        platformCycleCoroutine = StartCoroutine(PlatformCycleInternal());
    }

    // NOVO: Método público para parar o ciclo da plataforma
    public void StopPlatformCycle()
    {
        if (platformCycleCoroutine != null)
        {
            Debug.Log("Parando ciclo da plataforma.", this);
            StopCoroutine(platformCycleCoroutine);
            platformCycleCoroutine = null; // Reseta a referência para permitir um novo início

            // Opcional: Resetar a plataforma para a posição e escala originais ao parar
            // Isso evita que ela fique em um estado intermediário
            platformRigidbody.transform.localScale = originalLocalScale;
            platformRigidbody.MovePosition(originalPosition);
        }
    }

    // NOVO: O método real do ciclo agora é privado e chamado pelo StartPlatformCycle
    IEnumerator PlatformCycleInternal()
    {
        while (true) // Loop infinito para o ciclo
        {
            // 1. Descer e Encolher
            if (transform.position.y > originalPosition.y)
            {
                yield return StartCoroutine(MovePlatform(originalPosition, moveSpeed));
            }

            // Encolher
            yield return StartCoroutine(ChangeScale(new Vector3(originalLocalScale.x, shrinkScale, originalLocalScale.z), shrinkDuration));
            yield return new WaitForSeconds(waitAtShrunk);

            // 2. Expandir
            isExpanding = true;
            if (expandSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(expandSound);
            }
            yield return StartCoroutine(ChangeScale(originalLocalScale, expandDuration));
            isExpanding = false;

            // 3. Subir para a altura desejada
            yield return StartCoroutine(MovePlatform(originalPosition + Vector3.up * moveHeight, moveSpeed));

            // 4. Aplicar força no topo
            ApplyForceToAvatars();

            yield return new WaitForSeconds(waitAtExpanded);

            // 5. Descer de volta para a posição original para o próximo ciclo
            yield return StartCoroutine(MovePlatform(originalPosition, moveSpeed));
        }
    }

    IEnumerator ChangeScale(Vector3 targetScale, float duration)
    {
        Vector3 initialScale = transform.localScale;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            platformRigidbody.transform.localScale = Vector3.Lerp(initialScale, targetScale, timer / duration);
            yield return null;
        }
        platformRigidbody.transform.localScale = targetScale;
    }

    IEnumerator MovePlatform(Vector3 targetPos, float speed)
    {
        Vector3 initialPos = platformRigidbody.position;
        float distance = Vector3.Distance(initialPos, targetPos);
        if (distance == 0) yield break;

        float duration = distance / speed;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            platformRigidbody.MovePosition(Vector3.Lerp(initialPos, targetPos, timer / duration));
            yield return null;
        }
        platformRigidbody.MovePosition(targetPos);
    }

    void ApplyForceToAvatars()
    {
        Debug.Log("Tentando aplicar força.");
        Collider[] hitColliders = Physics.OverlapSphere(platformRigidbody.position, forceRadius);

        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                if (rb != null && rb != platformRigidbody)
                {
                    rb.AddForce(Vector3.up * expandForce, ForceMode.Impulse);
                    Debug.Log($"Força aplicada a: {hitCollider.name}");
                }
            }
        }
        else
        {
            Debug.Log("Nenhum Rigidbody encontrado no raio de força.");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, forceRadius);

        Gizmos.color = Color.blue;
        if (Application.isPlaying)
        {
            Gizmos.DrawLine(originalPosition, originalPosition + Vector3.up * moveHeight);
            Gizmos.DrawWireSphere(originalPosition + Vector3.up * moveHeight, 0.2f);
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * moveHeight);
            Gizmos.DrawWireSphere(transform.position + Vector3.up * moveHeight, 0.2f);
        }
    }
}