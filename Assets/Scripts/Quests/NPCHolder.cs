﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHolder : MonoBehaviour
{
	public NPC NPC;
	
	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		NPC.SetQuests();
	}
}