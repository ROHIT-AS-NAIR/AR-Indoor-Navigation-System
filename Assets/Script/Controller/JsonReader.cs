using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReader : MonoBehaviour {

	public string fileroomname = "RoomJsonData.json";
	public string[] fileToLoadName = {"BuildingJsonData.json", "FloorJsonData.json", 
		"RoomJsonData.json", "NodeJsonData.json", "MarkerJsonData.json"};
	public string[] structTag = {"Building", "Floor", "Room", "Node", "Marker"};
	private string path;
	private string jsonString;

	private enum StctType{
		/* it is define order of object too */
		Building = 0,
		Floor = 1,
		Room = 2,
		Node = 3,
		Marker = 4
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			LoadBuilding();
			LoadFloor();
		}
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			LoadRoom();
		}
		if(Input.GetKeyDown(KeyCode.T))
		{
			CheckTag("Floor");
		}
	}

	public void InitReading(string filename)
	{
		path = Application.streamingAssetsPath + "/JsonData/";
		jsonString = File.ReadAllText(path + filename);
	}

#region Building
	private void LoadBuilding()
	/* load building */
	{
		string roomFileJsonName = fileToLoadName[(int)StctType.Building];
		InitReading(roomFileJsonName);

		JBuilding[] buildings = JsonHelper.FromJson<JBuilding>(jsonString);
		Debug.Log("Loading Building total:" + buildings.Length);

		foreach (JBuilding building in buildings)
		{
			GameObject emptyobj = new GameObject(structTag[(int)StctType.Building] + building.buildingID);
			emptyobj.tag = structTag[(int)StctType.Building];
			//define position
			emptyobj.AddComponent<BuildingData>();
			BuildingData bdt = emptyobj.GetComponent<BuildingData>();
			bdt.buidingID = building.buildingID;
			bdt.buildingName = building.buildingName;

			//set position depend on index *1000
		}
	}
#endregion

#region Floor
	private void LoadFloor()
	/* Load and Create Floor, floorPlane with material */
	{
		string floorFileJsonName = fileToLoadName[(int)StctType.Floor];
		InitReading(floorFileJsonName);
		Vector3 planePosition = new Vector3(500,0,500);
		Quaternion planeRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Vector3 planeScale = new Vector3(100, 1, 100);

		JFloor[] floors = JsonHelper.FromJson<JFloor>(jsonString);
		Debug.Log("Loading Floor total:" + floors.Length);

		foreach (JFloor floor in floors)
		{
			GameObject emptyobj = new GameObject(structTag[(int)StctType.Floor] + floor.floorID);
			emptyobj.tag = structTag[(int)StctType.Floor];
			
			// add floor data
			emptyobj.AddComponent<FloorData>();
			FloorData fdt = emptyobj.GetComponent<FloorData>();
			fdt.floorID = floor.floorID;
			fdt.floorName = floor.floorName;
			fdt.floorIndex = Mathf.CeilToInt(floor.floorIndex);
			fdt.fkBuildingID = floor.buildingID;

			//find building parent and set localpos
			FindObjectToAttract(emptyobj, StctType.Building, fdt.fkBuildingID);
			emptyobj.transform.localPosition = new Vector3(Mathf.CeilToInt(floor.floorIndex)*1000, 0, 0);
			
			// create plane child
			GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			Transform planeTransform = plane.transform;
			planeTransform.SetParent(emptyobj.transform);
			planeTransform.localPosition = planePosition;
			planeTransform.localRotation = planeRotation;
			planeTransform.localScale = planeScale;
			ChangeMat(plane, floor.floorImageName);
			
		}
	}

	private void ChangeMat(GameObject planeObj, string floorImageName)
	{
		Material matt = new Material(Shader.Find("Unlit/UnlitShader01"));
		matt.mainTexture = (Texture) Resources.Load("MapImage/" + floorImageName);
		planeObj.GetComponent<Renderer>().material = matt;
	}

#endregion

#region Room
	private void LoadRoom()
	/* load room and attrach to floor */
	{
		string roomFileJsonName = fileToLoadName[(int)StctType.Room];
		InitReading(roomFileJsonName);

		JRoom[] rooms = JsonHelper.FromJson<JRoom>(jsonString);
		Debug.Log("Loading Room total:" + rooms.Length);

		foreach (JRoom room in rooms)
		{
			GameObject emptyobj = new GameObject(structTag[(int)StctType.Room] + room.roomID);
			emptyobj.tag = structTag[(int)StctType.Room];
			//define position
			emptyobj.AddComponent<RoomData>();
			RoomData rdt = emptyobj.GetComponent<RoomData>();
			rdt.roomID = room.roomID;
			rdt.roomName = room.roomName;
			rdt.roomDescription = room.roomDescription;
			rdt.isConnector = room.isConnector;
			rdt.fkFloorID = room.floorID;
			//find parent
			FindObjectToAttract(emptyobj, StctType.Floor, rdt.fkFloorID);
			emptyobj.transform.localPosition = Vector3.zero;
		}
	}
	
#endregion

#region Node
	private void LoadNode()
	{

	}
#endregion


#region Object Tag Find
	private bool FindObjectToAttract(GameObject objChild, StctType parentType, string parentName)
	/* find all object from given tag and attract to object that similar name
	parent name may + with string before
	 -> return false if no object */
	{
		GameObject[] tagObjs = GameObject.FindGameObjectsWithTag(structTag[(int)parentType]);
		if(tagObjs.Length == 0)
		{
			return false;
		}
		foreach (GameObject tObj in tagObjs)
		{
			if(structTag[(int)parentType] + parentName == tObj.name)
			{
				objChild.transform.SetParent(tObj.transform);
				return true;
			}
		}
		return false;
	}

	private bool CheckTag(string objdectTag)
	/* print all name of gameobject with that tag */
	{
		GameObject[] tagObjs = GameObject.FindGameObjectsWithTag(objdectTag);
		foreach (GameObject tObj in tagObjs)
		{
			Debug.Log(tObj.name);
		}
		return false;
	}
#endregion
}


#region Map Struct

[System.Serializable]
public class JNode
{
	public string nodeID;
	public string position;
	public string referencePosition;
	public int zOrientation;
	public string xyOrientation;
	public string roomID;
}

[System.Serializable]
public class JRoom
{
	public string roomID;
	public string roomName;
	public string roomDescription;
	public bool isConnector;
	public string floorID;
}

[System.Serializable]
public class JFloor
{
	public string floorID;
	public string floorName;
	public float floorIndex;
	public string floorImageName;
	public string buildingID;
}

[System.Serializable]
public class JBuilding
{
	public string buildingID;
	public string buildingName;
}

#endregion

