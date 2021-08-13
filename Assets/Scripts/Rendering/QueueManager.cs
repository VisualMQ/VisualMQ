using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class QueueManager : MonoBehaviour
{
    // Prefabs
    private GameObject blockPrefab;
    private GameObject linePrefab;

    // Scale factor of blocks in different axes
    private const float sXZ = 4f;
    private const float sY = 0.1286252f;
    private const string QM_NAME_DELIMITER = ".";

    public MQ.QueueManager queueManager;

    public Vector3 baseLoc;
    public Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();

    public Dictionary<string, Vector3> offsets;
    public Dictionary<string, int> numberOfRenderedQueues;
    public Dictionary<string, int[]> dimensions;

    public static QueueManagerDetailsController QMDetailWindow;

    public GameObject blockParent;


 


    // Unity calls this method at the complete beginning, even before Start
    void Awake()
    {
        blockPrefab = Resources.Load("Prefabs/Block") as GameObject;
        linePrefab = Resources.Load("Prefabs/Line") as GameObject;
    }

    void Start()
    {
        List<MQ.Queue> queues = queueManager.queues;
        List<MQ.Channel> channels = queueManager.channels;
        List<MQ.Application> applications = queueManager.applications;
        string qmName = queueManager.qmgrName;

        
        Debug.Log("Rendering " + queues.Count + " queues.");

        Dictionary<string, int> numberOfQueues = GetNumberOfQueuesOfType();
        List<KeyValuePair<string, int>> numberOfQueuesList = numberOfQueues.ToList();
        numberOfQueuesList.Sort((x, y) => x.Value.CompareTo(y.Value));

        List<int[]> areas = GetLargeSmallArea();
        int[] largeArea = areas[0];
        int[] smallArea = areas[1];
        // By design, larger side of smallArea is equal to smaller side of largeArea
        Debug.Assert(largeArea[1] == smallArea[0]);
        offsets = new Dictionary<string, Vector3>();
        dimensions = new Dictionary<string, int[]>();
        offsets[numberOfQueuesList[3].Key] = sXZ * new Vector3(0.5f, 0, 1.5f);
        offsets[numberOfQueuesList[2].Key] = sXZ * new Vector3(0.5f, 0, largeArea[1] + 1.5f);
        offsets[numberOfQueuesList[1].Key] = sXZ * new Vector3(largeArea[0] + 0.5f, 0, 1.5f);
        offsets[numberOfQueuesList[0].Key] = sXZ * new Vector3(largeArea[0] + 0.5f, 0, largeArea[1] + 1.5f);
        dimensions[numberOfQueuesList[3].Key] = largeArea;
        dimensions[numberOfQueuesList[2].Key] = largeArea;
        dimensions[numberOfQueuesList[1].Key] = smallArea;
        dimensions[numberOfQueuesList[0].Key] = smallArea;


        // Create queue manager plane
        int[] planeSizes = GetQueueManagerSize(false);
        Vector3 queueManagerCenter = baseLoc + (new Vector3(planeSizes[0], 0, planeSizes[1]) / 2);

        GameObject queueManagerPlane = Instantiate(blockPrefab, queueManagerCenter, Quaternion.identity) as GameObject;
        queueManagerPlane.name = qmName + QM_NAME_DELIMITER + queueManagerPlane.name;
        queueManagerPlane.transform.parent = this.transform;
        queueManagerPlane.transform.localScale = new Vector3(planeSizes[0] / sXZ, 1, planeSizes[1] / sXZ);

        // Create 3 line separating individual queue areas
        GameObject line = Instantiate(linePrefab, new Vector3(queueManagerCenter.x, sY * 1.001f, (largeArea[1] + 1) * sXZ) + baseLoc, Quaternion.Euler(0f, 90f, 0f));
        line.transform.parent = this.transform;
        line.transform.localScale += new Vector3(0, 0, largeArea[0] + smallArea[1] - 1);

        line = Instantiate(linePrefab, new Vector3(queueManagerCenter.x, sY * 1.001f, sXZ) + baseLoc, Quaternion.Euler(0f, 90f, 0f));
        line.transform.parent = this.transform;
        line.transform.localScale += new Vector3(0, 0, largeArea[0] + smallArea[1] - 1);

        line = Instantiate(linePrefab, sXZ * new Vector3(largeArea[0], sY * 1.001f, largeArea[1] + 1) + baseLoc, Quaternion.identity);
        line.transform.parent = this.transform;
        line.transform.localScale += new Vector3(0, 0, 2 * largeArea[1] - 1);


        // Render inidividual queues
        numberOfRenderedQueues = new Dictionary<string, int>();
        numberOfRenderedQueues[MQ.AliasQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.RemoteQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.TransmissionQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.LocalQueue.typeName] = 0;
        foreach (MQ.Queue queue in queues)
        {
            string queueType = queue.GetTypeName();
            string uniqueQueueName = qmName + QM_NAME_DELIMITER + queue.queueName;

            GameObject queueGameObject = new GameObject(uniqueQueueName, typeof(Queue)); //Globally unique queue name
            Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
            queueComponent.rank = numberOfRenderedQueues[queueType]++;
            queueComponent.queue = queue;
            queueComponent.parent = this;
            queueGameObject.transform.parent = this.transform;

            NameRenderer nameRenderer = queueGameObject.GetComponent(typeof(NameRenderer)) as NameRenderer;
            nameRenderer.objectName = queue.queueName;

            renderedQueues.Add(queue.queueName, queueGameObject);
        }


        // Render channels
        for (int i = 0; i < channels.Count; i++)
        {
            MQ.Channel channel = channels[i];
            string uniqueChannelName = qmName + QM_NAME_DELIMITER + channel.channelName;
            GameObject channelGameObject = new GameObject(uniqueChannelName, typeof(Channel));
            Channel channelComponent = channelGameObject.GetComponent(typeof(Channel)) as Channel;
            channelComponent.channel = channel;
            channelGameObject.transform.position = new Vector3(sXZ * (i + 0.5f), sY * 2, sXZ * 0.5f) + baseLoc;
            channelGameObject.transform.parent = this.transform;

            NameRenderer nameRenderer = channelGameObject.GetComponent(typeof(NameRenderer)) as NameRenderer;
            nameRenderer.objectName = channel.channelName;
        }


        // Render applications
        for (int i = 0; i < applications.Count; i++)
        {
            MQ.Application application = applications[i];
            string uniqueConnectionName = qmName + QM_NAME_DELIMITER + application.conn;
            GameObject applicationGameObject = new GameObject(uniqueConnectionName, typeof(Application));
            Application applicationComponent = applicationGameObject.GetComponent((typeof(Application))) as Application;
            applicationComponent.application = application;
            applicationGameObject.transform.position = sXZ * new Vector3(-(i / (2 * largeArea[1] + 1) + 1), 0, i % (2 * largeArea[1]) + 1.5f) + baseLoc;
            applicationGameObject.transform.parent = this.transform;

            NameRenderer nameComponent = applicationGameObject.GetComponent(typeof(NameRenderer)) as NameRenderer;
            nameComponent.objectName = application.conn;
        }
    }

    public UnityEngine.Vector3 ComputePosition(string queueType, int rank)
    {
        // int i = numberOfRenderedQueues[queueType]++;
        Vector3 offset = offsets[queueType];
        Vector3 position = new Vector3(sXZ * (rank % dimensions[queueType][0]), 0, sXZ * (rank / dimensions[queueType][0]));
        Vector3 queueManagerHeight = new Vector3(0, sY * 2, 0);
        return (offset + position + queueManagerHeight + baseLoc);  
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
                string queueType = queue.GetTypeName();
                int i = numberOfRenderedQueues[queueType]++;

                string uniqueQueueName = queueManager.qmgrName + QM_NAME_DELIMITER + queue.queueName;
                GameObject queueGameObject = new GameObject(uniqueQueueName, typeof(Queue));
                Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
                queueComponent.rank = i;


                queueComponent.queue = queue;

                queueComponent.parent = this;
                // TODO: REMOVE?
                renderedQueues.Add(queue.queueName, queueGameObject);

                queueGameObject.transform.parent = this.transform;

                gameObject.BroadcastMessage("newQueueAdded", i);

            }
        }

        // Second: check which queues need to be destroyed
        foreach (KeyValuePair<string, GameObject> entry in renderedQueues)
        {
            if (!queuesToRender.Contains(entry.Key))
            {
                Debug.Log("Deleting queue " + entry.Key);

                Queue queueComponent = entry.Value.GetComponent(typeof(Queue)) as Queue;
                int rank = queueComponent.rank;
                string queueType = queueComponent.queue.GetTypeName();
                numberOfRenderedQueues[queueType]--;

                GameObject.DestroyImmediate(entry.Value);
                queuesToDestroy.Add(entry.Key);

                gameObject.BroadcastMessage("newQueueDeleted", rank);
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


    public Dictionary<string, int> GetNumberOfQueuesOfType()
    {
        // Compute how many queues are of each type
        Dictionary<string, int> numberOfQueues = new Dictionary<string, int>
        {
            [MQ.AliasQueue.typeName] = 0,
            [MQ.RemoteQueue.typeName] = 0,
            [MQ.TransmissionQueue.typeName] = 0,
            [MQ.LocalQueue.typeName] = 0
        };
        foreach (MQ.Queue queue in queueManager.queues)
        {
            numberOfQueues[queue.GetTypeName()]++;
        }
        return numberOfQueues;
    }


    /* We will have 2 large rectangle areas and 2 small rectangle areas
       -----------
       |2    |4  |
       |     |   |
       X-----X----
       |1    |3  |
       |     |   |
       X-----X----

        We sort types of queues (Alias/Remote..) based on the number of queues
        of that type there are. Two highest have the 2 large areas, two smallest
        have the 2 small areas.
        The large area is formed based on the queue with the highest number of
        queues, the small are based on the queue type with the second smallest number
        of queues.
    */

    /*
        Each area has associated offset vector and dimension.
        Offset determines its position. See offsets variable later on.
        On diagram the offset is represented by X.
        Dimensions/areas are 2-element int array specifying the 2 rectangle axes sizes.
        The first element of the int array is always the greater one.
    */
    public List<int[]> GetLargeSmallArea()
    {
        List<int[]> result = new List<int[]>();
        Dictionary<string, int> numberOfQueues = GetNumberOfQueuesOfType();

        List<KeyValuePair<string, int>> numberOfQueuesList = numberOfQueues.ToList();
        numberOfQueuesList.Sort((x, y) => x.Value.CompareTo(y.Value));

        KeyValuePair<string, int> highestQueueType = numberOfQueuesList[3];
        KeyValuePair<string, int> secondSmallestQueueType = numberOfQueuesList[1];

        int[] largeArea = QueueManager.ComputeRectangleArea(highestQueueType.Value);
        int[] smallArea = QueueManager.ComputeRectangleArea(secondSmallestQueueType.Value, largeArea[1]);

        result.Add(largeArea);
        result.Add(smallArea);

        return result;
    }


    public int[] GetQueueManagerSize(bool includeApplication)
    {
        List<int[]> areas = GetLargeSmallArea();

        // Length in x axe
        int x, z;

        if (includeApplication)
        {
            x = (int)sXZ * (areas[0][0] + areas[1][1] + (queueManager.applications.Count / (2 * areas[0][1])) + 2);
        }
        else
        {
            x = (int)sXZ * (areas[0][0] + areas[1][1]);
        }
        // Length in z axis "+1" is for channel length
        z = (int) sXZ * (2 * areas[0][1] + 1);

        return new int[] { x, z };
    }


    // Our algorithm for creating a rectangle that can hold N queues
    // This method returns dimensions of that rectangle
    public static int[] ComputeRectangleArea(int N)
    {
        int a = 1;
        int b = 0;
        bool stopFlag = false;

        while (!stopFlag)
        {
            b = N / a;
            if (Math.Abs(b - a) <= 2) stopFlag = true;
            else a++;
        }

        // We need extra space for remainder queues and for dynamic updates
        // Notice that it has to be the case that b >= a
        int[] res = { b + 1, a };
        return res;
    }

    // Returns dimensions of a rectangle that can hold N queues but
    // one size of the rectangle is constrained to size a
    public static int[] ComputeRectangleArea(int N, int a)
    {
        int b = N / a;
        int[] res = { Math.Max(a, b + 1), Math.Min(a, b + 1) };
        return res;
    }

}
