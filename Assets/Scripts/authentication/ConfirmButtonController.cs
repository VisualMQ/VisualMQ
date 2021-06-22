using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MQ;

//using UserNameController;
//using static UserNameController;
//using static APIController;


public class ConfirmButtonController : MonoBehaviour
{
    public Button confirmButton;
/*
    private string userName = "yuexu";
    private string apiKey = "uKnxScu6GXxkbEAWLIGMPCPp5hl1ZPco553uDtWOD620";
    private string MQURL= "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
    private string QMName ="QM1";
*/
    //public UserNameController scriptName;
    //public APIController scriptAPI;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Authentication: Confirm Button");
        //confirmButton.onClick.AddListener(ConfirmButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ConfirmButtonClicked()
    {
        //Debug.Log("Comfirm Button clicked");
        //userName = scriptName.GetCurrentUserName();
        //apiKey = scriptAPI.GetCurrentAPIKey();

        //Debug.Log(userName);
        //QueueManager queue_manager = new QueueManager(MQURL, QMName, userName, apiKey);
        //string queueInfo = queue_manager.GetQueue("DEV.QUEUE.1");
        //Debug.Log(queueInfo);
    }
}
