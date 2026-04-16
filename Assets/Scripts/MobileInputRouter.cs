using UnityEngine;
using UnityEngine.SceneManagement;

public class MobileInputRouter : MonoBehaviour
{
    private PlayerMoviment player;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ConectarPlayer();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConectarPlayer();
    }

    void ConectarPlayer()
    {
        player = FindFirstObjectByType<PlayerMoviment>();
    }

    public void FrenteDown()
    {
        if (player != null) player.MobileFrenteDown();
    }

    public void FrenteUp()
    {
        if (player != null) player.MobileFrenteUp();
    }

    public void DireitaDown()
    {
        if (player != null) player.MobileDireitaDown();
    }

    public void DireitaUp()
    {
        if (player != null) player.MobileDireitaUp();
    }

    public void EsquerdaDown()
    {
        if (player != null) player.MobileEsquerdaDown();
    }

    public void EsquerdaUp()
    {
        if (player != null) player.MobileEsquerdaUp();
    }

    public void TrasDown()
    {
        if (player != null) player.MobileTrasDown();
    }

    public void TrasUp()
    {
        if (player != null) player.MobileTrasUp();
    }

    public void Jump()
    {
        if (player != null) player.JumpMobile();
    }
}