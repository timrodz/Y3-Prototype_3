using UnityEngine;

[System.Flags]
public enum EventName
{
    NONE = (1 << 0),
    OnPlayerTriggerEnter = (1 << 1),
    OnPlayerTriggerExit = (1 << 2),
	DialogueRequest = (1 << 3),
	DialogueStart = (1 << 4),
	DialogueEnd = (1 << 5),
	QuestStart = (1 << 6),
	QuestFail = (1 << 7),
	QuestComplete = (1 << 8),
    QuestCheck = (1 << 9),
    QuestIncomplete = (1 << 10),
	PlayerJump = (1 << 11),
    PlayerItemPickup = (1 << 12)
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