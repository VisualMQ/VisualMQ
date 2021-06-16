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
        Debug.Log("haha");
        RESTQM queue_manager = new RESTQM("https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443", "QM1", "shuchengtian", "XXXXX");
        string allQueues = queue_manager.GetAllQueues();
        Debug.Log(allQueues);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
