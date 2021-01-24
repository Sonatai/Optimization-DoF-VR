using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/*
 * Draws Path of agent
 */
[RequireComponent(typeof(LineRenderer))]
public class PathNaviVisualization : MonoBehaviour {

    [SerializeField]
    private NavMeshAgent agentToDebug;
    private LineRenderer lineRender;

	// Use this for initialization
	void Start () {
        lineRender = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {

    if (agentToDebug.hasPath) {

            lineRender.positionCount = agentToDebug.path.corners.Length;
            lineRender.SetPositions (agentToDebug.path.corners);
            lineRender.enabled = true;

        } else {
            lineRender.enabled = false;
        }
    }
}
