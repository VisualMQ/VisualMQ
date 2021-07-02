using System;
using System.Collections.Generic;
using UnityEngine;


public class QueueManager : MonoBehaviour
{



    public MQ.QueueManager queueManager;
    public List<MQ.Queue> queues;
    private Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();
    private int numQueues = 0;

    void Start()
    {
        Debug.Log(queues.Count);

        for (int x = 0; x < queues.Count; ++x)
        {
            GameObject blockPrefab = Resources.Load("Block") as GameObject;
            Instantiate(blockPrefab, new Vector3(2.5f*x, 0, 0), Quaternion.identity);
        }

        for (int x = 0; x < queues.Count; ++x)
        {
            MQ.Queue queue = queues[x];
            GameObject queueGameObject = new GameObject(queue.queueName, typeof(Queue));
            Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
            queueComponent.position = new Vector3(2.5f*x, 0.25f, 0);
            queueComponent.queue = queue;

            renderedQueues.Add(queue.queueName, queueGameObject);
            numQueues++;
        }
    }

    void Update()
    {



    }


    public void UpdateQueues(List<MQ.Queue> queues)
    {
        List<string> queuesToRender = new List<string>();
        List<string> queuesToDestroy = new List<string>();

        foreach (MQ.Queue queue in queues)
        {
            queuesToRender.Add(queue.queueName);

            if (!renderedQueues.ContainsKey(queue.queueName))
            {
                // Render block
                GameObject blockPrefab = Resources.Load("Block") as GameObject;
                Instantiate(blockPrefab, new Vector3(2.5f * numQueues, 0, 0), Quaternion.identity);

                // Render new queue
                GameObject queueGameObject = new GameObject(queue.queueName, typeof(Queue));
                Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
                queueComponent.position = new Vector3(2.5f * numQueues, 0.25f, 0);
                queueComponent.queue = queue;

                renderedQueues.Add(queue.queueName, queueGameObject);
                numQueues++;
            }
        }

        foreach (KeyValuePair<string, GameObject> item in renderedQueues)
        {
            if (!queuesToRender.Contains(item.Key))
            {
                Debug.Log("Attempt to delete queue " + item.Key);
                GameObject.DestroyImmediate(item.Value);
                queuesToDestroy.Add(item.Key);
            }
        }
        foreach (string queueName in queuesToDestroy)
        {
            renderedQueues.Remove(queueName);
        }
        



    }

}
