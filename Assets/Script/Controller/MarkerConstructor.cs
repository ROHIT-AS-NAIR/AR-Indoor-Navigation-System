using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class MarkerConstructor : MonoBehaviour
{

    public List<DraftMarkerData> draftMarkerList;
    private GameObject[] allNodeList;
    private bool mChipsObjectCreated = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        IEnumerable<TrackableBehaviour> trackableBehaviours = TrackerManager.Instance.GetStateManager().GetActiveTrackableBehaviours();

        // Loop over all TrackableBehaviours.
        foreach (TrackableBehaviour trackableBehaviour in trackableBehaviours)
        {
            //check that marker not has markerdata. 
            if (trackableBehaviour.gameObject.GetComponent<MarkerData>() == null)
            {
                foreach (DraftMarkerData dm in draftMarkerList)
                {
                    //check name that match draftmarkerlist and add markerdata.
                    if (dm.markerImageName == trackableBehaviour.TrackableName)
                    {
                        trackableBehaviour.gameObject.AddComponent<MarkerData>();
                        MarkerData tmd = trackableBehaviour.gameObject.GetComponent<MarkerData>();
                        tmd.markerID = dm.markerID;
                        tmd.markerImageName = dm.markerImageName;
                        tmd.priority = dm.priority;
                        tmd.fkNodeID = dm.fkNodeID;
                        trackableBehaviour.gameObject.name = "Marker"+tmd.markerID;
                        Debug.Log(" Add MarkerData to " + trackableBehaviour.TrackableName);

                        //attract marker object to node
                        string nodeStructureTag = "Node";
                        if(allNodeList == null)
                        {
                            allNodeList = GameObject.FindGameObjectsWithTag(nodeStructureTag);
                        }
                        foreach (GameObject tObj in allNodeList)
                        {
                            if (nodeStructureTag + tmd.fkNodeID == tObj.name)
                            {
                                trackableBehaviour.gameObject.transform.SetParent(tObj.transform);
                                trackableBehaviour.gameObject.transform.localPosition = Vector3.zero;
                                Debug.Log("Attrach to "+ tObj.name);
                                break;
                            }
                        }
                        draftMarkerList.Remove(dm); //remove for best performance in next loop
                        break;
                    }
                }
            }

            //attrach using AR (note that it is update loop)

            
            string name = trackableBehaviour.TrackableName;

            if (name.Equals("chips") && !mChipsObjectCreated)
            {
                // chips target detected for the first time
                // augmentation object has not yet been created for this target
                // let's create it
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                // attach cube under target
                cube.transform.parent = trackableBehaviour.transform;

                // Add a Trackable event handler to the Trackable.
                // This Behaviour handles Trackable lost/found callbacks.
                trackableBehaviour.gameObject.AddComponent<DefaultTrackableEventHandler>();

                // set local transformation (i.e. relative to the parent target)
                cube.transform.localPosition = new Vector3(0, 0.2f, 0);
                cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                cube.transform.localRotation = Quaternion.identity;
                cube.gameObject.SetActive(true);

                mChipsObjectCreated = true;
            }
        }
    }

    public void AddDraftMarker(DraftMarkerData draftmarker)
    /* add draft data from jsonReader */
    {
        if (draftMarkerList == null)
        {
            draftMarkerList = new List<DraftMarkerData>();
        }
        Debug.Log("Add marker " + draftmarker.markerID);
        draftMarkerList.Add(draftmarker);
    }
}
