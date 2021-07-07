using UnityEngine;
using System.Collections.Generic;
using MQ;

public class Queue : MonoBehaviour
{

    public Vector3 position;
    public MQ.Queue queue;
    public GameObject messagePrefab;
    public List<GameObject> messages;

    void Awake()
    {
        messagePrefab = Resources.Load("Message") as GameObject;
        messages = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {
        string prefabName;
        if (queue is MQ.RemoteQueue) 
        {
            prefabName = "RemoteQueue";
        }
        else if (queue is MQ.LocalQueue)
        {
            prefabName = "LocalQueue";
        }
        else if (queue is MQ.TransmissionQueue)
        {
            prefabName = "TransmissionQueue";
        }
        else if (queue is MQ.AliasQueue)
        {
            prefabName = "AliasQueue";
        }
        else
        {
            prefabName = "LocalQueue"; //TODO: undefined queue
        }
        GameObject queuePrefab = Resources.Load(prefabName) as GameObject;
        GameObject instantiatedQueue = Instantiate(queuePrefab, position, Quaternion.identity) as GameObject;
        instantiatedQueue.transform.parent = this.transform;

        int currentDepth = 0;
        if (queue is MQ.LocalQueue)
        {
            currentDepth = ((MQ.LocalQueue)queue).currentDepth;
            createMessages(currentDepth, 10); //TODO: change 10 to actual maximumDepth

        }
        else if (queue is MQ.TransmissionQueue)
        {
            currentDepth = ((MQ.TransmissionQueue)queue).currentDepth;
            createMessages(currentDepth, 10); //TODO: change 10 to actual maximumDepth
        }
    }

    void createMessages(int currentDepth, int MaximumDepth)
    {
        if (currentDepth == 0) return;

        double utilization = (double)currentDepth / (double)MaximumDepth;
        string messageColor;
        if (utilization > 0.6) //determine what color to render the message prefabs
        {
            messageColor = "QueueRed";
        }
        else if (utilization < 0.4)
        {
            messageColor = "QueueBlue";
        }
        else
        {
            messageColor = "QueueYellow";
        }

        for (int i = 0; i < currentDepth; i++)
        {
            Vector3 messagePosition = position;
            messagePosition.y = position.y + (i + 1) * 0.2f;
            GameObject instantiatedMessage = Instantiate(messagePrefab, messagePosition, Quaternion.identity) as GameObject;
            instantiatedMessage.transform.parent = this.transform;
            instantiatedMessage.GetComponent<MeshRenderer>().material = Resources.Load(messageColor) as Material;

            messages.Add(instantiatedMessage);
        }
    }

    public void updateMessages(int newDepth)
    {
        // First: destroy all old messages
        for (int i = 0; i < messages.Count; i++)
        {
            GameObject.DestroyImmediate(messages[i]);
        }

        // Second: Update to new messages
        if (queue is MQ.LocalQueue)
        {
            ((MQ.LocalQueue)queue).currentDepth = newDepth;
            createMessages(newDepth, 10); //TODO: change 10 to actual maximumDepth

        }
        else if (queue is MQ.TransmissionQueue)
        {
            ((MQ.TransmissionQueue)queue).currentDepth = newDepth;
            createMessages(newDepth, 10); //TODO: change 10 to actual maximumDepth
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
