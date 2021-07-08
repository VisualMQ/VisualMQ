using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QueueManager : MonoBehaviour
{

    private GameObject blockPrefab;
    private GameObject linePrefab;

    // Scale factor of blocks
    private const float sXZ = 4f;
    private const float sY = 0.1286252f;

    public List<MQ.Queue> queues;
    private Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();

    
    // Unity calls this method at the complete beginning, even before Start
    void Awake()
    {
        blockPrefab = Resources.Load("Prefabs/Block") as GameObject;
        linePrefab = Resources.Load("Prefabs/Line") as GameObject;
    }


    void Start()
    {
        Debug.Log("Rendering " + queues.Count + " queues.");

        // Compute how many queues are of each type
        Dictionary<string, int> numberOfQueues = new Dictionary<string, int>();
        numberOfQueues[MQ.AliasQueue.typeName] = 0;
        numberOfQueues[MQ.RemoteQueue.typeName] = 0;
        numberOfQueues[MQ.TransmissionQueue.typeName] = 0;
        numberOfQueues[MQ.LocalQueue.typeName] = 0;
        foreach (MQ.Queue queue in queues)
        {
            numberOfQueues[queue.GetTypeName()]++;
        }

        // We will have 2 large rectangle areas and 2 small rectangle areas
        List<KeyValuePair<string, int>> numberOfQueuesList = numberOfQueues.ToList();
        numberOfQueuesList.Sort((x, y) => x.Value.CompareTo(y.Value));

        KeyValuePair<string, int> highestQueues = numberOfQueuesList[3];
        KeyValuePair<string, int> secondSmallestQueues = numberOfQueuesList[1];

        int[] largeArea = ComputeRectangleArea(highestQueues.Value);
        int[] smallArea = ComputeRectangleArea(secondSmallestQueues.Value, largeArea[1]);
        Debug.Assert(largeArea[1] == smallArea[0]);
        Debug.Log("large area dimensions " + largeArea[0] + " " + largeArea[1]);
        Debug.Log("small area dimensions " + smallArea[0] + " " + smallArea[1]);


        Dictionary<string, Vector3> offsets = new Dictionary<string, Vector3>();
        Dictionary<string, int[]> dimensions = new Dictionary<string, int[]>();
        offsets[numberOfQueuesList[3].Key] = new Vector3(0, 0, 0);
        offsets[numberOfQueuesList[2].Key] = new Vector3(0, 0, sXZ * largeArea[1]);
        offsets[numberOfQueuesList[1].Key] = new Vector3(sXZ * largeArea[0], 0, 0);
        offsets[numberOfQueuesList[0].Key] = new Vector3(sXZ * largeArea[0], 0, sXZ * largeArea[1]);
        dimensions[numberOfQueuesList[3].Key] = largeArea;
        dimensions[numberOfQueuesList[2].Key] = largeArea;
        dimensions[numberOfQueuesList[1].Key] = smallArea;
        dimensions[numberOfQueuesList[0].Key] = smallArea;

        // Render 2 large areas on top of each other
        for (int x = 0; x < largeArea[0]; x++)
        {
            for (int z = 0; z < largeArea[1]; z++)
            {
                // Large area
                GameObject lowerBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z), Quaternion.identity);
                lowerBlock.transform.parent = this.transform;

                // Large area
                GameObject upperBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + offsets[numberOfQueuesList[2].Key], Quaternion.identity);
                upperBlock.transform.parent = this.transform;
            }
        }
        // Render 2 small areas
        for (int x = 0; x < smallArea[1]; x++)
        {
            for (int z = 0; z < smallArea[0]; z++)
            {
                // Small area
                GameObject lowerBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + offsets[numberOfQueuesList[1].Key], Quaternion.identity);
                lowerBlock.transform.parent = this.transform;

                // Small area
                GameObject upperBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + offsets[numberOfQueuesList[0].Key], Quaternion.identity);
                upperBlock.transform.parent = this.transform;
            }
        }

        // Render lines between areas among X-axis
        for (int x = 0; x < largeArea[0] + smallArea[1]; x++)
        {
            GameObject line = Instantiate(linePrefab, new Vector3(sXZ * x, sY * 1.001f, -2) + offsets[numberOfQueuesList[2].Key], Quaternion.Euler(0f, 90f, 0f));
            line.transform.parent = this.transform;
        }
        // Render lines between areas among Z-axis
        for (int z = 0; z < largeArea[1] + smallArea[0]; z++)
        {
            GameObject line = Instantiate(linePrefab, new Vector3(-2, sY * 1.001f, sXZ * z) + offsets[numberOfQueuesList[1].Key], Quaternion.identity);
            line.transform.parent = this.transform;
        }

        // TODO: Add text on blocks
        // Problem right now is that text is rendered with awful resolution
        // I don't really know the cause for that
        //foreach (KeyValuePair<string, Vector3> entry in offsets)
        //{
        //    GameObject textName = new GameObject();
        //    TextMesh textMesh = textName.AddComponent<TextMesh>() as TextMesh;
        //    textMesh.text = entry.Key;
        //    textMesh.anchor = TextAnchor.MiddleCenter;
        //    textMesh.alignment = TextAlignment.Center;
        //    textMesh.color = Color.black;
        //    textMesh.fontSize = 6;
        //    textMesh.transform.Rotate(90, 0, 0);
        //    textMesh.transform.position = entry.Value + new Vector3(-0.3f, 0.01f, -1.5f);
        //}



        // Render inidividual queues
        Dictionary<string, int> numberOfRenderedQueues = new Dictionary<string, int>();
        numberOfRenderedQueues[MQ.AliasQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.RemoteQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.TransmissionQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.LocalQueue.typeName] = 0;
        foreach (MQ.Queue queue in queues)
        {
            string queueType = queue.GetTypeName();
            int i = numberOfRenderedQueues[queueType]++;
            Vector3 offset = offsets[queueType];
            Vector3 position = new Vector3(sXZ * (i % dimensions[queueType][0]), 0, sXZ * (i / dimensions[queueType][0]));
            Vector3 queueManagerHeight = new Vector3(0, sY*2, 0);

            GameObject queueGameObject = new GameObject(queue.queueName, typeof(Queue));
            Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
            queueComponent.position = offset + position + queueManagerHeight;
            queueComponent.queue = queue;
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


    // Returns dimensions of a rectangle that can hold N queues
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
        int[] res = { cols + 1, rows };
        return res;
    }


    // Returns dimensions of a rectangle that can hold N queues but
    // one size of the rectangle is constrained to rows
    private static int[] ComputeRectangleArea(int N, int rows)
    {
        int cols = N / rows;
        int[] res = { Math.Max(rows, cols + 1), Math.Min(rows, cols + 1) };
        return res;
    }

}
