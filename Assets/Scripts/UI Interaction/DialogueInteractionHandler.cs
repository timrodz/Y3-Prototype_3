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
	[Header("World canvas transparency group")]
    [SerializeField] private CanvasGroup transparency;
	// [SerializeField] private TMPro.TextMeshProUGUI textField;
	
	[Space] [Header("Interactable field")]
    [SerializeField] private bool m_IsInteractable;
    [SerializeField] private Image m_InteractableImage;
    private Vector3 m_InteractableImageScale;
	// [SerializeField] private string name

    private bool m_CanInteract;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        transparency.alpha = 0;
		LoadImageButton();
    }

    private void LoadImageButton()
    {
        var sprite = Resources.Load<Sprite>("UI/Buttons/Interact_Button");
		Debug.Log(sprite);
		m_InteractableImage.sprite = sprite;
        m_InteractableImageScale = m_InteractableImage.rectTransform.localScale;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening(EventName.OnPlayerTriggerEnter, ShowInteractableImage);
        EventManager.StartListening(EventName.OnPlayerTriggerExit, HideInteractableImage);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.OnPlayerTriggerEnter, ShowInteractableImage);
        EventManager.StopListening(EventName.OnPlayerTriggerExit, HideInteractableImage);
    }

    private void ShowInteractableImage()
    {
        // If the sender is not the object, there's no need to show our interactability
        if (this.gameObject != EventManager.GetRegisteredSender())
        {
            HideInteractableImage();
            return;
        }

        Debug.Log("Showing " + name);

        m_CanInteract = true;
        transparency.DOFade(1, 0.15f);

        if (m_IsInteractable)
        {
            m_InteractableImage.rectTransform.DOScale(m_InteractableImageScale * 1.35f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void HideInteractableImage()
    {
        if (this.gameObject != EventManager.GetRegisteredSender()) return;

        Debug.Log("Hiding " + name);

        m_CanInteract = false;
        transparency.DOFade(0, 0.05f);

        if (m_IsInteractable)
        {
            m_InteractableImage.rectTransform.DOScale(m_InteractableImageScale, 0);
        }
    }

} // DialogueInteractionHandler