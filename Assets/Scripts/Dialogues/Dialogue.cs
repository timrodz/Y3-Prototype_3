using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [TextArea(1, 4)]
	[SerializeField] private string[] m_Sentences;
    
    public string[] GetSentences()
    {
        return m_Sentences;
    }
    
}