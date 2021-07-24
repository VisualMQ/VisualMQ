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

    public string qmName;
    public List<MQ.Queue> queues;
    public List<MQ.Channel> channels;
    public Vector3 baseLoc;
    public Dictionary<string, GameObject> renderedQueues = new Dictionary<string, GameObject>();
    public Dictionary<MQ.Queue, GameObject> MQQueueObjectQueue = new Dictionary<MQ.Queue, GameObject>();

    public Dictionary<string, Vector3> offsets;
    public Dictionary<string, int> numberOfRenderedQueues;
    public Dictionary<string, int[]> dimensions;

    public static QMDetailsController QMDetailWindow;

    public GameObject blockParent;


    // Unity calls this method at the complete beginning, even before Start
    void Awake()
    {
        blockPrefab = Resources.Load("Prefabs/Block") as GameObject;
        linePrefab = Resources.Load("Prefabs/Line") as GameObject;
    }

    void Start()
    {
        Debug.Log("Rendering " + queues.Count + " queues.");

        Dictionary<string, int> numberOfQueues = GetNumberOfQueuesOfType();
        List<KeyValuePair<string, int>> numberOfQueuesList = numberOfQueues.ToList();
        numberOfQueuesList.Sort((x, y) => x.Value.CompareTo(y.Value));

        List<int[]> areas = GetLargeSmallArea();
        int[] largeArea = areas[0];
        int[] smallArea = areas[1];
        // By design, larger side of smallArea is equal to smaller side of largeArea
        // See the diagram
        Debug.Assert(largeArea[1] == smallArea[0]);
        offsets = new Dictionary<string, Vector3>();
        dimensions = new Dictionary<string, int[]>();
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
                GameObject lowerBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + baseLoc, Quaternion.identity) as GameObject;
                lowerBlock.name = qmName + QM_NAME_DELIMITER + lowerBlock.name;
                lowerBlock.transform.parent = this.transform;

                lowerBlock.name = "Block."+qmName;
                lowerBlock.AddComponent<MeshCollider>().convex = true;
                lowerBlock.AddComponent(typeof(QMClicked));

                /*
                // Add mesh Colider
                MeshCollider mc = lowerBlock.transform.parent.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = lowerBlock.GetComponent<MeshFilter>().sharedMesh; */

                // Large area
                GameObject upperBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + baseLoc + offsets[numberOfQueuesList[2].Key], Quaternion.identity) as GameObject;
                upperBlock.name = qmName + QM_NAME_DELIMITER + upperBlock.name;
                upperBlock.transform.parent = this.transform;

                upperBlock.name = "Block."+qmName;
                upperBlock.AddComponent<MeshCollider>().convex = true;
                upperBlock.AddComponent(typeof(QMClicked));

            }
        }

        // Render 2 small areas
        for (int x = 0; x < smallArea[1]; x++)
        {
            for (int z = 0; z < smallArea[0]; z++)
            {
                // Small area
                GameObject lowerBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + baseLoc + offsets[numberOfQueuesList[1].Key], Quaternion.identity) as GameObject;
                lowerBlock.name = qmName + QM_NAME_DELIMITER + lowerBlock.name;
                lowerBlock.transform.parent = this.transform;

                lowerBlock.name = "Block."+qmName;
                lowerBlock.AddComponent<MeshCollider>().convex = true;
                lowerBlock.AddComponent(typeof(QMClicked));


                // Small area
                GameObject upperBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, sXZ * z) + baseLoc + offsets[numberOfQueuesList[0].Key], Quaternion.identity) as GameObject;
                upperBlock.name = qmName + QM_NAME_DELIMITER + upperBlock.name;
                upperBlock.transform.parent = this.transform;

                upperBlock.name = "Block."+qmName;
                upperBlock.AddComponent<MeshCollider>().convex = true;
                upperBlock.AddComponent(typeof(QMClicked));
            }
        }

        // Render lines between areas among X-axis
        for (int x = 0; x < largeArea[0] + smallArea[1]; x++)
        {
            GameObject line = Instantiate(linePrefab, new Vector3(sXZ * x, sY * 1.001f, -2) + baseLoc + offsets[numberOfQueuesList[2].Key], Quaternion.Euler(0f, 90f, 0f));
            line.transform.parent = this.transform;

            GameObject line2 = Instantiate(linePrefab, new Vector3(sXZ * x, sY * 1.001f, -2) + baseLoc, Quaternion.Euler(0f, 90f, 0f));
            line2.transform.parent = this.transform;
        }

        // Render lines between areas among Z-axis
        for (int z = 0; z < largeArea[1] + smallArea[0]; z++)
        {
            GameObject line = Instantiate(linePrefab, new Vector3(-2, sY * 1.001f, sXZ * z) + baseLoc + offsets[numberOfQueuesList[1].Key], Quaternion.identity);
            line.transform.parent = this.transform;
        }

      	// TODO: There is a bug that text is on top of everything
        //foreach (KeyValuePair<string, Vector3> entry in offsets)
        //{
        //   GameObject textName = new GameObject();
        //   TextMesh textMesh = textName.AddComponent<TextMesh>() as TextMesh;
        //   textMesh.text = entry.Key;
        //   textMesh.anchor = TextAnchor.MiddleCenter;
        //   textMesh.alignment = TextAlignment.Center;
        //   textMesh.color = Color.black;
        //   textMesh.fontSize = 24;
        //   textMesh.characterSize = 0.25f;
        //   textMesh.transform.Rotate(90, 0, 0);
        //   textMesh.transform.position = entry.Value + new Vector3(-0.3f, 0.01f, -1.5f) + baseLoc;
        //}

        // Render inidividual queues
        numberOfRenderedQueues = new Dictionary<string, int>();
        numberOfRenderedQueues[MQ.AliasQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.RemoteQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.TransmissionQueue.typeName] = 0;
        numberOfRenderedQueues[MQ.LocalQueue.typeName] = 0;
        foreach (MQ.Queue queue in queues)
        {
            string queueType = queue.GetTypeName();
            int i = numberOfRenderedQueues[queueType]++;

            string uniqueQueueName = qmName + QM_NAME_DELIMITER + queue.queueName;
            GameObject queueGameObject = new GameObject(uniqueQueueName, typeof(Queue)); //Globally unique queue name
            Queue queueComponent = queueGameObject.GetComponent(typeof(Queue)) as Queue;
            queueComponent.rank = i;
                
            queueComponent.queue = queue;

            queueComponent.parent = this;
            queueGameObject.transform.parent = this.transform;
            // queueComponent.repositionSelf();
            // TODO: REMOVE?
            renderedQueues.Add(queue.queueName, queueGameObject);
            MQQueueObjectQueue.Add(queue, queueGameObject);

        }

        // Render area for channels and channels on them
        for (int x = 0; x < largeArea[0] + smallArea[1]; x++)
        {
            GameObject lowerBlock = Instantiate(blockPrefab, new Vector3(sXZ * x, 0, -sXZ) + baseLoc, Quaternion.identity);
            lowerBlock.name = "Block."+qmName;
            lowerBlock.transform.parent = this.transform;
            lowerBlock.AddComponent<MeshCollider>().convex = true;
        }

        // TODO: There is a bug that text is on top of everything
        //GameObject textNameC = new GameObject();
        //TextMesh textMesh1 = textNameC.AddComponent<TextMesh>() as TextMesh;
        //textMesh1.text = "Channels";
        //textMesh1.anchor = TextAnchor.MiddleCenter;
        //textMesh1.alignment = TextAlignment.Center;
        //textMesh1.color = Color.black;
        //textMesh1.fontSize = 24;
        //textMesh1.characterSize = 0.25f;
        //textMesh1.transform.Rotate(90, 0, 0);
        //textMesh1.transform.position = new Vector3(-0.3f, 0.01f, -1.5f-sXZ) + baseLoc;

        int numberOfSenderChannels = 0;
        int numberOfReceiverChannels = 0;
        foreach (MQ.Channel channel in channels)
        {
            
            Vector3 queueManagerHeight = new Vector3(0, sY * 2, 0);
            Vector3 position = new Vector3();
            if (channel is MQ.SenderChannel)
            {
                int i = numberOfSenderChannels++;
                position = new Vector3(sXZ * i, 0, -sXZ) + baseLoc;
            }
            else if (channel is MQ.ReceiverChannel)
            {
                int j = numberOfReceiverChannels++;
                position = new Vector3(sXZ * (largeArea[0] + smallArea[1] - j - 1), 0, -sXZ) + baseLoc;
            }

            string uniqueChannelName = qmName + QM_NAME_DELIMITER + channel.channelName;
            GameObject channelGameObject = new GameObject(uniqueChannelName, typeof(Channel)); //Globally unique channel name
            Channel channelComponent = channelGameObject.GetComponent(typeof(Channel)) as Channel;
            channelComponent.position = position + queueManagerHeight;
            channelComponent.channel = channel;
            channelGameObject.transform.parent = this.transform;
        }
    }

    public UnityEngine.Vector3 ComputePosition(string queueType, int rank)
    {
        // int i = numberOfRenderedQueues[queueType]++;
        Vector3 offset = offsets[queueType];
        Vector3 position = new Vector3(sXZ * (rank % dimensions[queueType][0]), 0, sXZ * (rank / dimensions[queueType][0]));
        Vector3 queueManagerHeight = new Vector3(0, sY * 2, 0);
        Debug.Log("POSITIONING RANK" + rank);
        return (offset + position + queueManagerHeight + baseLoc);  
    }


    void Update()
    {
        // Check for RIGHT mouse input
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Casts the ray and get the first game object hit
            if (Physics.Raycast(ray, out hit)) {
                string objectname = hit.transform.name;
                string objectType = objectname.Substring(0, 5);
                string qmName = objectname.Substring(6);
                if (objectType == "Block")
                {
                    Debug.Log("QM Block is clicked: "+qmName);
                    // use the qmname trigger the qm detail window
                    QMDetailWindow.QueueManagerInfoInit(qmName);
                }
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            topView();
        }
    }


    void topView()
    {
        GameObject mainCamera = GameObject.Find("Main Camera");
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.transform.rotation = Quaternion.AngleAxis(90, new Vector3(1, 0, 0));
        Vector3 targetPosition = this.transform.position;
        targetPosition.y += 20f;
        targetPosition.x += 15f;
        targetPosition.z += 5f;
        mainCamera.transform.position =  targetPosition;
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

                string uniqueQueueName = qmName + QM_NAME_DELIMITER + queue.queueName;
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
           
        // TODO THIS DOES NOT WORK YET
        return;
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
        foreach (MQ.Queue queue in queues)
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


    public int[] GetQueueManagerSize()
    {
        List<int[]> areas = GetLargeSmallArea();

        // Length in x axe
        int x = (int) sXZ * (areas[0][0] + areas[1][1]);
        // Length in z axe "+1" is for channel length
        int y = (int) sXZ * (areas[0][1] + areas[1][0] + 1);

        int[] size = new int[] { x, y };
        return size;
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
