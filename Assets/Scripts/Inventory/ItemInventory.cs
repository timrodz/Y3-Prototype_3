using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour {

    //GameObject inventoryPanel;
    //GameObject slotPanel;

    [HideInInspector]
    private ItemDatabase database;

    public GameObject inventorySlot;
    public GameObject inventoryItem;


    public int MaxSlots = 4;

    public List<Item> items = new List<Item>();
    private InventorySlot[] slots;

	// Use this for initialization
	void Start () {

        //inventoryPanel = GameObject.Find("Inventory Panel");
        //slotPanel = inventoryPanel.transform.Find("Slot Panel").gameObject;
        database = ItemDatabase.instance;

        slots = GetComponentsInChildren<InventorySlot>();
	}

    void OnEnable()
    {
        EventManager.StartListening(EventName.QuestCheck, QuestCheckHasItem);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        EventManager.StartListening(EventName.QuestCheck, QuestCheckHasItem);
    }

    public void QuestCheckHasItem()
    {

    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            //print(AddItem(1).ToString());
            AddItem(2);
        }
    }

    public void HasItemRequest(int id)
    {

    }

    public ItemState AddItem(int id, int countMult = 1)
    {

        Item itemFound = database.FetchItemByID(id);

        if (itemFound == null)
        {
            return ItemState.IDInvalid;
        }

        for(int i = 0; i < items.Count; i++)
        {
            if(items[i].ID == id)
            {
                items[i].count += itemFound.count * countMult;
                UpdateItemSlot(items[i]);
                return ItemState.AddCountSuccess;
            }
        }

        if(items.Count >= 4)
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
        foreach (InventorySlot slot in slots)
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
            if(slot.isAvailable)
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
