using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ItemPickUp))]
public class ItemPickupEditor : Editor {

    int selected = 0;

    public override void OnInspectorGUI()
    {
        ItemPickUp pickup = (ItemPickUp)target;

        string[] options;
        if (DrawDefaultInspector())
        {
   
        }

        ItemDatabase database = ItemDatabase.instance;
        options = new string[database.database.Count];
        for (int i = 0; i < options.Length; i++)
        {
            options[i] = database.database[i].Title;
        }

        selected = EditorGUILayout.Popup("Item Type", selected, options);

        pickup.item = new Item(database.FetchItemByID(selected));
    }
}

