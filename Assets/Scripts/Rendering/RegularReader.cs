using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MQ;


public class RegularReader : MonoBehaviour
{

    State internalState;


/* To be changed to keep track of QMs*/
    private Dictionary<string, bool> renderedQMs = new Dictionary<string, bool>();
    private Dictionary<string, bool> renderedQueues = new Dictionary<string, bool>();


    void Start()
    {
        //Step one: create an Http client (QMClient class), remember to set the username and apikey
        string testUsername = "shuchengtian";
        string testAPIKey = "aCPHZ4ys0Tnn2xQLPsHc6lEz4CTenKmNsyW9q0MoQ0bf";
        string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
        string testQmgr = "QM1";

        //Step one+: create a second Http client
        QMClient mq, mq2;
        string testQMUrl2 = "https://web-qm2-3628.qm.eu-gb.mq.appdomain.cloud:443";
        string testQmgr2 = "qm2";
        try {
            mq = new QMClient(testQMUrl, testQmgr, testUsername, testAPIKey);
            mq2 = new QMClient(testQMUrl2, testQmgr2, testUsername, testAPIKey);
        } catch (Exception) {
            Console.WriteLine("Authentication failed.");
            return;
        }

        //Step two: create QMInfo, QueuesFactory(for making queues), and inset MessagesInfo objects to each queue
        string QMInfo = mq.GetQmgr();
        QMInfo QM1 = new QMInfo(QMInfo);

        string QMInfo2 = mq2.GetQmgr();
        QMInfo QM2 = new QMInfo(QMInfo2);

        string allqueue = mq.GetAllQueues();
        QueuesFactory Qs1 = new QueuesFactory();
        List<Queue> Queues1 = Qs1.makeQueues(allqueue);

        string allqueue2 = mq2.GetAllQueues();
        QueuesFactory Qs2 = new QueuesFactory();
        List<Queue> Queues2 = Qs2.makeQueues(allqueue2);

        for (int i = 0; i < Queues1.Count; i++) {
            string messageInfo = mq.GetAllMessageIds(Queues1[i].name);
            Queues1[i].messages = new MessagesInfo(Queues1[i].name, messageInfo);
        }

        for (int i = 0; i < Queues2.Count; i++) {
            string messageInfo = mq2.GetAllMessageIds(Queues2[i].name);
            Queues2[i].messages = new MessagesInfo(Queues2[i].name, messageInfo);
        }
        
        //Step three: create a QueueManager object that consists QMinfo and list of queues
        //Maybe store the QMClient in the QueueManager Object as well?
        QueueManager QueueManager1 = new QueueManager(QM1, Queues1);
        QueueManager QueueManager2 = new QueueManager(QM2, Queues2);
        
        //Step four: create a list of QueueManagers and create an internal state object
        List<QueueManager> qms = new List<QueueManager>();
        qms.Add(QueueManager1);
        qms.Add(QueueManager2);
        internalState = new State(qms);

        



        InvokeRepeating("ContinousRender", 1f, 10f);  //1s delay, repeat every 1s

    }

    // Update is called once per frame
    void ContinousRender()
    {
        CreateVisual.generate_render();
    }
}
