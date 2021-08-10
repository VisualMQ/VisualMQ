using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour
{
    // Sidebars for different entities
    public static SidebarController sidebarController;

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

        // If a queue is clicked on show Queue details window
        if (TryGetComponent(out Queue _))
        {
            List<string> temp = new List<string>() { transform.parent.name, name };
            sidebarController.ShowQueueDetails(temp);
        }
        //else if (TryGetComponent(out Channel _))
        //{
        //    Channel channel = gameObject.GetComponent<Channel>();
        //    channelDetailsWindow.GetChannelDetails(transform.parent.name, channel.channel.channelName);
        //    QueueDetailWindow.Close();
        //    applicationDetailsWindow.Close();
        //}
        //else if (TryGetComponent(out Application _))
        //{
        //    Application application = gameObject.GetComponent<Application>();
        //    applicationDetailsWindow.GetApplicationDetails(transform.parent.name, application.application.conn);
        //    QueueDetailWindow.Close();
        //    channelDetailsWindow.Close();
        //}
       
        Debug.Log("I hit " + this.name + " !");

        State state = GameObject.Find("State").GetComponent<State>(); //Might be time consuming operation
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
