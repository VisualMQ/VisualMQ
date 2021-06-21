using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MQ;

public class Queue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Script is working...");
        string testUsername = "YOUR_USERNAME";
        string testAPIKey = "YOUR_API_KEY";
        string testQMUrl = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
        string testQmgr = "QM1";

        QueueManager queue_manager = new QueueManager(testQMUrl, testQmgr, testUsername, testAPIKey);
        string allQueues = queue_manager.GetAllQueues();
        Debug.Log(allQueues);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
