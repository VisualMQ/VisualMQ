using UnityEngine;
using System.Collections.Generic;
using System;
using MQ;

public class Queue : MonoBehaviour
{
    public Vector3 position;
    public MQ.Queue queue;
    public GameObject messagePrefab;
    public List<GameObject> messages;
    
    void Awake()
    {
        messagePrefab = Resources.Load("Prefabs/Message") as GameObject;
        messages = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {
        string prefabName;
        if (queue is MQ.RemoteQueue) 
        {
            prefabName = "Prefabs/RemoteQueue";
        }
        else if (queue is MQ.LocalQueue)
        {
            prefabName = "Prefabs/LocalQueue";
        }
        else if (queue is MQ.TransmissionQueue)
        {
            prefabName = "Prefabs/TransmissionQueue";
        }
        else if (queue is MQ.AliasQueue)
        {
            prefabName = "Prefabs/AliasQueue";
        }
        else
        {
            prefabName = "Prefabs/LocalQueue"; //TODO: undefined queue
        }
        GameObject queuePrefab = Resources.Load(prefabName) as GameObject;
        GameObject instantiatedQueue = Instantiate(queuePrefab, position, Quaternion.identity) as GameObject;
        instantiatedQueue.transform.parent = this.transform;

        if (queue.holdsMessages)
        {
            int currentDepth = queue.currentDepth;
            CreateMessages(currentDepth, queue.maxNumberOfMessages); //TODO: change 10 to actual maximumDepth

        }
    }

    void CreateMessages(int currentDepth, int MaximumDepth)
    {
        if (currentDepth == 0) return;

        double utilization = (double)currentDepth / (double)MaximumDepth;
        string messageColor;
        if (utilization > 0.6) //determine what color to render the message prefabs
        {
            messageColor = "Materials/QueueRed";
        }
        else if (utilization < 0.4)
        {
            messageColor = "Materials/QueueBlue";
        }
        else
        {
            messageColor = "Materials/QueueYellow";
        }

        int messagePrefabNum = (int)Math.Ceiling(utilization * 20);
        for (int i = 0; i < messagePrefabNum; i++)
        {
            Vector3 messagePosition = position;
            messagePosition.y = position.y + (i + 1) * 0.2f;
            GameObject instantiatedMessage = Instantiate(messagePrefab, messagePosition, Quaternion.identity) as GameObject;
            instantiatedMessage.transform.parent = this.transform;
            instantiatedMessage.GetComponent<MeshRenderer>().material = Resources.Load(messageColor) as Material;

            messages.Add(instantiatedMessage);
        }
    }

    public void UpdateMessages(int newDepth)
    {
        // First: destroy all old messages
        for (int i = 0; i < messages.Count; i++)
        {
            GameObject.DestroyImmediate(messages[i]);
        }

        // Second: Update to new messages
        if (queue.holdsMessages)
        {
            queue.currentDepth = newDepth;
            CreateMessages(newDepth, queue.maxNumberOfMessages); //TODO: change 10 to actual maximumDepth
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
