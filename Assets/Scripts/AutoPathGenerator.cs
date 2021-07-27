using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathCreation{
    [RequireComponent(typeof(PathCreator))]
    public class AutoPathGenerator : MonoBehaviour {

        public bool closedLoop = true;
        public Transform[] waypoints;

        void Start () {
            if (waypoints.Length > 0) {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath (waypoints, closedLoop, PathSpace.xyz);
                bezierPath.IsClosed = false;
                GetComponent<PathCreator>().bezierPath = bezierPath;
            }
        }
    }
}
