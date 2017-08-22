using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueInteractionHandler : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private bool m_IsInteractable;
    private NPC m_NPCInfo;

    [Header("Visual Fields")]
    [SerializeField] private CanvasGroup m_Transparency;
    [SerializeField] private TMPro.TextMeshProUGUI m_NameField;
    [SerializeField] private Image m_InteractableImage;
    private Vector3 m_InteractableImageScale;

    private bool m_CanInteract;
    Tween interactableImageTween;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_Transparency.alpha = 0;
        m_InteractableImageScale = m_InteractableImage.rectTransform.localScale;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        Setup();
    }

    private void Setup()
    {
        m_NPCInfo = GetComponentInParent<NPCHolder>().NPC;
        m_NameField.text = m_NPCInfo.GetName();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening(EventName.OnPlayerTriggerEnter, ShowPanel);
        EventManager.StartListening(EventName.OnPlayerTriggerExit, HidePanel);
        EventManager.StartListening(EventName.DialogueStart, HidePanelButtonImage);
        EventManager.StartListening(EventName.DialogueEnd, ShowPanelButtonImage);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.OnPlayerTriggerEnter, ShowPanel);
        EventManager.StopListening(EventName.OnPlayerTriggerExit, HidePanel);
        EventManager.StopListening(EventName.DialogueStart, HidePanelButtonImage);
        EventManager.StopListening(EventName.DialogueEnd, ShowPanelButtonImage);
    }

    private void ShowPanel()
    {
        // If the sender is not the object, there's no need to show our interactability
        if (this.gameObject != EventManager.GetRegisteredSender())
        {
            HidePanel();
            return;
        }

        Debug.Log("Showing " + name);

        m_CanInteract = true;
        m_Transparency.DOFade(1, 0.35f);

        if (m_IsInteractable)
        {
            m_InteractableImage.DOFade(1, 0.25f);
            ShowPanelButtonImage();
        }
    }

    private void HidePanel()
    {
        if (this.gameObject != EventManager.GetRegisteredSender()) return;

        Debug.Log("Hiding " + name);

        m_CanInteract = false;
        m_Transparency.DOFade(0, 0.15f);

        if (m_IsInteractable)
        {
            interactableImageTween.SetLoops(0);
            interactableImageTween.Complete();
            interactableImageTween.Kill();
            m_InteractableImage.rectTransform.localScale = m_InteractableImageScale;
        }
    }

    private void ShowPanelButtonImage()
    {
        Debug.Log("Showing " + name + " button image");

        if (m_IsInteractable)
        {
            interactableImageTween.SetLoops(0);
            interactableImageTween.Complete();
            interactableImageTween.Kill();
            m_InteractableImage.DOFade(1, 0.25f);
            interactableImageTween = m_InteractableImage.rectTransform.DOScale(m_InteractableImageScale * 1.35f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void HidePanelButtonImage()
    {
        Debug.Log("Hiding " + name + " button image");

        if (m_IsInteractable)
        {
            interactableImageTween.SetLoops(0);
            interactableImageTween.Complete();
            interactableImageTween.Kill();
            m_InteractableImage.DOFade(0, 0.25f);
            m_InteractableImage.rectTransform.localScale = m_InteractableImageScale;
        }
    }

    public bool IsInteractable()
    {
        return m_IsInteractable;
    }

    public bool CanInteract()
    {
        return m_CanInteract;
    }

    public NPC GetNPC()
    {
        return m_NPCInfo;
    }

} // DialogueInteractionHandler