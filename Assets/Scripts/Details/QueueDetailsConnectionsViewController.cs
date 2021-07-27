using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsConnectionsViewController : MonoBehaviour
{
    // Windows
    public GameObject WindowQueueDetails, WindowMessageLists, WindowConnections;
    
    // Buttons
    private Button returnButton, closeButton;
    private Button tab1QueueDetails, tab2MessageLists, tab3Connections;
    private Button showIncome, showOutcome;

    // Transform
    
    
    /* Details Section for different type of Queue
    public GameObject QueueDetailLocal;
    public GameObject QueueDetailRemote;
    public GameObject QueueDetailAlias;
    public GameObject QueueDetailTransmission;*/
    /*
    // Text Fields: Remote
    private Transform typeGroups;
    private Transform textGroupRemote, textGroupAlias;
    private Text textQueue1_targetQM, textQueue2_targetQueue, textQueue3_transmission;
    // Text Fields: Alias
    private Text textQueue4_targetQueue, textQueue5_currentPath;
    // Text Fields: Local
    // Text Fields: Transmission*/



    // Awake: Connect to Objects
    private void Awake() 
    {
                /*
        // Locate Type Parents
        typeGroups = transform.Find("QueueTypesGameObject");
        textGroupRemote = typeGroups.Find("QueueRemoteGameObject");
        textGroupAlias = typeGroups.Find("QueueAliasGameObject");
        
        // Locate Text: Remote
        textQueue1_targetQM = textGroupRemote.Find("TextQueue_1").GetComponent<Text>();
        textQueue2_targetQueue = textGroupRemote.Find("TextQueue_2").GetComponent<Text>();
        textQueue3_transmission = textGroupRemote.Find("TextQueue_3").GetComponent<Text>();
        // Locate Text: Alias
        textQueue4_targetQueue = textGroupAlias.Find("TextQueue_4").GetComponent<Text>();
        textQueue5_currentPath = textGroupAlias.Find("TextQueue_5").GetComponent<Text>();
*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    /*

    Prepare Information based on the queue type
    
    void GetQueueLocal(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailLocal.SetActive(true);
    }

    void GetQueueRemote(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailRemote.SetActive(true);
        textqueue1_targetQM.text = queue.targetQueueName;
        textqueue2_targetQueue.text = queue.targetQmgrName;
        textqueue3_transmission.text = queue.transmissionQueueName;
    }

    void GetQueueAlias(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailAlias.SetActive(true);
        
        //textqueue4_targetQueue.text = ((AliasQueue)queue).targetQueueName;
        textQueue5_currentPath.text = "";
    }

    void GetQueueTransmission(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailTransmission.SetActive(true);
    }

    void SetAllQueueTypeInfoObjectFalse()
    {
        QueueDetailLocal.SetActive(false);
        QueueDetailRemote.SetActive(false);
        QueueDetailAlias.SetActive(false);
        QueueDetailTransmission.SetActive(false);
    }
    */
}



        /*
        // Get Queue Type
        switch (queue.GetTypeName())
        {
            case "Local":
                GetQueueLocal(queue);
                break;
            case "Alias":
                GetQueueAlias(queue);
                break;
            case "Remote":
                GetQueueRemote(queue);
                break;
            case "Transmission":
                GetQueueTransmission(queue);
                break;
            default:
                Debug.Log("Default case");
                break;
        }
        */