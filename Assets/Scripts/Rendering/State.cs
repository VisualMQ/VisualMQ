using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class State : MonoBehaviour
{
    private const float UPDATE_INTERVAL = 10.0f;
    private const int DISTANCE_BETWEEN_QMS = 10;
    private float updateCountdown = UPDATE_INTERVAL;
    public GameObject updateTimeText;

    // Main dictionary keeping all connection and their rendered counterparts
    public Dictionary<MQ.Client, GameObject> qmgrs = new Dictionary<MQ.Client, GameObject>();

    // Data structure for pre-computing dependencies between different MQ components
    public DependencyGraph dependencyGraph = new DependencyGraph();


    // Use this method for adding new MQ connections (ie connections to different Qmgrs)
    public void AddNewMqClient(MQ.Client newMqClient)
    {
        qmgrs.Add(newMqClient, null);
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
                if (entry.Value == null)
                {
                    newMqClient = entry.Key;
                    break;
                }
            }
            MQ.QueueManager newQmgr = newMqClient.GetQmgr();
            newQmgr.queues = newMqClient.GetAllQueues();
            newQmgr.channels = newMqClient.GetAllChannels();
            newQmgr.applications = newMqClient.GetAllApplications();

            // Render queue manager. Note that data is stored in Component (ie Script) not in GameObject!
            // GameObject is just an Entity/Container for Components that perform the real functionality
            GameObject qmgrGameObject = new GameObject(newQmgr.qmgrName, typeof(QueueManager));
            qmgrGameObject.transform.parent = this.transform;

            QueueManager qmgrComponent = qmgrGameObject.GetComponent(typeof(QueueManager)) as QueueManager;
            Vector3 position = GetNextQueueMangagerPosition();
            qmgrComponent.baseLoc = position;
            qmgrComponent.queueManager = newQmgr;

            qmgrs[newMqClient] = qmgrGameObject;
            
            dependencyGraph.CreateDependencyGraph(newQmgr.queues, newQmgr.channels, newQmgr.applications, newQmgr.qmgrName); //Create Dependency Graphs

            return;
        }

        // Otherwise periodically update all Queue managers
        updateCountdown -= Time.deltaTime;

        if (updateCountdown <= 0)
        {
            // Show update time information
            Text updateTimeTextComponent = updateTimeText.GetComponent<Text>();
            string nowTime = DateTime.Now.ToString().Substring(9);
            updateTimeTextComponent.text = "Last update time: " + nowTime;

            foreach (KeyValuePair<MQ.Client, GameObject> entry in qmgrs)
            {
                MQ.Client mqClient = entry.Key;
                GameObject renderedQmgr = entry.Value;

                List<MQ.Queue> queues = mqClient.GetAllQueues();
                List<MQ.Channel> channels = mqClient.GetAllChannels();
                List<MQ.Application> applications = mqClient.GetAllApplications();

                QueueManager qmgrComponent = renderedQmgr.GetComponent(typeof(QueueManager)) as QueueManager;
                qmgrComponent.UpdateQueues(queues);
                qmgrComponent.UpdateChannels(channels);
            }
            updateCountdown = UPDATE_INTERVAL;
        }

    }


    // Get names of all connected queue managers
    public List<string> GetRegisteredQueueManagers()
    {
        List<string> result = new List<string>();
        foreach (MQ.Client client in qmgrs.Keys)
        {
            result.Add(client.GetQueueManagerName());
        }
        return result;
    }


    /*
     * Below are methods that get information about the MQ system.
     * Since we are working in a high cohesion and loosely coupled
     * fashion the State component is responsible for communication
     * with the MQ REST API and other entities should retrieve
     * information through this State component.
     */
    public MQ.QueueManager GetQueueManagerDetails(string selectedQMName)
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


    public List<MQ.Queue> GetAllQueues(string qmgr)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == qmgr)
            {
                return client.GetAllQueues();
            }
        }
        return null;
    }

    
    public MQ.Queue GetQueueDetails(string qmgr, string queue)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == qmgr)
            {
                return client.GetQueue(queue);
            }
        }
        return null;
    }


    public List<MQ.Message> GetAllMessages(string qmgr, string queue)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == qmgr)
            {
                return client.GetAllMessages(queue);
            }
        }
        return null;
    }


    public MQ.Channel GetChannelDetails(string qmgr, string channel)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == qmgr)
            {
                return client.GetChannel(channel);
            }
        }
        return null;
    }


    public MQ.Application GetApplicationDetails(string qmgr, string application)
    {
        foreach (MQ.Client client in qmgrs.Keys)
        {
            if (client.GetQueueManagerName() == qmgr)
            {
                return client.GetApplication(application);
            }
        }
        return null;
    }


    /*
     * This method is used to find the next position where to render a queue manager.
     * We are rendering them in two paralel lines:
     * 
     *     ------      -------
     *     |QM2 |      |QM4  |       ....
     *     ------      -------
     *     
     *     ------      ----------
     *     |QM1 |      |QM3     |     ....
     *     |    |      |        |
     *     ------      ----------
     * 
     */
    public Vector3 GetNextQueueMangagerPosition()
    {
        // Method FindObjectsOfType returns qmgrs in chronological order how they
        // were added from newest to oldest. By the time the method is called,
        // it already finds the new queue manager that was added to index 0.
        QueueManager[] renderedQMs = FindObjectsOfType<QueueManager>();
        int numberOfRenderedQMs = renderedQMs.Length;

        Vector3 result = new Vector3();
        if (numberOfRenderedQMs <= 1)
        {
            return result;
        }
        else if (numberOfRenderedQMs % 2 == 0)
        {
            QueueManager lastQM = renderedQMs[1];
            result = lastQM.baseLoc;
            result.z = result.z + lastQM.GetQueueManagerSize(true)[1] + DISTANCE_BETWEEN_QMS;
        }
        else
        {
            QueueManager lastQM = renderedQMs[2];
            result = lastQM.baseLoc;
            result.x = result.x + lastQM.GetQueueManagerSize(true)[0] + DISTANCE_BETWEEN_QMS;
        }
        return result;
    }

}
