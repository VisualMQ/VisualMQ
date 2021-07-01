using System;
using System.Collections.Generic;
using UnityEngine;


public class State : MonoBehaviour
{


    void Start()
    {

        Debug.Log("Script has started.");

        //Step one: create an Http client (QMClient class), remember to set the username and apikey
        string testUsername = "lukascerny20";
        string testAPIKey = "B4HnZeDYfykU-4PfpxFLbLaayjkKTBlIhZHCrlIQqVJp";
        string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
        string testQmgr = "QM1";

        //Step one+: create a second Http client
        MQ.QMClient qmClient;
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

        //_QueueManagerJson qm = JsonConvert.DeserializeObject<_QueueManagerJson>(qmJson);
        //Debug.Log(qm.qmgr[0].name);
        //Debug.Log(qmgr.qmgrName);
        //Debug.Log(queues[0].queueName);

        //_QueueResponseJson qm = JsonUtility.FromJson<_QueueResponseJson>(queuesJson);
        //Debug.Log(qm.queue[0].name);


        //string allqueue = mq.GetAllQueues();
        //QueuesFactory Qs1 = new QueuesFactory();
        //List<Queue> Queues1 = Qs1.makeQueues(allqueue);


        //for (int i = 0; i < Queues1.Count; i++)
        //{
        //    string messageInfo = mq.GetAllMessageIds(Queues1[i].name);
        //    Queues1[i].messages = new MessagesInfo(Queues1[i].name, messageInfo);
        //}

    }

    void Update()
    {




    }
}

