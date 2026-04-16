using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Pega o componente Animator que está no mesmo objeto
        animator = GetComponent<Animator>();
    }

    // Executa quando algo entra na área do Box Collider (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", true);
        }
    }

    // Executa quando algo sai da área do Box Collider (Trigger)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("isOpen", false);
        }
    }
}