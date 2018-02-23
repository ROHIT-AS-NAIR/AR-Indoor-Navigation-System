using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARDisplayController : MonoBehaviour {

	private enum ARType
	{
		Arrow,
		Check,
		Board
	}

	public enum ArrowDirection
	{
		Up,
		Down,
		Front
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* get command from maincontroller
	detect marker show/lost from markerConstructor
	control arrow up/down*/

	/* point arrow to successor's position.  if null, point to destination */
	public void ShowArrow(GameObject objectNodeToAugment, bool navigatable)
	{
		GameObject markerObject = GetMarkerObjectToAugment(objectNodeToAugment);
		GameObject arrowObj = GetARObjectOfType(markerObject, ARType.Arrow);
		ArrowScript arrowsc = arrowObj.GetComponent<ArrowScript>();
		arrowsc.PointToCoordinate(objectNodeToAugment.GetComponent<NodeData>().successor.GetComponent<NodeData>().position);
		arrowObj.SetActive(true);
		PlusMarkerOrientation(arrowObj);
	}

	/* point directly to node */
	public void ShowArrow(GameObject objectNodeToAugment, GameObject destinationNode)
	{
		GameObject markerObject = GetMarkerObjectToAugment(objectNodeToAugment);
		GameObject arrowObj = GetARObjectOfType(markerObject, ARType.Arrow);
		ArrowScript arrowsc = arrowObj.GetComponent<ArrowScript>();
		arrowsc.PointToCoordinate(destinationNode.GetComponent<NodeData>().position);
		arrowObj.SetActive(true);
		PlusMarkerOrientation(arrowObj);
	}

	public void ShowArrow(GameObject objectNodeToAugment, bool navigatable, ArrowDirection arrowDirection)
	{
		GameObject markerObject = GetMarkerObjectToAugment(objectNodeToAugment);
		GameObject arrowObj = GetARObjectOfType(markerObject, ARType.Arrow);
		ArrowScript arrowsc = arrowObj.GetComponent<ArrowScript>();
		switch (arrowDirection)
		{
			case ArrowDirection.Up: arrowsc.PointArrowUp(); break;
			case ArrowDirection.Down: arrowsc.PointArrowDown(); break;
			case ArrowDirection.Front: arrowsc.PointToZero(); break;
			default: arrowsc.PointToZero(); break;
		}
		arrowObj.SetActive(true);
		PlusMarkerOrientation(arrowObj);
	}

	public void ShowCheck(GameObject objectNodeToAugment)
	{
		GameObject markerObject = GetMarkerObjectToAugment(objectNodeToAugment);
		GameObject checkObj = GetARObjectOfType(markerObject, ARType.Check);
		CheckTrueScript arrowsc = checkObj.GetComponent<CheckTrueScript>();
		checkObj.SetActive(true);
	}
	
	public void ShowDescriprionBoard(GameObject objectNodeToAugment)
	{
		GameObject markerObject = GetMarkerObjectToAugment(objectNodeToAugment);
		Debug.Log(markerObject.name + " is marker?  :" + (markerObject.GetComponent<MarkerData>()!=null));
		GameObject boardObj = GetARObjectOfType(markerObject, ARType.Board);
		DescriptionBoardScript boardsc = boardObj.GetComponent<DescriptionBoardScript>();
		boardsc.SetData();
		boardObj.SetActive(true);
	}

#region Private Method
	
	/* find AR object from marker gameobject, marker may have child */
	private GameObject GetARObjectOfType(GameObject objectMarkerToAugment, ARType artype)
	{
		Debug.Log(objectMarkerToAugment.transform.childCount);
		for (int i = 0; i < objectMarkerToAugment.transform.childCount; i++)
		{
			GameObject markertrans = objectMarkerToAugment.transform.GetChild(i).gameObject;
			if(artype == ARType.Arrow && (markertrans.GetComponent<ArrowScript>() != null))
			{
				return markertrans.gameObject;
			}
			else if(artype == ARType.Check && (markertrans.GetComponent<CheckTrueScript>() != null))
			{
				return markertrans.gameObject;
			}
			else if(artype == ARType.Board && (markertrans.GetComponent<DescriptionBoardScript>() != null))
			{
				return markertrans.gameObject;
			}
		}
		return null;
	}

	/* find marker from child of node. if that node have marker and marker has 3 child 
	return marker gameObject*/
	private GameObject GetMarkerObjectToAugment(GameObject objectNodeToAugment)
	{
		foreach (Transform markertrans in objectNodeToAugment.transform)
		{
			if(markertrans.childCount == 3)
			{
				return markertrans.gameObject;
			}
		}
		return objectNodeToAugment.transform.GetChild(0).gameObject; //or return null
	}

	/* plus marker orientation to local euler rotation
	for arrow */
	private void PlusMarkerOrientation(GameObject augmentObject)
	{
		augmentObject.transform.localEulerAngles = new Vector3(
			augmentObject.transform.localEulerAngles.x,
			augmentObject.transform.localEulerAngles.y,
			augmentObject.transform.localEulerAngles.z 
				+ augmentObject.transform.parent.gameObject.GetComponent<MarkerData>().markerOrientation
		);
	}
#endregion

}
