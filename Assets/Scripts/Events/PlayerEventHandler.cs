using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
    private CustomEvent m_Event;
	
	private bool m_CanInteract = true;
    private bool m_EnableInputForDialogue = false;
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
#if UNITY_STANDALONE
        if (m_EnableInputForDialogue && m_CanInteract && Input.GetKeyDown(KeyCode.E))
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
        if(other.GetComponent<EventDispatcher>() != null)
        {
            m_Event = new CustomEvent(EventName.OnPlayerTriggerEnter, other.gameObject);
            EventManager.Invoke(m_Event, true);
            m_EnableInputForDialogue = true;
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EventDispatcher>() != null)
        {
            // m_Event = new CustomEvent(EventName.OnPlayerTriggerExit, other.gameObject);
            EventManager.Invoke(EventName.OnPlayerTriggerExit);
            EventManager.ResetSender();
            m_EnableInputForDialogue = false;
        }

    }
}