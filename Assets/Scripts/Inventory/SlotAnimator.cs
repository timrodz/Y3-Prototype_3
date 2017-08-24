using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlotAnimator : MonoBehaviour {

    ItemInventory inventory;
    int prevCount = 0;

	// Use this for initialization
	void Start () {
        inventory = ItemInventory.instance;
        prevCount = inventory.items.Count;
    }

    // Update is called once per frame
    void Update () {

        if(inventory.items.Count != prevCount)
        {
            prevCount = inventory.items.Count;

            transform.DOMoveX(transform.position.x + 132 + (27.5f / 2.0f), 1.0f);
        }
    }
}

