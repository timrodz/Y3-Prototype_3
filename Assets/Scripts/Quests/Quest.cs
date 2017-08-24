using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum QuestState
{
	NotStarted,
	Started,
	Failed,
	Complete
}

[System.Serializable]
public class Quest
{
	[SerializeField] private string m_Name;
	[SerializeField] [EnumFlagsAttribute] private ItemHasg m_Item;
	[SerializeField] private int m_ItemQuantity;
	[SerializeField] private QuestState m_State;
	[SerializeField] private Dialogue dialogue;
	
	public Quest()
	{
		m_Name = "Default quest";
		m_Item = ItemHasg.None;
		m_ItemQuantity = 0;
	}
	
	public void GetInfo()
	{
		Debug.LogFormat("QUEST INFORMATION: Name {0}: Items required: {1} - Quantity: {2} - State: {3}", this.m_Name, this.m_Item, this.m_ItemQuantity, this.m_State);
	}
	
	public string GetName()
	{
		return m_Name;
	}
	
	public ItemHasg GetItem()
	{
		return m_Item;
	}
	
	public int GetItemQuantity()
	{
		return m_ItemQuantity;
	}
	
	public void SetState(QuestState state)
	{
		m_State = state;
	}
	
	public QuestState GetState()
	{
		return m_State;
	}
	
}