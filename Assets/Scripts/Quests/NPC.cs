using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC
{
    [SerializeField] private string m_Name;
    [SerializeField] private bool m_IsRecruited;
    [SerializeField] private bool m_IsInteracting;
    [SerializeField] private GameObject gameObject;
    
    [SerializeField] private Dialogue m_Dialogue;
    
    // [SerializeField] private List<Quest> m_Quests = new List<Quest>();
    [SerializeField] private Quest m_Quest;
    
    public NPC()
    {
        m_Name = null;
        gameObject = null;
    }
    
    public NPC(string _name, GameObject _object)
    {
        m_Name = _name;
        gameObject = _object;
    }
    
    public string GetName()
    {
        return m_Name;
    }
    
    public void SetObject(GameObject _object)
    {
        gameObject = _object;
    }
    
    public GameObject GetObject()
    {
        return gameObject;
    }
    
    public void AllowInteraction(bool _isInteracting)
    {
        m_IsInteracting = _isInteracting;
    }
    
    public bool IsInteracting()
    {
        return m_IsInteracting;
    }

    public void Recruit()
    {
        m_IsRecruited = true;
    }

    public bool IsRecruited()
    {
        return m_IsRecruited;
    }
    
    public void SetDialogue(Dialogue dialogue)
    {
        m_Dialogue = dialogue;
    }
    
    public Dialogue GetDialogue()
    {
        return m_Dialogue;
    }
    
    public Quest GetMostRecentQuest()
    {
        // if (m_Quests.Count > 0)
        // {
        //     return (m_Quests[0]);
        // }
        // return null;

        return m_Quest;
    }

    public void CompleteMostRecentQuest()
    {
        // if (m_Quests.Count > 0)
        // {
        //     m_Quests[0].SetState(QuestState.Complete);
        //     m_IsRecruited = true;
        //     m_Dialogue = m_Quests[0].GetDialogue();
        //     QuestManager.CompleteQuest(m_Quests[0]);
        // }

        m_Quest.SetState(QuestState.Complete);
        m_IsRecruited = true;
        m_Dialogue = m_Quest.GetDialogue();
        QuestManager.CompleteQuest(m_Quest);
    }

}