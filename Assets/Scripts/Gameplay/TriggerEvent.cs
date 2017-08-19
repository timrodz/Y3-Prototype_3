﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour {

    public UnityEvent myEvent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        if(myEvent != null)
        {
            myEvent.Invoke();
        }

    }

    public void OnTriggerExit(Collider other)
    {
        
    }
}
