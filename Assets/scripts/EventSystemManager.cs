using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    void Awake()
    {
        // Encuentra todos los sistemas de eventos en la escena
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        
        // Si hay más de un sistema de eventos, destruir los adicionales
        for (int i = 1; i < eventSystems.Length; i++)
        {
            Destroy(eventSystems[i].gameObject);
        }
    }
}