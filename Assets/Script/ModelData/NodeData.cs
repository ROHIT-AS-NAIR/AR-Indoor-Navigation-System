using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour {

	public string nodeID;
	public Vector3 position = Vector3.zero;
	public Vector3 referencePosition = Vector3.zero;
	public Vector3 orientation = Vector3.zero;
	public float cost = Single.PositiveInfinity;
	public GameObject predecessor = null;
	public GameObject successor = null;
	public List<GameObject> adjacentNodeList;

	public string fkRoomID;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* get parent data  (Roomdata) */
	private RoomData GetParentObjectData()
	{
		return this.transform.parent.gameObject.GetComponent<RoomData>();
	}

#region Floor Method

	public GameObject GetFloor() /* return gameObject of floorData */
	{
		//if(this.transform.parent.gameObject.GetComponent<FloorData>() != null)
		return this.transform.parent.gameObject;
	}

	/* assume that get nodedata to compare */
	public bool IsSameFloorWith(GameObject compareFloorObj)
	{
		if(this.GetParentObjectData().fkFloorID == compareFloorObj.GetComponent<NodeData>().GetParentObjectData().fkFloorID)
		{
			return true;
		}
		return false;
	}
#endregion

	private bool AttractFloor()
	/* for attract with floor and set position */
	{
		GameObject[] floorObjs = GameObject.FindGameObjectsWithTag("Floor");
		if(floorObjs.Length == 0)
		{
			return false;
		}
		foreach (GameObject flObj in floorObjs)
		{
			Debug.Log(flObj.name);
			
		}
		return true;
	}

	
	public void SetupSelf()
	{

	}

	private void AttractRoom()
	/* to be heirachy of room */
	{

	}

}
