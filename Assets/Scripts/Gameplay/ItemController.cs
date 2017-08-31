using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public ItemType itemName;
    private GameObject Player;
    private PlayerInventory playerInventory;
    

    // Use this for initialization
    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        Player = FindObjectOfType<PlayerEventHandler>().gameObject;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == Player)
        {
            Destroy(this.gameObject);
        }

        playerInventory.PickUpItem(itemName);
    }
}

