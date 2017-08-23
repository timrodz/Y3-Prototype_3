using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    private float berryCount;
    private float woodCount;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PickUpItem (ItemHasg item)
    {
        switch (item)
        {
            case ItemHasg.None:
                break;

            case ItemHasg.Berry:

                berryCount++;
                print("Number of berries " + berryCount);

                break;

            case ItemHasg.Wood:

                woodCount++;
                print("Number of wood " + woodCount);

                break;

            default:
                break;
        }
    }
}
