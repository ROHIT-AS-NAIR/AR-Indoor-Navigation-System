using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReader : MonoBehaviour {

	public string fileroomname = "RoomJsonData.json";
	public string[] fileToLoadName = {"FloorJsonData.json", "RoomJsonData.json"};
	private string path;
	private string jsonString;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			LoadFloor();
		}
		if(Input.GetKeyDown(KeyCode.LeftShift))
		{
			LoadRoom();
		}
	}

	public void InitReading(string filename)
	{
		path = Application.streamingAssetsPath + "/JsonData/";
		jsonString = File.ReadAllText(path + filename);
	}

	private void LoadFloor()
	/* Load and Create Floor, floorPlane with material */
	{
		string floorFileJsonName = fileToLoadName[0];
		InitReading(floorFileJsonName);
		Vector3 planePosition = new Vector3(500,0,500);
		Quaternion planeRotation = Quaternion.Euler(new Vector3(0, 180, 0));
		Vector3 planeScale = new Vector3(100, 1, 100);

		JFloor[] floors = JsonHelper.FromJson<JFloor>(jsonString);
		Debug.Log(floors.Length);

		foreach (JFloor floor in floors)
		{
			GameObject emptyobj = new GameObject("Floor"+floor.floorName);
			emptyobj.transform.position = new Vector3(Mathf.CeilToInt(floor.floorIndex)*1000, 0, 0);
			emptyobj.AddComponent<FloorData>();
			FloorData fdt = emptyobj.GetComponent<FloorData>();
			fdt.floorName = floor.floorName;
			fdt.floorIndex = Mathf.CeilToInt(floor.floorIndex);
			// create plane child
			GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
			Transform planeTransform = plane.transform;
			planeTransform.SetParent(emptyobj.transform);
			planeTransform.localPosition = planePosition;
			planeTransform.localRotation = planeRotation;
			planeTransform.localScale = planeScale;
			ChangeMat(plane, floor.floorImageName);
			//find building parent
		}
	}

	private void ChangeMat(GameObject planeObj, string floorImageName)
	{
		Material matt = new Material(Shader.Find("Unlit/UnlitShader01"));
		matt.mainTexture = (Texture) Resources.Load("MapImage/" + floorImageName);
		planeObj.GetComponent<Renderer>().material = matt;
	}

	private void LoadRoom()
	/* load room and attrach to floor */
	{
		string roomFileJsonName = fileToLoadName[1];
		InitReading(roomFileJsonName);

		JRoom[] rooms = JsonHelper.FromJson<JRoom>(jsonString);
		Debug.Log(rooms.Length);

		foreach (JRoom room in rooms)
		{
			GameObject emptyobj = new GameObject(room.roomName);
			//define position
			emptyobj.AddComponent<RoomData>();
			RoomData rdt = emptyobj.GetComponent<RoomData>();
			rdt.roomName = room.roomName;
			rdt.roomDescription = room.roomDescription;
			rdt.isConnector = room.isConnector;
			//find parent
		}
	}
}

[System.Serializable]
public class JFloor
{
	public string floorName;
	public float floorIndex;
	public string floorImageName;
}

[System.Serializable]
public class JRoom
{
	public string roomName;
	public string roomDescription;
	public bool isConnector;
}


