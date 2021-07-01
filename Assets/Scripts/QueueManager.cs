using System;
using System.Collections.Generic;
using UnityEngine;


public class QueueManager : MonoBehaviour
{

    public MQ.QueueManager queueManager;
    public List<MQ.Queue> queues;
    public List<GameObject> renderedQueues = new List<GameObject>();

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

            renderedQueues.Add(queueGameObject);
        }
    }

    void Update()
    {

        
    }

}
