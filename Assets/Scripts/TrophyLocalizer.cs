using UnityEngine;

public class TrophyLocalizer : MonoBehaviour
{
    void Start()
    {
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.RegistrarTrofeu(this.gameObject);
        }
    }
}