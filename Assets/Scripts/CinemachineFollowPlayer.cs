using UnityEngine;
using Unity.Cinemachine;

public class CinemachineFollowPlayer : MonoBehaviour
{
    private CinemachineCamera cineCam;

    void Awake()
    {
        cineCam = GetComponent<CinemachineCamera>();

        if (cineCam == null)
        {
            Debug.LogError("Componente CinemachineCamera não encontrado neste GameObject.");
            enabled = false;
        }
    }

    void OnEnable()
    {
        Invoke(nameof(SetPlayerTarget), 0.1f);
    }

    void SetPlayerTarget()
    {
        if (cineCam == null)
            return;

        // Alterado de FindObjectOfType para FindAnyObjectByType
        PlayerManager player = FindAnyObjectByType<PlayerManager>();

        if (player != null)
        {
            cineCam.Follow = player.transform;
            cineCam.LookAt = player.transform;

            Debug.Log("Câmera configurada para seguir o Player.");
        }
        else
        {
            Invoke(nameof(SetPlayerTarget), 0.5f);
        }
    }
}