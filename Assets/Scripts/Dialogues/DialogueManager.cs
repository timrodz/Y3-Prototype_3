using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private bool m_HasDialogueStarted;
    [SerializeField] private bool m_IsProcessingText;

    [Space]
    [Header("Visual Fields")]
    [SerializeField] private CanvasGroup m_Transparency;
    [SerializeField] private Image m_DialogueFieldImage;
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private TMPro.TextMeshProUGUI m_TextField;
    [SerializeField] private RectTransform m_NextFieldCursor;
    private Vector3 m_NextFieldCursorOriginalPosition;

    [Space]
    [SerializeField] private Sprite m_TextBoxNoBerry;
    [SerializeField] private Sprite m_TextBoxBerry;

    [Space]
    [Header("Current Dialogue NPC")]
    [SerializeField] private GameObject m_Sender;
    [SerializeField] private NPC m_DialogueNPC;
    [SerializeField] private CustomEvent m_Event;

    private string m_CurrentSentence;
    private Queue<string> m_DialogueQueue = new Queue<string>();

    Tween cursorTween;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_Transparency.alpha = 0;
        m_TextField.text = "";
        m_NextFieldCursorOriginalPosition = m_NextFieldCursor.localPosition;
        m_NextFieldCursor.localScale = Vector3.zero;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening(EventName.DialogueRequest, ProcessDialogue);
        EventManager.StartListening(EventName.DialogueStart, StartDialogue);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.DialogueRequest, ProcessDialogue);
        EventManager.StopListening(EventName.DialogueStart, StartDialogue);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (m_IsProcessingText)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log(">> Pressed E while processing text");
                StopAllCoroutines();
                ShowNextFieldCursor(m_CurrentSentence);
            }
        }
    }

    private void ProcessDialogue()
    {
        // Dialogue is currently being shown
        if (m_IsProcessingText)
        {
            Debug.Log(">> Processing text - Can't process dialogue at this time.");
            return;
        }

        Debug.Log("==== Processing Dialogue ====");
        Debug.Log(">> Has dialogue started: " + m_HasDialogueStarted);

        // If the dialogue has not started yet
        if (!m_HasDialogueStarted)
        {
            m_DialogueNPC = SetupDialogueNPC();

            if (null == m_DialogueNPC)
            {
                Debug.LogWarning(">> Could not set-up dialogue");
                return;
            }

            Debug.Log(">> Checking if the NPC has assigned quests");
            
            // TODO: Make the player's inventory receive the message and then 
            // invoke a dialogue start message when it's done checking
            // EventManager.Invoke(EventName.QuestCheck);
            
            // For now: Directly start the dialogue
            EventManager.Invoke(EventName.DialogueStart);

            return;
        }

        ProcessTextField();

    }

    private void StartDialogue()
    {

        // NPC is setup, begin dialogue conversation
        m_DialogueNPC.AllowInteraction(true);
        
        // Fill the name field with the NPC's name
        m_NameField.text = m_DialogueNPC.GetName();

        Debug.Log(">> Starting conversation with: " + m_DialogueNPC.GetName());

        // Populate sentence queue
        m_DialogueQueue.Clear();

        foreach(string sentence in m_DialogueNPC.GetDialogue().GetSentences())
        {
            m_DialogueQueue.Enqueue(sentence);
        }

        m_DialogueFieldImage.sprite = m_TextBoxNoBerry;

        // Set a random image (Berry or no berry)
        int random = Random.Range(0, 2);
        if (random == 0)
        {
            m_DialogueFieldImage.sprite = m_TextBoxBerry;
        }

        m_Transparency.DOFade(1, 0.5f);

        m_DialogueFieldImage.rectTransform.DOMoveY(m_DialogueFieldImage.rectTransform.localPosition.y - 60, 0.5f).From().SetEase(Ease.OutBack);
        
        m_HasDialogueStarted = true;

        ProcessTextField();

    }

    private void EndDialogue()
    {
        Debug.Log(">> Ending dialogue");

        m_HasDialogueStarted = false;
        m_IsProcessingText = false;
        m_Transparency.DOFade(0, 0.25f);

        EventManager.Invoke(EventName.DialogueEnd);

        TryToAssignQuest();
    }

    private void TryToAssignQuest()
    {
        Quest quest = m_DialogueNPC.GetMostRecentQuest();

        // Got most recent quest from NPC
        if (null != quest)
        {
            quest.SetState(QuestState.Started);

            EventManager.Invoke(EventName.QuestStart);

            QuestManager.AddQuest(quest);
        }
    }

    private void ProcessTextField()
    {
        Debug.Log(">> Processing text field");
        // Empty the text field
        m_TextField.text = "";

        // If there are no more sentences left, end the dialogue
        if (m_DialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // Begin processing the text
        m_IsProcessingText = true;

        m_CurrentSentence = m_DialogueQueue.Dequeue();

        StartCoroutine(AnimateText(m_CurrentSentence, 0.5f, 0.2f, 0.01f));
    }

    /// <summary>
    /// Animates the text in a typewriter effect
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="initialDelay"></param>
    /// <param name="returnKeyDelay"></param>
    /// <param name="typingSpeed"></param>
    /// <returns></returns>
    private IEnumerator AnimateText(string sentence, float initialDelay, float returnKeyDelay, float typingSpeed)
    {
        // Hide the cursor
        m_NextFieldCursor.DOScale(Vector3.zero, 0.1f);
        cursorTween.SetLoops(0);
        cursorTween.Complete();
        cursorTween.Kill();

        yield return new WaitForSeconds(initialDelay);

        string finalText = "";

        for (int i = 0; i < sentence.Length; i++)
        {
            if (!m_IsProcessingText)
            {
                Debug.Log(">> Skipped text animation");
                break;
            }

            if (sentence[i] == '\n' || sentence[i] == ',')
            {
                yield return new WaitForSeconds(returnKeyDelay);
            }

            else
            {
                finalText += sentence[i];
            }

            m_TextField.text = finalText;
            yield return new WaitForSeconds(typingSpeed);
        }

        Debug.Log(">> Finished animating text - Showing next field cursor");
        ShowNextFieldCursor(sentence);

    }

    /// <summary>
    /// Show the next field cursor - Called whenever the text animation finishes
    /// </summary>
    private void ShowNextFieldCursor(string currentTextField)
    {
        Debug.Log(">> Showing next field cursor");

        m_TextField.text = currentTextField;

        m_NextFieldCursor.localPosition = m_NextFieldCursorOriginalPosition;

        // Show the cursor once the dialogue has finished
        m_NextFieldCursor.DOScale(Vector3.one * 0.25f, 0.2f).OnComplete(() =>
        {
            m_IsProcessingText = false;
            cursorTween = m_NextFieldCursor.DOLocalMoveY(m_NextFieldCursorOriginalPosition.y - 5, 0.3f).SetAutoKill(false).SetLoops(-1, LoopType.Yoyo);
        });
    }

    private NPC SetupDialogueNPC()
    {
        m_Sender = EventManager.GetRegisteredSender();

        // No sender
        if (null == m_Sender)
        {
            Debug.Log(">> No sender set");
            return null;
        }

        Debug.Log("===== ATTEMPTING TO ACQUIRE NPC INFORMATION ====");

        DialogueInteractionHandler dih = m_Sender.GetComponent<DialogueInteractionHandler>();

        // Check if the object has an interaction handler
        if (null == dih)
        {
            Debug.LogWarning(">> Dialogue interaction handler doesn't exist");
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

        NPC npc = dih.GetNPC();
        npc.SetObject(dih.transform.parent.gameObject);

        Debug.Log("===== ACQUIRED NPC INFORMATION ====");
        Debug.Log("NPC Name: " + npc.GetName());

        return npc;
    }

}