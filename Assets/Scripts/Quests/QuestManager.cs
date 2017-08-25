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
    [SerializeField] private List<NPC> m_NPCList = new List<NPC>();
    [SerializeField] private List<Quest> m_QuestList = new List<Quest>();
    [SerializeField] private List<Quest> m_CompletedQuestList = new List<Quest>();
    [SerializeField] private List<Quest> m_FailedQuestList = new List<Quest>();
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
        EventManager.StartListening(EventName.QuestCheck, CompleteCurrentQuest);
        EventManager.StartListening(EventName.QuestCheckAll, CheckAllQuests);
        EventManager.StartListening(EventName.QuestCompleteAll, CompleteAllQuests);
        EventManager.StartListening(EventName.QuestFailAll, FailAllQuests);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.PlayerJump, RespondToPlayerJump);
        EventManager.StopListening(EventName.QuestCheck, CompleteCurrentQuest);
        EventManager.StopListening(EventName.QuestCheckAll, CheckAllQuests);
        EventManager.StopListening(EventName.QuestCompleteAll, CompleteAllQuests);
        EventManager.StopListening(EventName.QuestFailAll, FailAllQuests);
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

        m_QuestPanel.DOLocalMoveY(m_QuestPanel.localPosition.y - 5, 0.35f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            m_QuestPanel.DOLocalMoveY(m_QuestPanel.localPosition.y + 5, 0.25f);
        });
    }

    public static void SetCurrentQuest(Quest quest)
    {
        Debug.Log("==== Set current quest ====");

        QuestManager.Instance.m_CurrentQuest = quest;

        QuestManager.Instance.m_CurrentQuest.GetInfo();
        QuestManager.Instance.m_QuestTextField.text = QuestManager.Instance.m_CurrentQuest.GetName();

        QuestManager.Instance.ShowQuestPanel();
    }

    public static void AddQuest(Quest quest)
    {
        QuestManager.Instance.m_QuestList.Add(quest);

        Debug.Log("==== Added quest ====");

        quest.GetInfo();
    }

    public static void AddNPC(NPC npc)
    {
        int index = QuestManager.Instance.m_NPCList.IndexOf(npc);
        if (index == -1)
        {
            QuestManager.Instance.m_NPCList.Add(npc);
            Debug.Log("==== Added NPC ====");

            Debug.Log(">> " + npc.GetName());
        }

        QuestManager.AddQuest(QuestManager.Instance.m_NPCList[index].GetMostRecentQuest());
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

            EventManager.SetCurrentQuest(QuestManager.Instance.m_QuestList[index]);

            EventManager.Invoke(EventName.QuestFail);

            QuestManager.Instance.m_FailedQuestList.Add(QuestManager.Instance.m_QuestList[index]);

            QuestManager.Instance.m_QuestList.Remove(QuestManager.Instance.m_QuestList[index]);
        }

        QuestManager.DeterminePanelVisibility();
    }

    public static void CompleteQuest(Quest quest)
    {
        int index = QuestManager.Instance.m_QuestList.IndexOf(quest);
        if (index != -1)
        {
            Debug.Log("==== Completed quest ====");

            QuestManager.Instance.m_QuestList[index].SetState(QuestState.Complete);

            quest.GetInfo();

            EventManager.SetCurrentQuest(QuestManager.Instance.m_QuestList[index]);

            EventManager.Invoke(EventName.QuestComplete);

            QuestManager.Instance.m_CompletedQuestList.Add(QuestManager.Instance.m_QuestList[index]);

            QuestManager.Instance.m_QuestList.Remove(QuestManager.Instance.m_QuestList[index]);
        }

        QuestManager.DeterminePanelVisibility();
    }

    public static void CompleteCurrentQuest()
    {
        int index = QuestManager.Instance.m_QuestList.IndexOf(QuestManager.Instance.m_CurrentQuest);

        if (index != -1)
        {
            Debug.Log("==== Completed current quest ====");

            QuestManager.Instance.m_QuestList[index].SetState(QuestState.Complete);

            QuestManager.Instance.m_CurrentQuest.GetInfo();

            EventManager.SetCurrentQuest(QuestManager.Instance.m_QuestList[index]);

            EventManager.Invoke(EventName.QuestComplete);

            QuestManager.Instance.m_CompletedQuestList.Add(QuestManager.Instance.m_QuestList[index]);

            QuestManager.Instance.m_QuestList.Remove(QuestManager.Instance.m_QuestList[index]);
        }

        QuestManager.DeterminePanelVisibility();
    }

    public static void CheckAllQuests()
    {
        Debug.Log("==== STARTED QUESTS ====");
        foreach(Quest q in QuestManager.Instance.m_QuestList)
        {
            q.GetInfo();
        }
        Debug.Log("==== COMPLETED QUESTS ====");
        foreach(Quest q in QuestManager.Instance.m_CompletedQuestList)
        {
            q.GetInfo();
        }
        Debug.Log("==== FAILED QUESTS ====");
        foreach(Quest q in QuestManager.Instance.m_FailedQuestList)
        {
            q.GetInfo();
        }
    }

    public static void CompleteAllQuests()
    {
        Debug.Log("==== Completing all quests ====");

        for (int index = 0; index < QuestManager.Instance.m_QuestList.Count; index++)
        {
            QuestManager.Instance.m_QuestList[index].SetState(QuestState.Complete);

            QuestManager.Instance.m_CurrentQuest.GetInfo();

            EventManager.SetCurrentQuest(QuestManager.Instance.m_QuestList[index]);

            EventManager.Invoke(EventName.QuestComplete);

            QuestManager.Instance.m_CompletedQuestList.Add(QuestManager.Instance.m_QuestList[index]);

            QuestManager.Instance.m_QuestList.Remove(QuestManager.Instance.m_QuestList[index]);
        }

        QuestManager.DeterminePanelVisibility();
    }

    public static void FailAllQuests()
    {
        foreach(Quest q in QuestManager.Instance.m_QuestList)
        {
            q.GetInfo();
            q.SetState(QuestState.Failed);
            QuestManager.Instance.m_CompletedQuestList.Add(q);
            QuestManager.Instance.m_QuestList.Remove(q);

        }
        QuestManager.DeterminePanelVisibility();
    }

    private static void DeterminePanelVisibility()
    {
        if (QuestManager.Instance.m_QuestList.Count <= 0)
        {
            Debug.Log(">> Hiding quest panel - No more active quests");
            QuestManager.Instance.m_Transparency.DOFade(0, 0.2f);
        }
    }

}