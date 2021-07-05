using System;
using System.Collections.Generic;
using UnityEngine;


public class State : MonoBehaviour
{

    private const float UPDATE_INTERVAL = 10.0f; // 10 seconds
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

            // Render queue manager. Note that data is stored in Component (ie Script) not in GameObject!
            // GameObject is just an Entity/Container for Components that perform the real functionality
            GameObject qmgrGameObject = new GameObject(newQmgr.qmgrName, typeof(QueueManager));
            QueueManager qmgrComponent = qmgrGameObject.GetComponent(typeof(QueueManager)) as QueueManager;
            qmgrComponent.queues = newQueues;

            qmgrs[newMqClient] = qmgrGameObject;
            return;
        }


        // Otherwise periodically update all Queue managers
        updateCountdown -= Time.deltaTime;
        if (updateCountdown <= 0)
        {
            Debug.Log("Updating state...");

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

}

