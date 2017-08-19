using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventHandler : MonoBehaviour
{
	private CustomEvent m_Event;
	
	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			m_Event = new CustomEvent(EventName.DialogueRequest);
			EventManager.Invoke(m_Event);
		}
	}

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
		
		m_Event = new CustomEvent(EventName.OnPlayerTriggerEnter, other.gameObject);
		EventManager.Invoke(m_Event, true);
    }
	
	/// <summary>
	/// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerExit(Collider other)
	{
		m_Event = new CustomEvent(EventName.OnPlayerTriggerExit, other.gameObject);
		EventManager.Invoke(m_Event, false);
		EventManager.ResetSender();
	}

}