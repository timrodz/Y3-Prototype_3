using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	private GameObject m_Sender;
	private NPC m_DialogueNPC;
	private CustomEvent m_Event;

    // Use this for initialization
    void Start()
    {

    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening(EventName.DialogueRequest, ShowDialogue);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.DialogueRequest, ShowDialogue);
    }

    private void ShowDialogue()
    {
		m_DialogueNPC = SetupDialogueNPC();
		
		if (null == m_DialogueNPC)
		{
			Debug.LogWarning(">> Could not set-up dialogue");
			return;
		}
		
		m_Event = new CustomEvent(EventName.DialogueOngoing, m_DialogueNPC.GetObject());
		
		// NPC is setup, begin dialogue conversation
		m_DialogueNPC.AllowInteraction(true);

    }
	
	private NPC SetupDialogueNPC()
	{
		Debug.Log("===== ATTEMPTING TO ACQUIRE NPC INFORMATION ====");
		
		m_Sender = EventManager.GetRegisteredSender();
		
		// No sender
		if (null == m_Sender)
		{
			Debug.Log(">> No sender set");
			return null;
		}
		
        DialogueInteractionHandler dih = m_Sender.GetComponent<DialogueInteractionHandler>();

        // Check if the object has an interaction handler
        if (null == dih)
        {
            Debug.LogWarning(">> Dialogue interaction handler doesn't have an NPC");
            return null;
        }

        // Check if the object is interactable
        if (!dih.IsInteractable())
        {
			Debug.Log(">> Object is not interactable");
            return null;
        }
		
		// Check if the object can be interacted with
        if (!dih.IsInteractable())
        {
			Debug.Log(">> Object cannot be interacted with right now.");
            return null;
        }
		
		NPC npc = dih.GetNPCInformation();
		
		Debug.Log("===== ACQUIRED NPC INFORMATION ====");
		Debug.Log("NPC Name: " + npc.GetName());
		
		return npc;
	}
	
}