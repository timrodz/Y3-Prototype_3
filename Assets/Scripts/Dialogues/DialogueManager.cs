using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private bool m_HasDialogueStarted;
    [SerializeField] private bool m_IsProcessingText;

    [Space]
    [Header("Visual Fields")]
    [SerializeField] private CanvasGroup m_Transparency;
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private TMPro.TextMeshProUGUI m_TextField;
    [SerializeField] private RectTransform m_NextFieldCursor;

    [Space]
    [Header("Current Dialogue NPC")]
    [SerializeField] private GameObject m_Sender;
    [SerializeField] private NPC m_DialogueNPC;
    [SerializeField] private CustomEvent m_Event;

    private string m_CurrentSentence;
    private Queue<string> m_DialogueQueue = new Queue<string>();

    Tween t;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_Transparency.alpha = 0;
        m_TextField.text = "";
        m_NextFieldCursor.localScale = Vector3.zero;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening(EventName.DialogueRequest, ProcessDialogue);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.DialogueRequest, ProcessDialogue);
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

            // Begin the dialogue
            EventManager.Invoke(EventName.DialogueStart);

            // NPC is setup, begin dialogue conversation
            m_DialogueNPC.AllowInteraction(true);

            ShowDialoguePanel();

            m_HasDialogueStarted = true;

            return;
        }

        ProcessTextField();

    }

    private void ShowDialoguePanel()
    {
        m_NameField.text = m_DialogueNPC.GetName();

        Debug.Log("Starting conversation with: " + m_DialogueNPC.GetName());

        // Populate sentence queue
        m_DialogueQueue.Clear();

        foreach(string sentence in m_DialogueNPC.GetDialogue().sentences)
        {
            m_DialogueQueue.Enqueue(sentence);
        }

        ProcessTextField();

        m_Transparency.DOFade(1, 0.5f).SetEase(Ease.OutBack);

    }

    private void EndDialogue()
    {
        m_Transparency.DOFade(0, 0.25f).OnComplete(() =>
        {
            m_HasDialogueStarted = false;
        });
    }

    private void ProcessTextField()
    {
        m_IsProcessingText = true;

        if (m_DialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        m_CurrentSentence = m_DialogueQueue.Dequeue();

        StartCoroutine(AnimateText(m_CurrentSentence, 0.5f, 0.3f, 0.02f));
    }

    /// <summary>
    /// Animates the text in a typewriter effect
    /// </summary>
    /// <param name="text"></param>
    /// <param name="initialDelay"></param>
    /// <param name="returnKeyDelay"></param>
    /// <param name="typingSpeed"></param>
    /// <returns></returns>
    private IEnumerator AnimateText(string text, float initialDelay, float returnKeyDelay, float typingSpeed)
    {
        // Hide the cursor
        m_NextFieldCursor.DOScale(Vector3.zero, 0.1f);
        m_NextFieldCursor.localPosition = Vector3.zero;
        t.SetLoops(0);
        t.Complete();
        t.Kill();

        yield return new WaitForSeconds(initialDelay);

        string finalText = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (!m_IsProcessingText)
            {
                Debug.Log(">> Skipped text animation");
                break;
            }

            if (text[i] == '\n')
            {
                yield return new WaitForSeconds(returnKeyDelay);
            }

            if (text[i] == '\\')
            {
                if (text[i + 1] == 'n')
                {
                    finalText += "<br>";
                    i++;
                    yield return new WaitForSeconds(returnKeyDelay);
                }
            }
            else
            {
                finalText += text[i];
            }

            m_TextField.text = finalText;
            yield return new WaitForSeconds(typingSpeed);
        }

        Debug.Log(">> Finished animating text - Showing next field cursor");
        ShowNextFieldCursor(text);

    }

    private void CheckForEscapeCharactersInText() { }

    /// <summary>
    /// Show the next field cursor - Called whenever the text animation finishes
    /// </summary>
    private void ShowNextFieldCursor(string currentTextField)
    {
        Debug.Log(">> Showing next field cursor");

        m_TextField.text = currentTextField;

        m_NextFieldCursor.localPosition = Vector3.zero;

        // Show the cursor once the dialogue has finished
        m_NextFieldCursor.DOScale(Vector3.one, 0.2f).OnComplete(() =>
        {
            m_IsProcessingText = false;
            t = m_NextFieldCursor.DOLocalMoveY(-5, 0.3f).SetAutoKill(false).SetLoops(-1, LoopType.Yoyo);
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