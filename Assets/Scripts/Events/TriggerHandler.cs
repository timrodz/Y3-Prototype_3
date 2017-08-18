using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
	private CustomEvent m_Event;

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