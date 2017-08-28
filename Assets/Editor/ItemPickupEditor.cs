using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ItemPickUp))]
public class ItemPickupEditor : Editor {

    int selected = 0;
    int index = 0;
    int prev = -1;
    bool Edited = false;

    int itemID;
    int itemName;

    public override void OnInspectorGUI()
    {
        ItemPickUp pickup = (ItemPickUp)target;

        string[] options;
        ItemDatabase database = ItemDatabase.instance;
        options = new string[database.database.Count];
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = database.database[i].Title;
        }

        selected = EditorGUILayout.Popup("Item Type", selected, options);

        if (!pickup.isPickupInstance)
        {
            if (selected != prev)
            {
                itemName = selected;
                prev = selected;
                pickup.item = new Item(database.FetchItemByID(selected));
            }
        }
        if (DrawDefaultInspector())
        {
   
        }
    }
}

