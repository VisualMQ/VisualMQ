using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class State : MonoBehaviour
{
    private const float UPDATE_INTERVAL = 10.0f;
    private const string QM_NAME_DELIMITER = ".";
    // Distance between two QMs
    private const int DISTANCE_BETWEEN_QMS = 10;

    private float updateCountdown = UPDATE_INTERVAL;

    public GameObject updateTimeText;


    // Main dictionary keeping all connection and their rendered counterparts
    private Dictionary<MQ.Client, GameObject> qmgrs = new Dictionary<MQ.Client, GameObject>();

    public DependencyGraph dependencyGraph = new DependencyGraph();


    public List<MQ.QueueManager> queueManagers = new List<MQ.QueueManager>();

    // Use this method for adding new Mq connections (aka connections to different Qmgrs)
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
            queueManagers.Add(newQmgr);


            // Render queue manager. Note that data is stored in Component (ie Script) not in GameObject!
            // GameObject is just an Entity/Container for Components that perform the real functionality

            GameObject qmgrGameObject = new GameObject(newQmgr.qmgrName, typeof(QueueManager));
            QueueManager qmgrComponent = qmgrGameObject.GetComponent(typeof(QueueManager)) as QueueManager;
            qmgrComponent.queueManager = newQmgr;
           


            dependencyGraph.CreateDependencyGraph(newQmgr.queues, newQmgr.channels, newQmgr.applications, newQmgr.qmgrName); //Create Dependency Graphs

            ///DELETE: debug info
            foreach (KeyValuePair<string, List<string>> dependency in dependencyGraph.indirectDependencies)
            {
                Debug.Log("Indirect Dependency for " + dependency.Key + " is: " + string.Join(" , ", dependency.Value.ToArray()));
            }
            ///

            // Get the rendering position according its order
            Vector3 position = GetQueueMangagerPosition();
            Debug.Log("New position" + position);
            qmgrComponent.baseLoc = position;
            qmgrs[newMqClient] = qmgrGameObject;


            qmgrGameObject.transform.parent = this.transform;

            return;
        }


        // Otherwise periodically update all Queue managers
        updateCountdown -= Time.deltaTime;

        

        if (updateCountdown <= 0)
        {
            // Show update time information
            Text updateTimeTextComponent = updateTimeText.GetComponent<Text>();
            String nowTime = DateTime.Now.ToString();
            nowTime = nowTime.Substring(9);
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
                return client.GetQueue(selectedQueueName);
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
                return client.GetAllMessages(selectedQueueName);
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


    public Vector3 GetQueueMangagerPosition()
    {
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

