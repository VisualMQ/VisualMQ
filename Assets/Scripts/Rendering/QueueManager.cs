/*
The QueueManager GameObjects live one level below the State Object
It controls where to position every entity that lives on the
QueueManager plane, as well as the dynamic update to reflect latest
status of the Qmgr on the Cloud

A queue manager has 2 large rectangle areas and 2 small rectangle areas and 1 lane
at the bottom. Those 4 rectangles are for four different types of queues, and the 1
lane is for placing channels.

z   -----------
^   |2    |4  |
    |     |   |
|   X-----X----
    |1    |3  |
|   |     |   |
    X-----X----
|   |         |
    O----------
|
 -  -  -  -  -  -  > x axis

We sort types of queues (Alias/Remote..) based on the number of queues
of that type there are. Two highest have the 2 large areas, two smallest
have the 2 small areas.
The large area is formed based on the queue with the highest number of
queues, the small are based on the queue type with the second smallest number
of queues.

Each area has associated offset vector (showed as X) and dimensions.
The queue manager has a baseLoc showed as O on the diagram.

Coordinates are expressed in number of queues/channels, but need to be multiplied
by a scaling factor sXZ (scale in X and Z dimension). On the other hand cY is a constant
offset in the Y dimension corresponding to the height of a queue manager.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QueueManager : MonoBehaviour
{
    // Prefabs
    private GameObject blockPrefab;
    private GameObject linePrefab;

    // Scale factor of blocks in different axes
    private const float sXZ = 4f;
    private const float cY = 0.1287538f;
    private const string QM_NAME_DELIMITER = ".";
    public const string QM_NAME_PREFIX = "Plane";

    public MQ.QueueManager queueManager;

    public Vector3 baseLoc;
    public Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> renderedChannels = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> renderedApplications = new Dictionary<string, GameObject>();

    public Dictionary<string, Vector3> offsets;
    public Dictionary<string, int> numberOfRenderedQueues;
    public Dictionary<string, int[]> dimensions;

    public int[] planeSizes;
    public int[] largeArea;
    public int[] smallArea;


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

        Dictionary<string, int> numberOfQueues = GetNumberOfQueuesOfType();
        List<KeyValuePair<string, int>> numberOfQueuesList = numberOfQueues.ToList();
        numberOfQueuesList.Sort((x, y) => x.Value.CompareTo(y.Value));

        List<int[]> areas = GetLargeSmallArea();
        largeArea = areas[0];
        smallArea = areas[1];
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
        planeSizes = GetQueueManagerSize(false);
        Vector3 queueManagerCenter = baseLoc + new Vector3(planeSizes[0], 0, planeSizes[1]) / 2;

        GameObject planeGameObject = new GameObject(QM_NAME_PREFIX + QM_NAME_DELIMITER + qmName, typeof(HighlightRenderer), typeof(MouseListener));
        planeGameObject.transform.parent = this.transform;
        planeGameObject.transform.position = queueManagerCenter;
        planeGameObject.transform.localScale = new Vector3(planeSizes[0] / sXZ, 1, planeSizes[1] / sXZ);
        GameObject planePrefab = Instantiate(blockPrefab, planeGameObject.transform) as GameObject;
        planePrefab.transform.name = QM_NAME_PREFIX + QM_NAME_DELIMITER + qmName + ".Prefab";
        planePrefab.transform.localScale = new Vector3(1, 1, 1);
        planePrefab.transform.localPosition = Vector3.zero;
        MeshCollider mc = planeGameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = planePrefab.GetComponent<MeshFilter>().sharedMesh;


        // Create 3 line separating individual queue areas
        GameObject lineX1 = Instantiate(linePrefab, new Vector3(queueManagerCenter.x - baseLoc.x, cY, (largeArea[1] + 1) * sXZ) + baseLoc, Quaternion.Euler(0f, 90f, 0f));
        lineX1.transform.parent = this.transform;
        lineX1.transform.localScale += new Vector3(0, 0, largeArea[0] + smallArea[1] - 1);
        GameObject lineX2 = Instantiate(linePrefab, new Vector3(queueManagerCenter.x - baseLoc.x, cY, sXZ) + baseLoc, Quaternion.Euler(0f, 90f, 0f));
        lineX2.transform.parent = this.transform;
        lineX2.transform.localScale += new Vector3(0, 0, largeArea[0] + smallArea[1] - 1);
        GameObject lineZ1 = Instantiate(linePrefab, sXZ * new Vector3(largeArea[0], cY, largeArea[1] + 1) + baseLoc, Quaternion.identity);
        lineZ1.transform.parent = this.transform;
        lineZ1.transform.localScale += new Vector3(0, 0, 2 * largeArea[1] - 1);


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
        RenderChannels(channels);

        // Render applications
        RenderApplications(applications);
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
            if (Math.Abs(b - a) <= 2)
            {
                stopFlag = true;
            }
            else a++;
        }

        // We need extra space for remainder queues and for dynamic updates
        // Notice that it has to be the case that b >= a
        int[] res = { b + 1, a };
        return res;
    }


    // Returns dimensions of a rectangle that can hold N queues but
    // one side of the rectangle is constrained to size a
    public static int[] ComputeRectangleArea(int N, int a)
    {
        int b = N / a;
        int[] res = { Math.Max(a, b + 1), Math.Min(a, b + 1) };
        return res;
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
        z = (int)sXZ * (2 * areas[0][1] + 1);

        return new int[] { x, z };
    }


    // This method is called from State object on the periodical update
    public bool UpdateQueues(List<MQ.Queue> queues)
    {
        List<string> queuesToRender = new List<string>();
        List<string> queuesToDestroy = new List<string>();
        bool modified = false;

        // First: check which queues are not rendered yet
        foreach (MQ.Queue queue in queues)
        {
            queuesToRender.Add(queue.queueName);

            if (!renderedQueues.ContainsKey(queue.queueName))
            {
                modified = true;
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

                if (oldDepth != newDepth)
                {
                    queueComponent.UpdateMessages(newDepth);
                }
            }
        }

        return modified;

    }


    public bool UpdateChannels(List<MQ.Channel> channels)
    {   
        bool modified = false;
        // Check if the number of channels changed
        if(channels.Count == renderedChannels.Count)
        {
            // If the number not changed, check whether all the channels are not changed
            for(int i=0; i<channels.Count; i++)
            {
                if(!renderedChannels.ContainsKey(channels[i].channelName))
                {
                    modified = true;
                    break;
                }
            }
        }
        else
        {
            modified = true;
        }

        // Re-render with lastest channels if modified == true
        if(modified)
        {   
            // Destroy all the channels, in order to relocate and resize the channels
            foreach (KeyValuePair<string, GameObject> entry in renderedChannels)
            {
                GameObject.DestroyImmediate(entry.Value);
            }
            RenderChannels(channels);
        }

        return modified;
    }


    public void RenderChannels(List<MQ.Channel> channels)
    {
        renderedChannels.Clear();

        for (int i = 0; i < channels.Count; i++)
        {
            MQ.Channel channel = channels[i];
            string uniqueChannelName = queueManager.qmgrName + QM_NAME_DELIMITER + channel.channelName;
            GameObject channelGameObject = new GameObject(uniqueChannelName, typeof(Channel));
            Channel channelComponent = channelGameObject.GetComponent(typeof(Channel)) as Channel;
            channelComponent.channel = channel;
            channelGameObject.transform.parent = this.transform;

            // Position channels dynamically depending on how many channels there are
            // If there are more channels, we need to scale down the spaces between them
            if (channels.Count > largeArea[0] + smallArea[1])
            {
                channelGameObject.transform.position = new Vector3(((float)planeSizes[0] / (float)channels.Count) * (i + 0.5f), cY * 2, sXZ * 0.5f) + baseLoc;
            }
            else
            {
                channelGameObject.transform.position = new Vector3(sXZ * (i + 0.5f), cY, sXZ * 0.5f) + baseLoc;
            }

            NameRenderer nameRenderer = channelGameObject.GetComponent(typeof(NameRenderer)) as NameRenderer;
            nameRenderer.objectName = channel.channelName;

            renderedChannels.Add(channel.channelName, channelGameObject);
        }
    }


    public bool UpdateApplications(List<MQ.Application> applications)
    {
        bool modified = false;
        // Check if the number of channels changed
        if (applications.Count == renderedApplications.Count)
        {
            // If the number not changed, check whether all the channels are not changed
            for (int i = 0; i < applications.Count; i++)
            {
                if (!renderedApplications.ContainsKey(applications[i].conn))
                {
                    modified = true;
                    break;
                }
            }
        }
        else
        {
            modified = true;
        }

        // Re-render with lastest applications if modified == true
        if (modified)
        {
            // Destroy all old application gameobjects
            foreach (KeyValuePair<string, GameObject> entry in renderedApplications)
            {
                GameObject.DestroyImmediate(entry.Value);
            }
            RenderApplications(applications);
        }

        return modified;
    }


    public void RenderApplications(List<MQ.Application> applications)
    {
        renderedApplications.Clear();

        for (int i = 0; i < applications.Count; i++)
        {
            MQ.Application application = applications[i];
            string uniqueConnectionName = queueManager.qmgrName + QM_NAME_DELIMITER + application.conn;
            GameObject applicationGameObject = new GameObject(uniqueConnectionName, typeof(Application));
            Application applicationComponent = applicationGameObject.GetComponent((typeof(Application))) as Application;
            applicationComponent.application = application;
            applicationGameObject.transform.position = sXZ * new Vector3(-(i / (2 * largeArea[1]) + 1), 0, i % (2 * largeArea[1]) + 1.5f) + baseLoc;
            applicationGameObject.transform.parent = this.transform;

            NameRenderer nameComponent = applicationGameObject.GetComponent(typeof(NameRenderer)) as NameRenderer;
            nameComponent.objectName = application.conn;

            renderedApplications.Add(application.conn, applicationGameObject);
        }
    }


    public UnityEngine.Vector3 ComputePosition(string queueType, int rank)
    {
        // int i = numberOfRenderedQueues[queueType]++;
        Vector3 offset = offsets[queueType];
        Vector3 position = new Vector3(sXZ * (rank % dimensions[queueType][0]), 0, sXZ * (rank / dimensions[queueType][0]));
        Vector3 queueManagerHeight = new Vector3(0, cY, 0);
        return (offset + position + queueManagerHeight + baseLoc);
    }

}
