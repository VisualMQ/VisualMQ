using System;
using System.Collections.Generic;
using UnityEngine;


public class State : MonoBehaviour
{

    private const float API_CHECK_MAXTIME = 10.0f; // 10 * 60.0f; ten minutes
    private float apiCheckCountdown = API_CHECK_MAXTIME;
    private MQ.QMClient qmClient;

    private GameObject renderedQmgr;

    void Start()
    {

        Debug.Log("Script has started.");

        //Step one: create an Http client (QMClient class), remember to set the username and apikey
        string testUsername = "lukascerny20";
        string testAPIKey = "B4HnZeDYfykU-4PfpxFLbLaayjkKTBlIhZHCrlIQqVJp";
        string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
        string testQmgr = "QM1";

        //Step one+: create a second Http client
        try
        {
            qmClient = new MQ.QMClient(testQMUrl, testQmgr, testUsername, testAPIKey);
            Debug.Log("Authentication succeeded.");
        }
        catch (Exception)
        {
            Debug.Log("Authentication failed.");
            return;
        }



        ////Step two: create QMInfo, QueuesFactory(for making queues), and inset MessagesInfo objects to each queue
        MQ.QueueManager qmgr = qmClient.GetQmgr();
        List<MQ.Queue> queues = qmClient.GetAllQueues();

        GameObject qmgrGameObject = new GameObject(qmgr.qmgrName, typeof(QueueManager));
        QueueManager qmgrComponent = qmgrGameObject.GetComponent(typeof(QueueManager)) as QueueManager;
        qmgrComponent.queueManager = qmgr;
        qmgrComponent.queues = queues;
        renderedQmgr = qmgrGameObject;

    }

    void Update()
    {

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

