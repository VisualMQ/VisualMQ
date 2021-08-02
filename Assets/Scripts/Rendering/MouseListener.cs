using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour
{
    // Sidebars for different entities
    public static QueueDetailsController QueueDetailWindow;

    //// Use this for initialization
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    // Hover functionality
    void OnMouseEnter()
    {
        gameObject.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);   
    }

    // Hover functionality
    void OnMouseExit()
    {
        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }


    // Click functionality
    void OnMouseUp()
    {
        // If user clicks on UI objects, Return: Avoid Click through
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //GameObject mainCamera = GameObject.Find("Main Camera");
        //mainCamera.transform.rotation = Quaternion.AngleAxis(70, new Vector3(1, 0, 0));
        //Vector3 targetPosition = this.transform.position;
        //targetPosition.y += 18f;
        //targetPosition.x += 5f;
        //targetPosition.z -= 5f;
        //mainCamera.transform.position = targetPosition;

        // If a queue is clicked on show Queue details window
        if (TryGetComponent(out Queue _))
        {
            List<string> temp = new List<string>() { transform.parent.name, name };
            QueueDetailWindow.GetQueueBasicInfo(temp);
        }

        // Highlight the selected object
        //var outline = gameObject.GetComponent<Outline>();
        //if (outline == null)
        //{
        //    outline = gameObject.AddComponent<Outline>();
        //    outline.OutlineMode = Outline.Mode.OutlineAll;
        //    outline.OutlineColor = Color.yellow;
        //    outline.OutlineWidth = 5f;
        //    outline.enabled = false;
        //}
        //outline.enabled = !outline.enabled;
       
        Debug.Log("I hit " + this.name + " !");

        State state = gameObject.GetComponentInParent(typeof(State)) as State;
        if (!state.dependencyGraph.graph.ContainsKey(this.name))
        {
            List<string> noDependency = new List<string>();
            noDependency.Add(this.name);
            state.BroadcastMessage("Highlight", noDependency, SendMessageOptions.DontRequireReceiver);
            return;
        }

        Debug.Log("Dependency found.");
        List<string> objectDependency = state.dependencyGraph.graph[this.name];

        objectDependency.Add(this.name);
        state.BroadcastMessage("Highlight", objectDependency, SendMessageOptions.DontRequireReceiver);

    }

}
