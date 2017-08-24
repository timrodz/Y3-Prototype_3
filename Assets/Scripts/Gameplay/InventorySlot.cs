using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour {

    public bool isAvailable = true;
    public bool hasItem = false;
    public int ItemCount = 0;
    //public ItemInfo item;

    public Item item;
    Image icon;
    TextMeshProUGUI text;

    public void Awake()
    {
        icon = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        icon.color = Utils.ChangeOpacity(icon.color, 0);
        text.text = "";
    }

    public bool SetItem(Item item)
    {
        this.item = item;
        icon.sprite = item.Sprite;
        icon.color = Color.white;
        text.text = item.count.ToString();

        hasItem = true;
        isAvailable = false;

        return true;
    }

}
