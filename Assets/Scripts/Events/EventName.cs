using UnityEngine;

public enum EventName
{
    NONE,
    OnPlayerTriggerEnter,
    OnPlayerTriggerExit,
	DialogueRequest,
	DialogueStart,
	DialogueEnd,
	QuestStart,
	QuestFail,
	QuestComplete,
    QuestCheck,
    QuestIncomplete,
    QuestCheckAll,
    QuestCompleteAll,
    QuestFailAll,
	PlayerJump,
    PlayerItemPickup
}

public struct CustomEvent
{
    private EventName name;
    private GameObject sender;
	
	public CustomEvent(EventName Name)
    {
        name = Name;
        sender = null;
    }

    public CustomEvent(EventName Name, GameObject sender)
    {
        name = Name;
        this.sender = sender;
    }
	
	public void SetName(EventName name)
	{
		this.name = name;
	}

    public EventName GetName()
    {
        return name;
    }
	
	public void SetSender(GameObject sender)
	{
		this.sender = sender;
	}
	
	public GameObject GetSender()
    {
        return sender;
    }
	
}