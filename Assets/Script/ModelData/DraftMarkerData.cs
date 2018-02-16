using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraftMarkerData {

	public string markerID;
    public string markerImageName;
    public int priority;
    public string fkNodeID;

	public DraftMarkerData(string markerID, string markerImageName, int priority, string fkNodeID){
		this.markerID = markerID;
		this.markerImageName = markerImageName;
		this.priority = priority;
		this.fkNodeID = fkNodeID;
	}
}
