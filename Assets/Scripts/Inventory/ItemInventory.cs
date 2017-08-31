using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    [HideInInspector]
    private ItemDatabase database;
    public static ItemInventory instance;

    public GameObject inventorySlot;
    public GameObject inventoryItem;

    public int MaxSlots = 4;

    public List<Item> items = new List<Item>();
    private InventorySlot[] slots;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Use this for initialization
    void Start()
    {

        if (instance != null)
            Destroy(this.gameObject);

        instance = this;

        //inventoryPanel = GameObject.Find("Inventory Panel");
        //slotPanel = inventoryPanel.transform.Find("Slot Panel").gameObject;
        database = ItemDatabase.instance;

        slots = GetComponentsInChildren<InventorySlot>();
    }

    void OnEnable()
    {
        EventManager.StartListening(EventName.QuestCheck, QuestCheckHasItem);
        EventManager.StartListening(EventName.DialogueStart, Hide);
        EventManager.StartListening(EventName.DialogueEnd, Show);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening(EventName.QuestCheck, QuestCheckHasItem);
        EventManager.StopListening(EventName.DialogueStart, Hide);
        EventManager.StopListening(EventName.DialogueEnd, Show);
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
    }

    public void QuestCheckHasItem()
    {
        Debug.Log(">> Checking quests");

        if (QuestManager.GetQuestList().Count == 0)
        {
            Debug.Log(">> No quests");
            EventManager.Invoke(EventName.DialogueStart);
            return;
        }

        bool canComplete = false;

        foreach(Quest q in QuestManager.GetQuestList())
        {
            q.GetInfo();

            int itemQuantity = 0;

            foreach(Item i in items)
            {
                Debug.Log(">> Item name: " + i.Title);

                if (i.Title == "Carrot")
                {
                    itemQuantity++;
                }

            }

            int it = q.GetItemQuantity();

            Debug.Log(">>>>>> " + it + " >>>>> " + itemQuantity);

            if (it == itemQuantity)
            {
                Debug.Log("Matched item quantity");
                canComplete = true;
            }
        }

        if (canComplete)
        {
            // QuestManager.CompleteCurrentQuest();
            EventManager.Invoke(EventName.QuestComplete);
        }

        Debug.Log("OK");

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //print(AddItem(1).ToString());
            AddItem(2);
        }
    }

    public void HasItemRequest(int id)
    {

    }

    public ItemState AddItem(Item item)
    {

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == item.ID)
            {
                items[i].count += item.count;
                UpdateItemSlot(items[i]);
                return ItemState.AddCountSuccess;
            }
        }

        if (items.Count >= 4)
        {
            return ItemState.NoSpace;
        }
        else
        {
            Item itemToAdd = new Item(item);

            items.Add(itemToAdd);
            AddItemToFreeSlot(itemToAdd);

            return ItemState.AddSuccess;
        }
    }

    public ItemState AddItem(int id, int countMult = 1)
    {

        Item itemFound = database.FetchItemByID(id);

        if (itemFound == null)
        {
            return ItemState.IDInvalid;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ID == id)
            {
                items[i].count += itemFound.count * countMult;
                UpdateItemSlot(items[i]);
                return ItemState.AddCountSuccess;
            }
        }

        if (items.Count >= 4)
        {
            return ItemState.NoSpace;
        }
        else
        {
            Item itemToAdd = new Item(itemFound);

            items.Add(itemToAdd);
            AddItemToFreeSlot(itemToAdd);

            return ItemState.AddSuccess;
        }
    }

    private void UpdateItemSlot(Item item)
    {
        foreach(InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                slot.SetItem(item);
                return;
            }
        }
    }
    private void AddItemToFreeSlot(Item item)
    {
        foreach(InventorySlot slot in slots)
        {
            if (slot.isAvailable)
            {
                slot.SetItem(item);
                return;
            }
        }
    }

    public enum ItemState
    {
        IDInvalid,
        AddSuccess,
        AddCountSuccess,
        RemoveSuccess,
        AddFail,
        NoSpace,
        RemoveFail
    }
}