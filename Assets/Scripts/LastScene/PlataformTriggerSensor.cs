using UnityEngine;

public class PlatformTriggerSensor : MonoBehaviour
{
    // Referência à plataforma gravitacional que este sensor controlará
    public GravitationalPlatform targetPlatform;

    // Tag que identifica o avatar que pode ativar o sensor
    public string avatarTag = "Player"; // Ou a tag que você usa para seu avatar

    private bool platformCycleStarted = false;

    void Start()
    {
        // Garante que o objeto tem um Collider e que ele é um trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("PlatformTriggerSensor requires a Collider component!", this);
            enabled = false;
            return;
        }
        if (!col.isTrigger)
        {
            Debug.LogWarning("Collider on PlatformTriggerSensor is not set to Is Trigger. Setting it now.", this);
            col.isTrigger = true;
        }

        // Garante que a plataforma alvo foi atribuída
        if (targetPlatform == null)
        {
            Debug.LogError("Target Platform is not assigned to PlatformTriggerSensor!", this);
            enabled = false;
        }
        else
        {
            // Inicialmente, paramos o ciclo da plataforma, pois ela só deve começar com o trigger
            // A plataforma deve começar o ciclo no Awake, mas vamos pará-lo imediatamente.
            // Para isso, faremos uma pequena modificação no GravitationalPlatform.
            targetPlatform.StopPlatformCycle(); // Novo método que vamos adicionar ao GravitationalPlatform
            platformCycleStarted = false;
        }
    }

    // Chamado quando um Collider entra no trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(avatarTag))
        {
            Debug.Log($"Avatar ({other.name}) entrou na área do sensor. Ativando plataforma.", this);
            if (targetPlatform != null && !platformCycleStarted)
            {
                targetPlatform.StartPlatformCycle(); // Novo método
                platformCycleStarted = true;
            }
        }
    }

    // Chamado quando um Collider sai do trigger
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(avatarTag))
        {
            Debug.Log($"Avatar ({other.name}) saiu da área do sensor. Desativando plataforma.", this);
            if (targetPlatform != null && platformCycleStarted)
            {
                targetPlatform.StopPlatformCycle(); // Novo método
                platformCycleStarted = false;
            }
        }
    }
}