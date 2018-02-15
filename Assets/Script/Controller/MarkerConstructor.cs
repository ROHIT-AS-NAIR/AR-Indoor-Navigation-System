using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class MarkerConstructor : MonoBehaviour {

	private bool mChipsObjectCreated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		IEnumerable<TrackableBehaviour> trackableBehaviours = TrackerManager.Instance.GetStateManager().GetActiveTrackableBehaviours();
  
        // Loop over all TrackableBehaviours.
        foreach (TrackableBehaviour trackableBehaviour in trackableBehaviours)
        {
            string name = trackableBehaviour.TrackableName;
            Debug.Log ("Trackable name: " + name);
             
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
                cube.transform.localPosition = new Vector3(0,0.2f,0);
                cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                cube.transform.localRotation = Quaternion.identity;
                cube.gameObject.SetActive(true);
                 
                mChipsObjectCreated = true;
            }
        }
	}
}
