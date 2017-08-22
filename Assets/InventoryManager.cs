using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    Inventory inventory;


    private InventorySlot[] Slots;


	// Use this for initialization
	void Start () {
        inventory = Inventory.instance;

        inventory.onItemChangedCallback += UpdateInventory;

        Slots = GetComponentsInChildren<InventorySlot>();

        print(Slots.Length);
	}
	
    public void UpdateInventory(ItemInfo item)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            if(Slots[i].UpdateItem(item))
                return;
        }
    }

    

	// Update is called once per frame
	void Update () {
		
	}
}
