using System;
using System.Collections.Generic;
using UnityEngine;


public class State : MonoBehaviour
{

    private const float API_CHECK_MAXTIME = 10.0f; // 10 * 60.0f; ten minutes
    private float apiCheckCountdown = API_CHECK_MAXTIME;
    public MQ.QMClient qmClient = null;

    private GameObject renderedQmgr = null;

    //Step one: create an Http client (QMClient class), remember to set the username and apikey

    //public string testUsername = "lukascerny20";
    //public string testAPIKey = "xxxxxxx";
    //public string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
    //public string testQmgr = "QM1";

    void Start()
    {



    }

    void Update()
    {

        if (renderedQmgr == null && qmClient == null) return;

        if (renderedQmgr == null && qmClient != null)
        {
            Debug.Log("Script has started.");

            ////Step two: create QMInfo, QueuesFactory(for making queues), and inset MessagesInfo objects to each queue
            MQ.QueueManager qmgr = qmClient.GetQmgr();
            List<MQ.Queue> queues = qmClient.GetAllQueues();

            GameObject qmgrGameObject = new GameObject(qmgr.qmgrName, typeof(QueueManager));
            QueueManager qmgrComponent = qmgrGameObject.GetComponent(typeof(QueueManager)) as QueueManager;
            qmgrComponent.queueManager = qmgr;
            qmgrComponent.queues = queues;
            renderedQmgr = qmgrGameObject;
            return;
        }
        
        apiCheckCountdown -= Time.deltaTime;
        // Periodically check the status
        if (apiCheckCountdown <= 0)
        {
            Debug.Log("Updating state...");

            List<MQ.Queue> queues = qmClient.GetAllQueues();
            QueueManager qmgrComponent = renderedQmgr.GetComponent(typeof(QueueManager)) as QueueManager;
            qmgrComponent.UpdateQueues(queues);

            apiCheckCountdown = API_CHECK_MAXTIME;
        }


    }
}

