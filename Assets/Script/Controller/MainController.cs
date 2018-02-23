using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{

    public static MainController instance;
    private DijsktraAlgorithm dijsktra;
    private ARDisplayController ar;
    //private CanvasButtonScript canvasButton;
    public GameObject beginPoint = null;
    public GameObject destinationPoint = null;
    public GameObject reachedPoint = null;
    private GameObject oldBeginPoint = null, oldDestinationPoint = null, oldReachePoint = null;
    public enum AppState
    {
        Idle,
        Navigate
    }

    public AppState appState = AppState.Idle;
    private AppState oldAppState = AppState.Idle;
    public bool navigatable = false; //due arcontrolscr use

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            dijsktra = new DijsktraAlgorithm();
            ar = new ARDisplayController();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /* observer zone, control state changing */
        if (appState != oldAppState)
        {
            //call observer
            oldAppState = appState;
        }
    }

    #region SetPoint

    public void SetBeginPoint(GameObject beginPoint)
    /* get node object from markerConstructor
    change appstate and point value
    process and send to display */
    {
        if (oldBeginPoint != beginPoint)
        {
            if (beginPoint.GetComponent<NodeData>() != null)
            {
                this.beginPoint = beginPoint;
                Debug.Log("Set Begin Point to room " + beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName + " @node"
                     + beginPoint.GetComponent<NodeData>().nodeID);
            }

            /* brgonpoint process part */
            #region Beginpoint Process path
            if (this.destinationPoint == null && this.reachedPoint == null)
            {
                appState = AppState.Idle;
                ar.ShowDescriprionBoard(this.beginPoint);
            }
            else if (this.destinationPoint == null && this.reachedPoint != null)
            {
                appState = AppState.Idle;
                if(IsSameRoom(this.beginPoint, this.reachedPoint))
                {
                    ar.ShowCheck(this.beginPoint);
                }
                else{
                    ar.ShowDescriprionBoard(this.beginPoint);
                }
            }
            else if (this.destinationPoint != null)
            {
                if(IsSameRoom(this.beginPoint, this.destinationPoint))
                {
                    appState = AppState.Idle;
                    this.reachedPoint = this.destinationPoint;
                    this.destinationPoint = null;
                    ar.ShowDescriprionBoard(this.beginPoint);
                }
                else
                {
                    appState = AppState.Navigate;
                    Navigate();
                    if(navigatable)
                    {
                        ar.ShowArrow(this.beginPoint, navigatable);
                    }
                    else
                    {
                        // process arrow direction
                        int beginfloor = this.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorIndex;
                        int destfloor = this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorIndex;
                        if(beginfloor < destfloor)
                        {
                            ar.ShowArrow(this.beginPoint, navigatable, ARDisplayController.ArrowDirection.Up);
                        }
                        else if(beginfloor > destfloor)
                        {
                            ar.ShowArrow(this.beginPoint, navigatable, ARDisplayController.ArrowDirection.Down);
                        }
                        else{
                            ar.ShowArrow(this.beginPoint, this.destinationPoint);
                        }
                    }
                }
            }
            //else if (this.destinationPoint != null && this.reachedPoint != null)
            #endregion
            oldBeginPoint = beginPoint;
        }
        //ShowAR(beginPoint); //remove if user can set
    }
    public void SetDestinationPoint(GameObject destinationPoint)
    {
        //    IF from room, random or calculate from shortest path from beginpoint

        //if (destinationPoint.GetComponent<MarkerData>() != null)
        //{
        this.destinationPoint = destinationPoint;
        //Debug.Log("Set Destination Point to " + destinationPoint.GetComponent<MarkerData>().roomName);
        //}
        //canvasButton.OnDestinationPointChange(destinationPoint);
        switch (appState)
        {
            case AppState.Idle:
                if (this.beginPoint != null && this.destinationPoint != null)   // already has begin point, start navigate
                {
                    NavigateIfNotSamePoint();
                }
                break;
            case AppState.Navigate:
                if (this.destinationPoint == null)
                {
                    appState = AppState.Idle;
                }
                else if (this.beginPoint != null && this.destinationPoint != null)   // already has begin point, start navigate
                {
                    NavigateIfNotSamePoint();       // select dest same as current point false
                }
                break;
            default:
                appState = AppState.Navigate;
                break;
        }
        //showState.OnDestinationPointChange(destinationPoint);
        oldDestinationPoint = destinationPoint;
    }
    public void ClearDestinationPoint()
    {
        SetDestinationPoint(null);
        this.reachedPoint = null;
        appState = AppState.Idle;
    }

    #endregion

    #region Navigate

    private bool NavigateIfNotSamePoint() //unused
    /* check before navigate that two new point 
    -didn't at destination room
    -not same node
    change state them
    return true if two point didn't same room and go to navigate*/
    {
        if (this.beginPoint != null && this.destinationPoint != null)
        {
            if (this.beginPoint.GetComponent<NodeData>().nodeID != this.oldBeginPoint.GetComponent<NodeData>().nodeID) //not old point
            {
                if (this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
                                == this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName) //is at destination room
                {
                    this.reachedPoint = this.destinationPoint;
                    this.destinationPoint = null;
                    appState = AppState.Idle;
                    return false;
                }
                // if still not destination but new point, run navigate
                appState = AppState.Navigate;
                Navigate();
            }
        }
        return true;
    }

    public void Navigate()
    /* run navigate algorithm by usion beginpoint and destpoint to update all markerdata value
	if in same floor, navigate to connecttor of floor 
	if can't find route, arrow will directly point to destination*/
    {
        if (this.beginPoint.GetComponent<NodeData>().IsSameFloorWith(this.destinationPoint))
        {
            Debug.Log("Same Floor  Navigating " + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorName);
            // if (this.beginPoint.GetComponent<NodeData>().GetParentObjectData().roomName
            //     == this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().roomName) //<<<<< Dead Code. check room name
            // {
            //     Debug.Log("=== Founded Destination ===");
            //     appState = AppState.Idle;
            //     Debug.Log(" Reach at Main navigate and idle");
            // }
            if (dijsktra.FindShortestPath(this.beginPoint, this.destinationPoint))
            {
                navigatable = true;
            }
            else
            {
                navigatable = false;
            }
        }
        else
        {
            Debug.Log("==================================== Different Floor  Navigating "
                + this.beginPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorName
                + " To " + this.destinationPoint.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().floorName);

            bool begintoLift = dijsktra.FindShortestPath(this.beginPoint, GetConnectorNode(this.beginPoint));
            Debug.Log("==================================== brk");
            bool liftToDest = dijsktra.FindShortestPath(GetConnectorNode(this.destinationPoint), this.destinationPoint);
            if (begintoLift && liftToDest)
            {
                navigatable = true;
            }
            else
            {
                navigatable = false;
            }
        }
        //case point to null of successor
    }


    #endregion

    #region ShowAR
    //function recive prefabType loop all child if met active it and flag as met, if meet more than one destroy it
    // public void ShowAR(GameObject objectToAugment)
    // /* show AR depending on state, works with ArControlScript, navigate before show */
    // {
    //     ArControlScript arControl = objectToAugment.GetComponent<ArControlScript>();
    //     foreach (Transform child in objectToAugment.transform)
    //     {
    //         child.gameObject.SetActive(false);
    //     }

    //     if (appState == AppState.Idle) //set desboard if not in old node or navigated to destination
    //     {
    //         if (this.beginPoint != null && this.reachedPoint != null)
    //         {
    //             if (objectToAugment.GetComponent<MarkerData>().roomName == this.reachedPoint.GetComponent<MarkerData>().roomName)
    //             {
    //                 arControl.CreateCheckTrue();
    //                 Debug.Log(" Reach at Main ShowAR Idle Mode and idle");
    //             }
    //             else
    //             {
    //                 arControl.CreateDescriptionBoard();
    //             }
    //         }
    //         else if (this.beginPoint != null)
    //         {
    //             arControl.CreateDescriptionBoard();
    //         }
    //     }
    //     else if (appState == AppState.Navigate)  //set arrow
    //     {
    //         arControl.CreateArrow();
    //         if (navigatable)
    //         {
    //             Debug.Log("navigatable point to " + objectToAugment.GetComponent<MarkerData>().successor.GetComponent<MarkerData>().position);
    //             arControl.GetArrow().GetComponent<ArrowScript>().PointToCoordinate(
    //                 objectToAugment.GetComponent<MarkerData>().successor.GetComponent<MarkerData>().position);
    //         }
    //         else
    //         {
    //             int beginfloor = this.beginPoint.GetComponent<MarkerData>().GetFloor().GetComponent<FloorData>().floorIndex;
    //             int destfloor = this.destinationPoint.GetComponent<MarkerData>().GetFloor().GetComponent<FloorData>().floorIndex;
    //             if (beginfloor < destfloor)
    //             {
    //                 Debug.Log("point down");
    //                 arControl.GetArrow().GetComponent<ArrowScript>().PointArrowUp();
    //             }
    //             else if (beginfloor > destfloor)
    //             {
    //                 Debug.Log("point up");
    //                 arControl.GetArrow().GetComponent<ArrowScript>().PointArrowDown();
    //             }
    //             else
    //             {
    //                 Debug.Log("ObjecttoAugmnt " + objectToAugment.name + " pointing");
    //                 arControl.GetArrow().GetComponent<ArrowScript>()
    //                         .PointToCoordinate(this.destinationPoint.GetComponent<MarkerData>().position);
    //                 objectToAugment.GetComponentInChildren<ArrowScript>()
    //                         .PointToCoordinate(this.destinationPoint.GetComponent<MarkerData>().position);
    //             }
    //         }
    //     }
    // }
    #endregion

    #region Private calculate method

    /* get connector node from nodeobject  currenly return first objectof roomobject
    return node that have room are connector */
    private GameObject GetConnectorNode(GameObject nodeObj)
    {
        //Transform[] roomlist = nodeObj.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().gameObject.transform;
        foreach (GameObject room in nodeObj.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().gameObject.transform)
        {
            if (room.GetComponent<RoomData>().isConnector)
            {
                return room.transform.GetChild(0).gameObject;
            }
        }
        // return first child of floor, room
        return nodeObj.GetComponent<NodeData>().GetParentObjectData().GetParentObjectData().gameObject.transform.GetChild(0).GetChild(0).gameObject;
    }

    /* compare roomname from node data */
    private bool IsSameRoom(GameObject node1, GameObject node2)
    {
        if (node1.GetComponent<NodeData>() != null && node2.GetComponent<NodeData>() != null)
        {
            RoomData node1roomData = node1.GetComponent<NodeData>().GetParentObjectData();
            RoomData node2roomData = node2.GetComponent<NodeData>().GetParentObjectData();
            return node1roomData.roomName == node2roomData.roomName;
        }
        return false;
    }
    #endregion
}
