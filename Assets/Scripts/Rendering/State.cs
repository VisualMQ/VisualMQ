using System;
using System.Collections.Generic;
using UnityEngine;


public class State : MonoBehaviour
{
    // TODO: Change, right now dynamic updates do not work with Queue areas
    private const float UPDATE_INTERVAL = 100000.0f;
    private float updateCountdown = UPDATE_INTERVAL;

    // Main dictionary keeping all connection and their rendered counterparts
    private Dictionary<MQ.Client, GameObject> qmgrs = new Dictionary<MQ.Client, GameObject>();


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
            qmgrs[newMqClient] = qmgrGameObject;

            qmgrGameObject.transform.parent = this.transform;

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
}

