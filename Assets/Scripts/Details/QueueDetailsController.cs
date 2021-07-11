using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsController : MonoBehaviour
{
    // PUBLIC
    public GameObject QueueDetailLeftWindow;
    public GameObject QueueDetailRightWindow;
    public GameObject QMDetailRightWindow;

    // Details Section for different type of Queue
    public GameObject QueueDetailLocal;
    public GameObject QueueDetailRemote;
    public GameObject QueueDetialAlias;
    public GameObject QueueDetailTransmission;

    // Text Fields Basic Info
    private Transform textGroup;
    private Text text0_queuename, text1_maxnumbermessage, text2_maxmessagelength, 
            text3_put, text4_get,text5_description, text6_created, text7_altered, 
            text8_depth;
    
    // Text Fields: Remote
    private Transform typeGroups;
    private Transform textGroupRemote, textGroupAlias;
    private Text textqueue1_targetQM, textqueue2_targetQueue, textqueue3_transmission;
    // Text Fields: Alias
    private Text textqueue4_targetQueue, textqueue5_currentPath;
    // Text Fields: Local
    // Text Fields: Transmission

    // Button
    private Button returnButton, closeButton;
    private Button toQueueDetail, toMessageList;

    public List<string> currentSelected;

    private void Awake() {

        // Locate the Button
        returnButton = transform.Find("ButtonReturn").GetComponent<Button>();
        closeButton = transform.Find("ButtonCloseQueue").GetComponent<Button>();
        toQueueDetail = transform.Find("ButtonQueueDetails").GetComponent<Button>();
        toMessageList = transform.Find("ButtonMessageList").GetComponent<Button>();

        // Locate the Text
        textGroup = transform.Find("TextFieldsGroup");
        text0_queuename = textGroup.Find("Text").GetComponent<Text>();
        text1_maxnumbermessage = textGroup.Find("Text1").GetComponent<Text>();
        text2_maxmessagelength = textGroup.Find("Text2").GetComponent<Text>();
        text3_put = textGroup.Find("Text3").GetComponent<Text>();
        text4_get = textGroup.Find("Text4").GetComponent<Text>();
        text5_description = textGroup.Find("Text5").GetComponent<Text>();
        text6_created = textGroup.Find("Text6").GetComponent<Text>();
        text7_altered = textGroup.Find("Text7").GetComponent<Text>();
        text8_depth = textGroup.Find("Text8").GetComponent<Text>();

        // Locate Type Parents
        typeGroups = transform.Find("QueueTypesGameObject");
        textGroupRemote = typeGroups.Find("QueueRemoteGameObject");
        textGroupAlias = typeGroups.Find("QueueAliasGameObject");
        
        // Locate Text: Remote
        textqueue1_targetQM = textGroupRemote.Find("TextQueue_1").GetComponent<Text>();
        textqueue2_targetQueue = textGroupRemote.Find("TextQueue_2").GetComponent<Text>();
        textqueue3_transmission = textGroupRemote.Find("TextQueue_3").GetComponent<Text>();
        // Locate Text: Alias
        textqueue4_targetQueue = textGroupAlias.Find("TextQueue_4").GetComponent<Text>();
        textqueue5_currentPath = textGroupAlias.Find("TextQueue_5").GetComponent<Text>();

        
    }

    // Start is called before the first frame update
    void Start()
    {
        //Button Listeners
        returnButton.onClick.AddListener(ReturnClicked);
        closeButton.onClick.AddListener(CloseClicked);
        toQueueDetail.onClick.AddListener(ToQueueDetailsClicked);
        toMessageList.onClick.AddListener(ToMessageListClicked);

        // Hide the Windows
        //QueueDetailLeftWindow.SetActive(false);
        //QueueDetailRightWindow.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
        Content Generation
    */
    public void GetQueueBasicInfo(List<string> temp)
    {
        currentSelected = temp;
        string qmName = temp[0];
        string queueName = temp[1];

        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        MQ.Queue queue = stateComponent.GetQueueuDetail(qmName, queueName);

        text0_queuename.text = queue.queueName;
        text1_maxnumbermessage.text = queue.maxNumberOfMessages.ToString();
        text2_maxmessagelength.text = queue.maxMessageLength.ToString();
        text3_put.text = queue.inhibitPut.ToString();
        text4_get.text = queue.inhibitGet.ToString();
        text5_description.text = queue.description;
        text6_created.text = queue.timeCreated;
        text7_altered.text = queue.timeAltered;
        text8_depth.text = queue.currentDepth.ToString();

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
    

    /*
    * Prepare Information based on the queue type
    */
    void GetQueueLocal(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailLocal.SetActive(true);
    }

    void GetQueueRemote(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetailRemote.SetActive(true);
        /*
        textqueue1_targetQM.text = queue.targetQueueName;
        textqueue2_targetQueue.text = queue.targetQmgrName;
        textqueue3_transmission.text = queue.transmissionQueueName;*/
    }

    void GetQueueAlias(MQ.Queue queue)
    {
        SetAllQueueTypeInfoObjectFalse();
        QueueDetialAlias.SetActive(true);
        
        //textqueue4_targetQueue.text = ((AliasQueue)queue).targetQueueName;
        textqueue5_currentPath.text = "";
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
        QueueDetialAlias.SetActive(false);
        QueueDetailTransmission.SetActive(false);
    }


    /*
    *    Button Listeners
    */
    // Return to Queue Lists
    void ReturnClicked()
    {
        // Close Current
        QueueDetailLeftWindow.SetActive(false);
        // To Queue List Window (QMDetailRight)
        QMDetailRightWindow.SetActive(true);
        QMDetailRightWindow.SendMessage("GenerateQueueList", currentSelected[0]);

    }

    void CloseClicked()
    {
        QueueDetailLeftWindow.SetActive(false);
    }

    // Reload Current Window
    void ToQueueDetailsClicked()
    {
        return;
    }

    // Switch to Message List Window
    void ToMessageListClicked()
    {
        QueueDetailRightWindow.SetActive(true);
        QueueDetailRightWindow.SendMessage("GenerateMessageList", currentSelected);

        QueueDetailLeftWindow.SetActive(false);
    }
}
