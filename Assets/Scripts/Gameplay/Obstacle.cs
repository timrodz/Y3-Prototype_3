using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Obstacle : MonoBehaviour {

    public Transform FinalPosition;
    public Transform RestingPosition;


    bool Complete = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveObstacle()
    {
        if (Complete)
            return;

        transform.DOMove(FinalPosition.position, 1.0f);

        Complete = true;
    }
}
