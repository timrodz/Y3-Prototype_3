using System.Collections;
using System.Collections.Generic; // Dictionary
using UnityEngine;
using UnityEngine.Events; // UnityEvents

/// <summary>
/// Manages event handling through UnityEvents
/// 
/// Author: Juan Rodriguez
/// Date: 18/08/2017
/// </summary>
public class EventManager : MonoBehaviour
{	
	private static EventManager eventManager;
	
	public static EventManager Instance
	{
		get
		{
			if (!eventManager)
			{
				eventManager = FindObjectOfType (typeof(EventManager)) as EventManager;
				
				if (!eventManager)
				{
					Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
				}
				else
				{
					eventManager.Initialize();
				}
			}
			
			return eventManager;	
		}
	}
	
	// Subscribes, Unsubscribes messages
	private Dictionary<EventName, UnityEvent> m_EventDictionary = new Dictionary<EventName, UnityEvent>();
	
	private GameObject m_RegisteredSender;
	private EventName m_LastEvent;
	private Quest m_RegisteredQuest;
	
	// -------------------------------------------------------------------
	
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		Initialize();
	}
	
	private void Initialize()
	{
		if (null == m_EventDictionary)
		{
			m_EventDictionary = new Dictionary<EventName, UnityEvent>();
		}
	}
	
	/// <summary>
	/// Subscribes a listener to an event
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="listener"> The UnityAction that will be listened for</param>
	public static void StartListening(EventName eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		
		// If event name exists, populate 'thisEvent' with the event found
		if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
		{
			Debug.Log(">> Added listener " + listener.Method.Name + " to event: " + eventName.ToString());
			thisEvent.AddListener(listener);
		}
		// Event not found
		else
		{
			Debug.Log(">> Added listener " + listener.Method.Name + " to event: " + eventName.ToString());
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			Instance.m_EventDictionary.Add(eventName, thisEvent);
		}
		
	}
	
	/// <summary>
	/// Un-subscribes a listener from an event
	/// </summary>
	/// <param name="eventName"></param>
	/// <param name="listener"></param>
	public static void StopListening(EventName eventName, UnityAction listener)
	{
		if (null == eventManager) return;
		
		UnityEvent thisEvent = null;
		
		// If event name exists, populate 'thisEvent' with the event found
		if (Instance.m_EventDictionary.TryGetValue(eventName, out thisEvent))
		{
			Debug.Log("<< Removed listener " + listener.Method.Name + " from event: " + eventName.ToString());
			thisEvent.RemoveListener(listener);
		}
	}
	
	/// <summary>
	/// Triggers an event
	/// </summary>
	/// <param name="_event"> The event type</param>
	/// <param name="_shouldRegisterSender"> The object that is invoking the event</param>
	public static void Invoke(CustomEvent _event, bool _shouldRegisterSender)
	{
		UnityEvent thisEvent = null;
		
		if (Instance.m_EventDictionary.TryGetValue(_event.GetName(), out thisEvent))
		{
			if (_shouldRegisterSender)
			{
				Instance.m_RegisteredSender = _event.GetSender();
				Debug.Log("Sender set to " + Instance.m_RegisteredSender);
			}
			
			Debug.Log(_event.GetName() + " Invoked");
			thisEvent.Invoke();
			
			// Register this event as the last event
			Instance.m_LastEvent = _event.GetName();
		}
	}
	
	/// <summary>
	/// Triggers an event
	/// </summary>
	/// <param name="_event"> The event name</param>
	public static void Invoke(EventName _event)
	{
		UnityEvent thisEvent = null;
		
		if (Instance.m_EventDictionary.TryGetValue(_event, out thisEvent))
		{
			Debug.Log(_event + " Invoked");
			thisEvent.Invoke();
			
			// Register this event as the last event
			Instance.m_LastEvent = _event;
		}
	}
	
	public static void ResetSender()
	{
		Instance.m_RegisteredSender = null;
	}
	
	public static GameObject GetRegisteredSender()
	{
		return (Instance.m_RegisteredSender);
	}
	
	public static EventName GetLastEvent()
	{
		return (Instance.m_LastEvent);
	}
	
	public static void SetQuest(Quest sender)
	{
		Instance.m_RegisteredQuest = sender;
	}
	
	public static Quest GetQuest()
    {
        return (Instance.m_RegisteredQuest);
    }

} // EventManager