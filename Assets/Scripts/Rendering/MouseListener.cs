using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseListener : MonoBehaviour
{
    // Sidebars for different entities
    public static QueueDetailsController QueueDetailWindow;
    public static ChannelDetailsController channelDetailsWindow;
    public static ApplicationDetailsController applicationDetailsWindow;

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
        // If user clicks on UI objects, Return: Avoid Click through UI
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
        } else if (TryGetComponent(out Channel _))
        {
            Channel channel = gameObject.GetComponent<Channel>();
            channelDetailsWindow.GetChannelDetails(transform.parent.name, channel.channel.channelName);
        }
        else if (TryGetComponent(out Application _))
        {
            Application application = gameObject.GetComponent<Application>();
            applicationDetailsWindow.GetApplicationDetails(transform.parent.name, application.application.conn);
        }
       
        Debug.Log("I hit " + this.name + " !");

        // Highlight Functionality
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

        state.BroadcastMessage("DisableHighlight", SendMessageOptions.DontRequireReceiver); // disable any previous highlights

        state.BroadcastMessage("HighlightSelf", this.name, SendMessageOptions.DontRequireReceiver); // highlight the clicked object
        state.BroadcastMessage("HighlightDirect", directDependency, SendMessageOptions.DontRequireReceiver); // highlight direct dependency
        state.BroadcastMessage("HighlightIndirect", indirectDenpendency, SendMessageOptions.DontRequireReceiver); // highlight indirect dependency
    }
}
