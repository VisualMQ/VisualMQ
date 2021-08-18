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
        // Do not zoom in/out the queue manager plane
        if (gameObject.transform.name.StartsWith(QueueManager.QM_NAME_PREFIX))
        {
            return;
        }

        gameObject.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);   
    }


    // Hover functionality
    void OnMouseExit()
    {
        // Do not zoom in/out the queue manager plane
        if (gameObject.transform.name.StartsWith(QueueManager.QM_NAME_PREFIX))
        {
            return;
        }

        gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }


    // Click functionality
    void OnMouseUp()
    {
        // If user clicks on UI objects, Return: Avoid Click through UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        /*
         *  Sidebar with details functionality
         */
        if (TryGetComponent(out Queue _))
        {
            sidebarController.ShowQueueDetails(transform.parent.name, name);
        }
        else if (TryGetComponent(out Channel _))
        {
            Channel channel = gameObject.GetComponent<Channel>();
            sidebarController.ShowChannelDetails(transform.parent.name, channel.channel.channelName);
        }
        else if (TryGetComponent(out Application _))
        {
            Application application = gameObject.GetComponent<Application>();
            sidebarController.ShowApplicationDetails(transform.parent.name, application.application.conn);
        }
        else if (gameObject.transform.name.StartsWith(QueueManager.QM_NAME_PREFIX))
        {
            string qmgrName = gameObject.transform.name.Substring(QueueManager.QM_NAME_PREFIX.Length + 1);
            sidebarController.ShowQueueManagerDetails(qmgrName);
            gameObject.BroadcastMessage("HighlightSelf", gameObject.transform.name);
            // Queue manager does not have any dependencies so just return
            return;
        }


        /*
         *  Highlight functionality
         */
        State state = GameObject.Find("State").GetComponent<State>(); //Might be time consuming operation
        List<string> directDependency;
        List<string> indirectDenpendency;

        state.dependencyGraph.directDependencies.TryGetValue(this.name, out directDependency);
        state.dependencyGraph.indirectDependencies.TryGetValue(this.name, out indirectDenpendency);

        // If no dependency found, initialize to empty list
        // Because passing null value = zero argument to the function broadcasted
        if (directDependency == null)
        {
            directDependency = new List<string>();
        }
        if (indirectDenpendency == null)
        {
            indirectDenpendency = new List<string>();
        }

        // HighlightRendered components that each rendered entity has associated
        // will responds appropriately to these broadcasted messages
        state.BroadcastMessage("DisableHighlight", SendMessageOptions.DontRequireReceiver);
        state.BroadcastMessage("HighlightSelf", this.name, SendMessageOptions.DontRequireReceiver);
        state.BroadcastMessage("HighlightDirect", directDependency, SendMessageOptions.DontRequireReceiver);
        state.BroadcastMessage("HighlightIndirect", indirectDenpendency, SendMessageOptions.DontRequireReceiver);
    }

}
