using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QueueManager : MonoBehaviour
{

    public List<MQ.Queue> queues;
    private Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();

    private GameObject blockPrefab;


    // Unity calls this method at the complete beginning, even before Start
    void Awake()
    {
        blockPrefab = Resources.Load("Prefabs/Block") as GameObject;
    }


    void Start()
    {
        Debug.Log("Rendering " + queues.Count + " queues.");

        // Render Qmgr plane from blocks
        //for (int x = 0; x < queues.Count; ++x)
        //{
        //    Instantiate(blockPrefab, new Vector3(2.5f * x, 0, 0), Quaternion.identity);
        //    block.transform.parent = this.transform;
        //}
        Dictionary<string, int> numberOfQueues = new Dictionary<string, int>();
        foreach (MQ.Queue queue in queues)
        {
            if (queue is MQ.LocalQueue) numberOfQueues["local"]++;
            else if (queue is MQ.TransmissionQueue) numberOfQueues["transmission"]++;
            else if (queue is MQ.RemoteQueue) numberOfQueues["remote"]++;
            else if (queue is MQ.AliasQueue) numberOfQueues["alias"]++;

        }



        // Render inidividual queues
        for (int x = 0; x < queues.Count; ++x)
        {
            MQ.Queue queue = queues[x];
            GameObject queueGameObject = new GameObject(queue.queueName, typeof(Queue));
            Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
            queueComponent.position = new Vector3(2.5f*x, 0.25f, 0);
            queueComponent.queue = queue;
            // DELETE LATER: TEST CODE
            if (queue is MQ.LocalQueue)
            {
                Debug.Log(queue.queueName + " currently has " + ((MQ.LocalQueue)queue).currentDepth + " messages.");
            } else if (queue is MQ.TransmissionQueue)
            {
                Debug.Log(queue.queueName + " currently has " + ((MQ.TransmissionQueue)queue).currentDepth + " messages.");
            }
            //
            renderedQueues.Add(queue.queueName, queueGameObject);

            queueGameObject.transform.parent = this.transform;
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

        // First: check which queues are not rendered yet
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
                queueGameObject.transform.parent = this.transform;
            }
        }

        // Second: check which queues need to be destroyed
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

        // Third: for local queues and transmission queues, check if the utilization level has changed
        foreach (MQ.Queue queue in queues)
        {
            if (queue.holdsMessages)
            {
                Queue queueComponent = renderedQueues[queue.queueName].GetComponent(typeof(Queue)) as Queue;
                int oldDepth = queueComponent.queue.currentDepth;
                int newDepth = queue.currentDepth;

                //TODO: Assign new message fields when message API is ready
                //TODO: Re-render might be necessary even if oldDepth = newDepth -- messages might have changed

                if (oldDepth != newDepth)
                {
                    queueComponent.UpdateMessages(newDepth);
                }
            }
        }

    }


    private Dictionary<string, int[]> GetQueueAreaDimensions(Dictionary<string, int> numberOfQueuesOfType)
    {
        List<KeyValuePair<string, int>> numberOfQueues = numberOfQueuesOfType.ToList();
        numberOfQueues.Sort((x, y) => x.Value.CompareTo(y.Value));

        KeyValuePair<string, int> highestQueues = numberOfQueues[3];
        KeyValuePair<string, int> secondSmallestQueues = numberOfQueues[1];

        int[] largeArea = ComputeRectangleArea(highestQueues.Value);
        int[] smallArea = ComputeRectangleArea(secondSmallestQueues.Value, largeArea[0]);

        Dictionary<string, int[]> result = new Dictionary<string, int[]>();
        result[numberOfQueues[3].Key] = largeArea;
        result[numberOfQueues[2].Key] = largeArea;
        result[numberOfQueues[1].Key] = smallArea;
        result[numberOfQueues[0].Key] = smallArea;

        return result;
    }


    private int[] ComputeRectangleArea(int N)
    {
        int rows = 1;
        int cols = 0;
        bool stopFlag = false;

        while (!stopFlag)
        {
            cols = N / rows;
            if (Math.Abs(cols - rows) <= 2) stopFlag = true;
            else rows++;
        }

        // We need extra space for remainder queues and for dynamic updates
        // cols will always be higher than rows, ie cols > rows
        int[] res = { rows, cols + 1 };
        return res;
    }


    private static int[] ComputeRectangleArea(int N, int rows)
    {
        int cols = N / rows;
        int[] res = { rows, cols + 1 };
        return res;
    }

}
