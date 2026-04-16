using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Pause")]
    public bool isPaused = false;
    public GameObject img; // painel de pause

    [Header("References")]
    public PlayerMoviment playerMoviment;

    void Start()
    {
        // Garante que o jogo sempre começa despausado
        Resume();
    }

    void Update()
    {
        HandleKeyboardInputs();
    }

    // ===============================
    // INPUTS DO TECLADO
    // ===============================
    void HandleKeyboardInputs()
    {
        if (Keyboard.current == null) return;

        // ESC = Toggle Pause
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseResume();
        }

        // M = Menu principal
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ReturnToMainMenu();
        }
    }

    // ===============================
    // PAUSE SYSTEM
    // ===============================

    // ⭐ ESTA FUNÇÃO DEVE SER USADA PELO BOTÃO UI
    public void TogglePauseResume()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (img != null)
            img.SetActive(true);

        if (playerMoviment != null)
            playerMoviment.enabled = false;
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (img != null)
            img.SetActive(false);

        if (playerMoviment != null)
            playerMoviment.enabled = true;
    }

    // ===============================
    // MENU
    // ===============================

    // ⭐ ESTA FUNÇÃO É PARA O BOTÃO MENU
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // MUITO IMPORTANTE (evita bug ao trocar cena)
        SceneManager.LoadScene(0);
    }

    // ===============================
    // BOTÃO JUMP (caso use depois)
    // ===============================
    public void UiJumpButton()
    {
        if (playerMoviment != null)
        {
            playerMoviment.JumpMobile();
        }
    }
}