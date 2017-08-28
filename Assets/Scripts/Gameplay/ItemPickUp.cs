using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using DG.Tweening;

[RequireComponent(typeof(Collider))]
public class ItemPickUp : MonoBehaviour {


    [Header("Item from Database")]
    public Item item;

    [Header("Variables")]
    [SerializeField]
    private bool m_IsInteractable;
    [SerializeField]
    private bool m_DoesDestroySelf;

    public bool isPickupInstance = false;

    [SerializeField] private Image m_InteractableImage;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    [Header("Additional PickUp Events")]
    [SerializeField]
    public UnityEvent Events;
    

    private Vector3 m_InteractableImageScale;
    private bool m_CanInteract;
    private bool m_PlayerTriggered = false;
    Tween interactableImageTween;

    public void Start()
    {
        m_InteractableImageScale = m_InteractableImage.rectTransform.localScale;
        text.gameObject.SetActive(false);
        m_InteractableImage.gameObject.SetActive(false);
    }

    public void Update()
    {
        if(!m_PlayerTriggered)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            ItemInventory.instance.AddItem(item);

            if(Events != null)
            {
                Events.Invoke();
            }

            if (m_DoesDestroySelf)
                Destroy(this.gameObject);

        }
    }

    public void OnValidate()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            m_PlayerTriggered = true;
            text.gameObject.SetActive(true);
            m_InteractableImage.gameObject.SetActive(true);

            m_InteractableImage.DOFade(1, 0.25f);
            interactableImageTween = m_InteractableImage.rectTransform.DOScale(m_InteractableImageScale * 1.35f, 0.4f).SetLoops(-1, LoopType.Yoyo);

            text.text = item.Title + " x " + item.count;
        }
    }


    public void OnTriggerExit(Collider other)
    {
        text.gameObject.SetActive(false);
        interactableImageTween.SetLoops(0);
        interactableImageTween.Complete();
        interactableImageTween.Kill();
        m_InteractableImage.DOFade(0, 0.25f);
        m_InteractableImage.rectTransform.localScale = m_InteractableImageScale;
    }
}
