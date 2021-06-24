using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MQ;


public class RegularReader : MonoBehaviour
{


    
    void Start()
    {
        //Step one: create an Http client (QueueManager class), remember to set the username and apikey
        string testUsername = "NA";
        string testAPIKey = "NA";
        string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
        string testQmgr = "QM1";
        QueueManager mq = new QueueManager(testQMUrl, testQmgr, testUsername, testAPIKey);

        //Step two: create QMInfo, QueuesInfo, and List of MessagesInfo objects
        string QMInfo = mq.GetQmgr();
        QMInfo QM1 = new QMInfo(QMInfo);

        string allqueue = mq.GetAllQueues();
        QueuesInfo Qs1 = new QueuesInfo(allqueue);

        List<MessagesInfo> allQMessages = new List<MessagesInfo>();
        

        //Step three: create a state object that consists of the three objects/list of objects in step three
        State internalState = new State(QM1, Qs1, allQMessages);

        // Display this current QM1

        CreateVisual.VisualizeQM(QM1);

        foreach(QueueStorage storage in internalState.QueuesInfo.storage)
        {
            CreateVisual.VisualizeQueue(QM1, storage);
        }
       
        



    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        // dict[queuemanager1] = GameObject
        
       
    }
}
