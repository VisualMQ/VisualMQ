using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsConnectionsViewController : MonoBehaviour
{
    // Windows
    public GameObject WindowQueueDetails, WindowMessageLists, WindowConnections;
    public GameObject WindowQMQueueList;
    
    // Buttons
    private Button returnButton, closeButton;
    private Button tab1QueueDetails, tab2MessageLists, tab3Connections;
    private Button showIncome, showOutcome;

    // Test Group Objects
    private GameObject QueueDetailLocal, QueueDetailRemote, QueueDetailAlias, QueueDetailTransmission;

    // Text Fields
    private Transform typeGroups;
    private Transform textGroupRemote, textGroupAlias, textGroupLocal, textGroupTransmission;

    // Text Fields: Remote
    private Text textQueue1_targetQM, textQueue2_targetQueue, textQueue3_transmission;

    // Text Fields: Alias
    private Text textQueue4_targetQueue, textQueue5_currentPath;

    // Text Fields: Local
    private Text textQueue6_inputCount, textQueue7_outputCount;

    // Text Fields: Transmission
    private Text textQueue8_inputCount, textQueue9_outputCount;

    private List<string> currentSelected;


    // Awake: Connect to Objects
    private void Awake() 
    {
        // Locate Buttons
        returnButton = transform.Find("ButtonClose").GetComponent<Button>();
        closeButton = transform.Find("ButtonReturn").GetComponent<Button>();
        showIncome = transform.Find("ButtonShowIncome").GetComponent<Button>();
        showOutcome = transform.Find("ButtonShowOutcome").GetComponent<Button>();

        // Button Listener
        closeButton.onClick.AddListener(closeWindowClicked);
        returnButton.onClick.AddListener(returnToQueueList);
        showIncome.onClick.AddListener(showIncomingPath);
        showOutcome.onClick.AddListener(showOutcomingPath);

        // Locate Type Parents
        typeGroups = transform.Find("QueueTypesGameObject");
        textGroupRemote = typeGroups.Find("QueueRemoteGameObject");
        textGroupAlias = typeGroups.Find("QueueAliasGameObject");
        textGroupLocal = typeGroups.Find("QueueLocalGameObject");
        textGroupTransmission = typeGroups.Find("QueueTransmissionGameObject");

        // Locate Type Objects
        QueueDetailLocal = textGroupLocal.GetComponent<GameObject>();
        QueueDetailRemote = textGroupRemote.GetComponent<GameObject>();
        QueueDetailAlias = textGroupAlias.GetComponent<GameObject>();
        QueueDetailTransmission = textGroupTransmission.GetComponent<GameObject>();

        // Locate Text: Remote
        textQueue1_targetQM = textGroupRemote.Find("TextQueue_1").GetComponent<Text>();
        textQueue2_targetQueue = textGroupRemote.Find("TextQueue_2").GetComponent<Text>();
        textQueue3_transmission = textGroupRemote.Find("TextQueue_3").GetComponent<Text>();
        
        // Locate Text: Alias
        textQueue4_targetQueue = textGroupAlias.Find("TextQueue_4").GetComponent<Text>();
        textQueue5_currentPath = textGroupAlias.Find("TextQueue_5").GetComponent<Text>();
        
        // Locate Text: Local
        textQueue6_inputCount = textGroupLocal.Find("TextQueue_6").GetComponent<Text>();
        textQueue7_outputCount = textGroupLocal.Find("TextQueue_7").GetComponent<Text>();

        // Locate text: Transmission
        textQueue8_inputCount = textGroupTransmission.Find("TextQueue_8").GetComponent<Text>();
        textQueue9_outputCount = textGroupTransmission.Find("TextQueue_9").GetComponent<Text>();

        // Hide
        WindowConnections.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Given the queue name -> Queue Tyep detail
    public void generateQueueTypeDetail(List<string> temp)
    {
        currentSelected = temp;

        string qmName = temp[0];
        string queueName = temp[1];

        int starIdx = qmName.Length;
        string removeQMQueueName = queueName.Substring(starIdx+1);

        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        MQ.Queue queue = stateComponent.GetQueueDetails(qmName, removeQMQueueName);


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

    }



    // Details: Local Queue
    void GetQueueLocal(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailLocal.SetActive(true);

    }

    void GetQueueRemote(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailRemote.SetActive(true);
        //textQueue1_targetQM.text = queue.targetQueueName;
        //textqueue2_targetQueue.text = queue.targetQmgrName;
        //textqueue3_transmission.text = queue.transmissionQueueName;
    }

    // Type Details: Alias
    void GetQueueAlias(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailAlias.SetActive(true);
        
        //textqueue4_targetQueue.text = ((AliasQueue)queue).targetQueueName;
        textQueue5_currentPath.text = "";
    }

    // Type Details: Transmission
    private void GetQueueTransmission(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailTransmission.SetActive(true);
        textQueue8_inputCount.text = "";
        textQueue9_outputCount.text = "";

    }

    // Hide all queue type details
    private void SetAllQueueTypeInfoObjectFalse()
    {
        QueueDetailLocal.SetActive(false);
        QueueDetailRemote.SetActive(false);
        QueueDetailAlias.SetActive(false);
        QueueDetailTransmission.SetActive(false);
    }



// Button Listeners
//
//
// 

    // close current window
    private void closeWindowClicked()
    {
        WindowConnections.SetActive(false);
    }

    // to QM queue list
    private void returnToQueueList()
    {
        WindowMessageLists.SetActive(false);  // close current
        WindowQMQueueList.SetActive(true);   // show left
        WindowQMQueueList.SendMessage("GenerateQueueList", currentSelected[0]);
    }

    // show incoming path
    private void showIncomingPath()
    {

    }

    // show outcoming path
    private void showOutcomingPath()
    {

    }

    private void ToQueueDetailsClicked()
    {
        WindowConnections.SetActive(false);  // close current
        WindowQueueDetails.SetActive(true);   // show left
        WindowQueueDetails.SendMessage("GetQueueBasicInfo", currentSelected);  // Reload Details
    }

    private void ToMessageListClicked()
    {
        WindowConnections.SetActive(true);
        WindowMessageLists.SendMessage("GenerateMessageList", currentSelected);
        WindowQueueDetails.SetActive(false); // Close current
    }

}



        