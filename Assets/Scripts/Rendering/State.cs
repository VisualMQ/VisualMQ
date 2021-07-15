using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class State : MonoBehaviour
{
    // TODO: Change, right now dynamic updates do not work with Queue areas
    private const float UPDATE_INTERVAL = 10.0f;
    private float updateCountdown = UPDATE_INTERVAL;

    // Main dictionary keeping all connection and their rendered counterparts
    private Dictionary<MQ.Client, GameObject> qmgrs = new Dictionary<MQ.Client, GameObject>();

    // Counter for recording the number of rendered QM
    private int renderedQueueManagers = 0;
    // Integer array for recording the postion of the last rendered manager
    private List<Vector3> QMsPosition = new List<Vector3>();
    // Scale factor of blocks
    private const int blockSize = 4;
    // Distance between two QMs
    private const int QMsDistance = 10;


    // Use this method for adding new Mq connections (aka connections to different Qmgrs)
    public void AddNewMqClient(MQ.Client newMqClient)
    {
        qmgrs.Add(newMqClient, null);
    }

    
    void Start()
    {
        
    }


    void Update()
    {
        // If there are no MQ client and no Qmgrs rendered, there is nothing to update
        if (qmgrs.Count == 0) return;

        // If we have a new MQ connection that has not been rendered yet, render it
        // This relies on the fact that adding new Qmgr for visualisation can be
        // achieved by adding (MQ.Client, null) via the AddNewMqClient method
        if (qmgrs.ContainsValue(null))
        {
            Debug.Log("Rendering new Qmgr.");

            // Get data
            MQ.Client newMqClient = null;
            foreach (KeyValuePair<MQ.Client, GameObject> entry in qmgrs)
            {
                if (entry.Value == null) {
                    newMqClient = entry.Key;
                    break;
                }

            }
            MQ.QueueManager newQmgr = newMqClient.GetQmgr();
            List<MQ.Queue> newQueues = newMqClient.GetAllQueues();
            List<MQ.Channel> newChannels = newMqClient.GetAllChannels();

            // Render queue manager. Note that data is stored in Component (ie Script) not in GameObject!
            // GameObject is just an Entity/Container for Components that perform the real functionality

            GameObject qmgrGameObject = new GameObject(newQmgr.qmgrName, typeof(QueueManager));

            QueueManager qmgrComponent = qmgrGameObject.GetComponent(typeof(QueueManager)) as QueueManager;
            qmgrComponent.queues = newQueues;
            qmgrComponent.channels = newChannels;
            List<int[]> areas = GetLargeSmallArea(newMqClient);
            qmgrComponent.largeArea = areas[0];
            qmgrComponent.smallArea = areas[1];
            // Get the rendering position according its order
            Vector3 position = GetQueueMangagerPosition(renderedQueueManagers);
            Debug.Log("New position"+position);
            qmgrComponent.baseLoc = position;
            qmgrs[newMqClient] = qmgrGameObject;

            qmgrGameObject.transform.parent = this.transform;
            // Counter for QMs ++
            renderedQueueManagers++;

            return;
        }


        // Otherwise periodically update all Queue managers
        updateCountdown -= Time.deltaTime;
        if (updateCountdown <= 0)
        {
            //Debug.Log("Updating state...");

            foreach (KeyValuePair<MQ.Client, GameObject> entry in qmgrs)
            {
                MQ.Client mqClient = entry.Key;
                GameObject renderedQmgr = entry.Value;

                List<MQ.Queue> queues = mqClient.GetAllQueues();
                QueueManager qmgrComponent = renderedQmgr.GetComponent(typeof(QueueManager)) as QueueManager;
                qmgrComponent.UpdateQueues(queues);

            }

            updateCountdown = UPDATE_INTERVAL;
        }

    }


    /*
    * For Information Panel
    */

    // Get Number of QM Registered -> For Navigation Check box
    public int GetNumberOfRegisteredQM()
    {
        return qmgrs.Count; 
    }

    
    // Get MQ Name List -> For Navigation Check box
    public List<string> RegisteredQMNameList()
    {
        List<string> mqlist = new List<string>();

        foreach (MQ.Client client in qmgrs.Keys) 
        {
            mqlist.Add(client.GetQueueManagerName());
        }

        return mqlist;
    }


    // Return the details of selected QM
    public MQ.QueueManager GetSelectedQmgr(string selectedQMName)
    {
        foreach (MQ.Client client in qmgrs.Keys) 
        {
            if (client.GetQueueManagerName() == selectedQMName)
            {
                return client.GetQmgr();
            }
        }
        return null;
    }


    // Return List of queues for the table of queues of a QM
    public List<MQ.Queue> GetAllQueuesInQmgr(string selectedQMName)
    {
        List<string> queuesList = new List<string>();

        foreach (MQ.Client client in qmgrs.Keys) 
        {
            if (client.GetQueueManagerName() == selectedQMName)
            {
                return client.GetAllQueues();
            }
        }
        return null;
    }

    // Return the detail of one queue
    public MQ.Queue GetQueueDetails(string selectedQMName, string selectedQueueName)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == selectedQMName)
            {
                foreach (MQ.Queue queue in client.GetAllQueues())
                {
                    if (queue.queueName == selectedQueueName)
                    {
                        return client.GetQueue(selectedQueueName);
                    }
                }
            }
        }
        return null;
    }

    // Return All messages under current QM and queue
    public List<MQ.Message> GetAllMessages(string selectedQMName, string selectedQueueName)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == selectedQMName)
            {
                foreach (MQ.Queue queue in client.GetAllQueues())
                {
                    if (queue.queueName == selectedQueueName)
                    {
                        return client.GetAllMessages(selectedQueueName);
                    }
                }
            }
        }
        return null;
    }

    // Return the message
    public MQ.Message GetMessage(string selectedQMName, string selectedQueueName, string messageID)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == selectedQMName)
            {
                foreach (MQ.Queue queue in client.GetAllQueues())
                {
                    if (queue.queueName == selectedQueueName)
                    {
                        foreach (MQ.Message message in client.GetAllMessages(queue.queueName))
                        {
                            if (message.messageId == messageID)
                            {
                                return message;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }


    public Vector3 GetQueueMangagerPosition(int renderedQueueManagers)
    {
        List<MQ.Client> qmgrsList=qmgrs.Keys.ToList();
        Vector3 result = new Vector3(0,0,0);
        if (renderedQueueManagers==0)
        {
            QMsPosition.Add(result);
            return result;
        }
        if ((renderedQueueManagers+1)%2==0)
        {
            int[] lastQMSize = GetQueueManagerSize(qmgrsList[renderedQueueManagers-1]);
            result.x=QMsPosition[renderedQueueManagers-1].x+lastQMSize[0]+QMsDistance;
            result.y=QMsPosition[renderedQueueManagers-1].y;
            result.z=QMsPosition[renderedQueueManagers-1].z;
        }else{
            int[] lastQMSize = GetQueueManagerSize(qmgrsList[renderedQueueManagers-2]);
            result.x=QMsPosition[renderedQueueManagers-2].x;
            result.y=QMsPosition[renderedQueueManagers-2].y;
            result.z=QMsPosition[renderedQueueManagers-2].z+lastQMSize[1]+QMsDistance;
        }

        QMsPosition.Add(result);

        return result;
    }


    public List<int[]> GetLargeSmallArea(MQ.Client manager)
    {
        List<int[]> res = new List<int[]>();
        List<MQ.Queue> queues = manager.GetAllQueues();
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

        List<KeyValuePair<string, int>> numberOfQueuesList = numberOfQueues.ToList();
        numberOfQueuesList.Sort((x, y) => x.Value.CompareTo(y.Value));

        KeyValuePair<string, int> highestQueueType = numberOfQueuesList[3];
        KeyValuePair<string, int> secondSmallestQueueType = numberOfQueuesList[1];

        int[] largeArea = QueueManager.ComputeRectangleArea(highestQueueType.Value);
        int[] smallArea = QueueManager.ComputeRectangleArea(secondSmallestQueueType.Value, largeArea[1]);

        res.Add(largeArea);
        res.Add(smallArea);

        return res;


    }

    public int[] GetQueueManagerSize(MQ.Client manager)
    {
        List<int[]> areas = GetLargeSmallArea(manager);

        // Length in x axe
        int x = blockSize*(areas[0][0]+areas[1][1]);
        // Length in z axe "+1" is for channel length
        int y = blockSize*(areas[0][1]+areas[1][0]+1);

        int[] size = new int[] {x,y};
        return size;
    }

}

