using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour
{
    private float downClickTime;
    private const float CLICK_DELTA_TIME = 0.5f;
    private Vector3 clickpo;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Check for LEFT mouse input 

        if (Input.GetMouseButtonDown(0))
        {
            downClickTime = Time.time;
            clickpo = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (Time.time - downClickTime <= CLICK_DELTA_TIME)
            {
                Ray ray = Camera.main.ScreenPointToRay(clickpo);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                if (Physics.Raycast(ray, out hit, 100))
                {
                    //string objectName = hit.transform.name;
                    //Debug.Log("I hit " + objectName + " !");

                    //State state = gameObject.GetComponent(typeof(State)) as State;
                    //if (!state.dependencyGraph.graph.ContainsKey(objectName))
                    //{
                    //    List<string> noDependency = new List<string>();
                    //    noDependency.Add(objectName);
                    //    gameObject.BroadcastMessage("Highlight", noDependency, SendMessageOptions.DontRequireReceiver);
                    //    return;
                    //}
                    //List<string> objectDependency = state.dependencyGraph.graph[objectName];

                    //objectDependency.Add(objectName);
                    //gameObject.BroadcastMessage("Highlight", objectDependency, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    Debug.Log("MISSSSSS");
                    gameObject.BroadcastMessage("Highlight", new List<string>(), SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
