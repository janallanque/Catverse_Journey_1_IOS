using UnityEngine;
using UnityEngine.EventSystems;

public class FixEventSystem : MonoBehaviour
{
    void Awake()
    {
        EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (systems.Length > 1)
        {
            Destroy(gameObject);
        }
    }
}