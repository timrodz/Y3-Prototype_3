using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance;

    public static QuestManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof (QuestManager)) as QuestManager;

                if (!_instance)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
            }

            return _instance;
        }
    }

    [Header("Visual Fields")]
    [SerializeField] private RectTransform m_QuestPanel;
    [SerializeField] private CanvasGroup m_Transparency;
    [SerializeField] private TMPro.TextMeshProUGUI m_QuestTextField;

    [Space]
    [Header("Quest information")]
    [SerializeField] private List<Quest> m_QuestList = new List<Quest>();
    [SerializeField] private Quest m_CurrentQuest;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (m_QuestList.Count <= 0)
        {
            m_Transparency.alpha = 0;
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening(EventName.PlayerJump, RespondToPlayerJump);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.PlayerJump, RespondToPlayerJump);
    }

    private void ShowQuestPanel()
    {
        if (m_Transparency.alpha == 1)
        {
            return;
        }
        m_Transparency.DOFade(1, 0.5f);
    }

    private void HideQuestPanel()
    {
        m_Transparency.DOFade(0, 0.25f);
    }

    private void RespondToPlayerJump()
    {
        if (m_Transparency.alpha == 0)
        {
            return;
        }

        m_QuestPanel.DOLocalMoveY(m_QuestPanel.localPosition.y - 5, 0.35f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            m_QuestPanel.DOLocalMoveY(m_QuestPanel.localPosition.y + 5, 0.25f);
        });
    }

    public static void SetCurrentQuest(Quest quest)
    {
        Debug.Log("==== Set current quest ====");
        
        QuestManager.Instance.m_CurrentQuest = quest;
        
        QuestManager.Instance.m_CurrentQuest.GetInfo();
        QuestManager.Instance.m_QuestTextField.text += QuestManager.Instance.m_CurrentQuest.GetName();
        
        QuestManager.Instance.ShowQuestPanel();
    }

    public static void AddQuest(Quest quest)
    {
        QuestManager.Instance.m_QuestList.Add(quest);
        Debug.Log("==== Added quest ====");
        quest.GetInfo();
    }

    public static QuestState CheckQuest(Quest quest)
    {
        int index = QuestManager.Instance.m_QuestList.IndexOf(quest);

        if (index != -1)
        {
            Debug.Log("==== Checked quest ====");
            quest.GetInfo();
            return (QuestManager.Instance.m_QuestList[index].GetState());
        }

        return QuestState.NotStarted;
    }

    public static void FailQuest(Quest quest)
    {
        int index = QuestManager.Instance.m_QuestList.IndexOf(quest);
        if (index != -1)
        {
            Debug.Log("==== Failed quest ====");
            QuestManager.Instance.m_QuestList[index].SetState(QuestState.Failed);
            quest.GetInfo();
            EventManager.SetQuest(QuestManager.Instance.m_QuestList[index]);
        }
    }

    public static void CompleteQuest(Quest quest)
    {
        int index = QuestManager.Instance.m_QuestList.IndexOf(quest);
        if (index != -1)
        {
            Debug.Log("==== Completed quest quest ====");
            QuestManager.Instance.m_QuestList[index].SetState(QuestState.Complete);
            quest.GetInfo();
            EventManager.SetQuest(QuestManager.Instance.m_QuestList[index]);
            EventManager.Invoke(EventName.QuestComplete);
        }
        
        // Hide the quest panel and empty the quest text 
        // field if there are no more quests
        if (QuestManager.Instance.m_QuestList.Count <= 0)
        {
            QuestManager.Instance.m_QuestTextField.text = "";
            QuestManager.Instance.HideQuestPanel();
        }
    }

}