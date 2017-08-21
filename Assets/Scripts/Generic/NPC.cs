using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPC
{
    [SerializeField] private string m_Name;
    [SerializeField] private bool m_IsInteracting;
    [SerializeField] private GameObject gameObject;
    
    [SerializeField] private Dialogue m_Dialogue;
    
    public NPC()
    {
        m_Name = "Default_NPC";
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
    
    public Dialogue GetDialogue()
    {
        return m_Dialogue;
    }

}