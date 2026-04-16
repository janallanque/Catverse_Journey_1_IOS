using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public void StartGame()
    { 
        SceneManager.LoadScene(1);
    }

    public void InfoCena()
    {
        SceneManager.LoadScene(4);
    }

    public void ExitGame()
    {
#if     UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif   UNITY_WEBGL
            Application.OpenURL("about:blank");
#else
            Application.Quit();
#endif

    }
}
