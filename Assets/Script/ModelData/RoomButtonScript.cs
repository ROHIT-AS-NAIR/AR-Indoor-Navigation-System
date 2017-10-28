﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomButtonScript : MonoBehaviour {

	public GameObject room;
	public bool isDestination = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetDestination()
	{
		if(isDestination)
		{
			MainController.instance.ClearDestinationPoint();
		}
		else
		{
			MainController.instance.SetDestinationPoint(room);
		}
		gameObject.transform.root.GetComponent<CanvasButtonScript>().OnCloseSerch();
	}

	public void PrintName()
	{
		Debug.Log(room.GetComponent<MarkerData>().roomName);
	}
}
