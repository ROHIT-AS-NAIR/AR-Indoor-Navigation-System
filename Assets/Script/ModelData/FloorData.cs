﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorData : MonoBehaviour {

	public List<GameObject> markerList;
	public string floor = "0";

	// Use this for initialization
	void Start () {
		/* Work after All Marker Data get their position */
		floor = gameObject.name.Replace("Floor","");
		foreach (Transform childTransform in transform)
		{
			GameObject objToAdd = childTransform.gameObject;
			if(objToAdd.GetComponent<MarkerData>() != null)
			{
				Debug.Log("Get Marker " + objToAdd.GetComponent<MarkerData>().markerName 
					+ ": " +objToAdd.GetComponent<MarkerData>().position);
				objToAdd.GetComponent<MarkerData>().floor = floor;
				markerList.Add(objToAdd);
			}
			
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}