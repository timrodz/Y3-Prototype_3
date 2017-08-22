using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    public bool isAvailable = true;
    public bool hasItem = false;
    public ItemInfo item;
    Image icon;

    public void Start()
    {
        icon = GetComponentInChildren<Image>();
    }

    public bool UpdateItem(ItemInfo item)
    {

        if(item.action == ItemAction.ADD && !hasItem && isAvailable)
        {
            this.item = item;

            icon.sprite = item.icon;
            icon.color = Color.white;


            isAvailable = false;
            hasItem = true;

            return true;
        }

        if (item == null)
            return false;

        else if(ItemAction.REMOVE == item.action)
        {
            if (hasItem && !isAvailable && item.name == this.item.name)
            {
                print("ITEM REMOVED");

                item = null;
                icon.sprite = null;
                icon.color = Color.clear;
                isAvailable = true;
                hasItem = false;

                return true;
            }
        }

        return false;

    }
}
