using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public static Inventory instance;

    public ItemInfo testingItem;

    public delegate void OnItemChanged(ItemInfo item);

    public OnItemChanged onItemChangedCallback;

    public List<ItemInfo> items = new List<ItemInfo>();
    public int space = 4;

    #region Singleton
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory");
            return;
        }
        instance = this;    
    }
#endregion Singleton

    public bool Add(ItemInfo item)
    {
        if(!item.defaultItem)
        {
            if(items.Count >= space)
            {
                Debug.Log("Not enough space in inventory");
                //Send Event for a dialogue "No Space";
                return false;
            }
            items.Add(item);

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke(item);
            }
        }

        return true;
    }

    public void Remove(ItemInfo item)
    {
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke(item);
        }
        print( "Removing = " + items.Remove(item).ToString());
    }


    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            testingItem.action = ItemAction.ADD;
            Add(testingItem);
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            testingItem.action = ItemAction.REMOVE;
            Remove(testingItem);
        }
    }
}
