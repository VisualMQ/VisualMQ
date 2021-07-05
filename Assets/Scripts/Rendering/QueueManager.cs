using System;
using System.Collections.Generic;
using UnityEngine;


public class QueueManager : MonoBehaviour
{

    public List<MQ.Queue> queues;
    private Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();

    private GameObject blockPrefab;


    // Unity calls this method at the complete beginning, even before Start
    void Awake()
    {
        blockPrefab = Resources.Load("Block") as GameObject;
    }


    void Start()
    {
        Debug.Log("Rendering " + queues.Count + " queues.");

        // Render Qmgr plane from blocks
        for (int x = 0; x < queues.Count; ++x)
        {   
            Instantiate(blockPrefab, new Vector3(2.5f*x, 0, 0), Quaternion.identity);
        }

        // Render inidividual queues
        for (int x = 0; x < queues.Count; ++x)
        {
            MQ.Queue queue = queues[x];
            GameObject queueGameObject = new GameObject(queue.queueName, typeof(Queue));
            Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
            queueComponent.position = new Vector3(2.5f*x, 0.25f, 0);
            queueComponent.queue = queue;
            // TEST CODE DELETE LATER
            if (queue is MQ.LocalQueue)
            {
                Debug.Log(queue.queueName + " currently has " + ((MQ.LocalQueue)queue).currentDepth + " messages.");
            } else if (queue is MQ.TransmissionQueue)
            {
                Debug.Log(queue.queueName + " currently has " + ((MQ.TransmissionQueue)queue).currentDepth + " messages.");
            }
        renderedQueues.Add(queue.queueName, queueGameObject);
        }
    }


    void Update()
    {

    }


    // This method is called from State object on the periodical update
    public void UpdateQueues(List<MQ.Queue> queues)
    {
        List<string> queuesToRender = new List<string>();
        List<string> queuesToDestroy = new List<string>();

        // First check which queues are not rendered yet
        foreach (MQ.Queue queue in queues)
        {
            queuesToRender.Add(queue.queueName);

            if (!renderedQueues.ContainsKey(queue.queueName))
            {
                // Render block on which the queue will reside on
                Instantiate(blockPrefab, new Vector3(2.5f * renderedQueues.Count, 0, 0), Quaternion.identity);
                // Render new queue
                GameObject queueGameObject = new GameObject(queue.queueName, typeof(Queue));
                Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
                queueComponent.position = new Vector3(2.5f * renderedQueues.Count, 0.25f, 0);
                queueComponent.queue = queue;

                renderedQueues.Add(queue.queueName, queueGameObject);
            }
        }

        // Secondly check which queues need to be destroyed
        foreach (KeyValuePair<string, GameObject> entry in renderedQueues)
        {
            if (!queuesToRender.Contains(entry.Key))
            {
                Debug.Log("Deleting queue " + entry.Key);
                GameObject.DestroyImmediate(entry.Value);
                queuesToDestroy.Add(entry.Key);
            }
        }

        foreach (string queueName in queuesToDestroy)
        {
            renderedQueues.Remove(queueName);
        }

    }

}
