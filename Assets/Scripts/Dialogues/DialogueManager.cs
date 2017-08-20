using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Visual Fields")]
    [SerializeField] private CanvasGroup m_Transparency;
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private TMPro.TextMeshProUGUI m_TextField;
    [SerializeField] private RectTransform m_NextFieldCursor;

    [Header("Current Dialogue NPC")]
    [SerializeField] private GameObject m_Sender;
    [SerializeField] private NPC m_DialogueNPC;
    [SerializeField] private CustomEvent m_Event;

    [SerializeField] private bool m_HasDialogueStarted;

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

    private void ProcessDialogue()
    {
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

        ProcessTextField();
        m_Transparency.DOFade(1, 0.5f).SetEase(Ease.OutBack);

    }

    private void ProcessTextField()
    {
        StartCoroutine(AnimateText(0.4f, 0.3f, 0.02f));
    }

    /// <summary>
    /// Animates the text in a typewriter effect
    /// </summary>
    /// <param name="initialDelay"></param>
    /// <param name="returnKeyDelay"></param>
    /// <param name="typingSpeed"></param>
    /// <returns></returns>
    private IEnumerator AnimateText(float initialDelay, float returnKeyDelay, float typingSpeed)
    {
        // Hide the cursor
        m_NextFieldCursor.DOScale(Vector3.zero, 0.1f);
        m_NextFieldCursor.DOLocalMoveY(0, 0f);
        
        yield return new WaitForSeconds(initialDelay);

        string text = "This is a random text field!\nHow are you?";

        string finalText = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                yield return new WaitForSeconds(returnKeyDelay);
            }
            finalText += text[i];
            m_TextField.text = finalText;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        // Show the cursor once the dialogue has finished
        m_NextFieldCursor.DOScale(Vector3.one, 0.2f).OnComplete(() =>
        {
            m_NextFieldCursor.DOLocalMoveY(-5, 0.3f).SetLoops(-1, LoopType.Yoyo);
        });
        
    }

    private NPC SetupDialogueNPC()
    {
        Debug.Log("===== ATTEMPTING TO ACQUIRE NPC INFORMATION ====");

        m_Sender = EventManager.GetRegisteredSender();

        // No sender
        if (null == m_Sender)
        {
            Debug.Log(">> No sender set");
            return null;
        }

        DialogueInteractionHandler dih = m_Sender.GetComponent<DialogueInteractionHandler>();

        // Check if the object has an interaction handler
        if (null == dih)
        {
            Debug.LogWarning(">> Dialogue interaction handler doesn't have an NPC");
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

        NPC npc = dih.GetNPCInformation();
        npc.SetObject(dih.transform.parent.gameObject);

        Debug.Log("===== ACQUIRED NPC INFORMATION ====");
        Debug.Log("NPC Name: " + npc.GetName());

        return npc;
    }

}