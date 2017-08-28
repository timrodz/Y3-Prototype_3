using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private bool m_CanInteract = true;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
#if UNITY_STANDALONE
        if (m_CanInteract && Input.GetKeyDown(KeyCode.E))
        {
            EventManager.Invoke(EventName.DialogueRequest);
        }
#endif
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        EventManager.Invoke(new CustomEvent(EventName.OnPlayerTriggerEnter, other.gameObject), true);
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {

        // m_Event = new CustomEvent(EventName.OnPlayerTriggerExit, other.gameObject);
        EventManager.Invoke(EventName.OnPlayerTriggerExit);
        EventManager.ResetSender();
    }

}