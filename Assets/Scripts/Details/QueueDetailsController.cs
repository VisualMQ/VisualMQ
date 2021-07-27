using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsController : MonoBehaviour
{
    
    // Public Window
    public GameObject WindowQueueDetails, WindowMessageLists, WindowConnections;

    // Text Fields Basic Info
    private Transform textGroup;
    private Text text0_queueName, text1_maxNumberMessage, text2_maxMessageLength, 
            text3_put, text4_get, text5_description, text6_created, text7_altered, 
            text8_depth;

    // Buttons
    private Button returnButton, closeButton;
    private Transform tabButtonsGroups;
    private Button toQueueDetail, toMessageList, toConnection;

    public List<string> currentSelected;


    private void Awake() {

        // Locate the Button
        returnButton = transform.Find("ButtonReturn").GetComponent<Button>();
        closeButton = transform.Find("ButtonCloseQueue").GetComponent<Button>();

        // Tab Buttons
        tabButtonsGroups = transform.Find("GameObjectTabButtons");
        toQueueDetail = tabButtonsGroups.Find("ButtonQueueDetails").GetComponent<Button>();
        toMessageList = tabButtonsGroups.Find("ButtonMessageList").GetComponent<Button>();
        toConnection = tabButtonsGroups.Find("ButtonConnections").GetComponent<Button>();

        // Locate the Text
        textGroup = transform.Find("TextFieldsGroup");
        text0_queueName = textGroup.Find("Text").GetComponent<Text>();
        text1_maxNumberMessage = textGroup.Find("Text1").GetComponent<Text>();
        text2_maxMessageLength = textGroup.Find("Text2").GetComponent<Text>();
        text3_put = textGroup.Find("Text3").GetComponent<Text>();
        text4_get = textGroup.Find("Text4").GetComponent<Text>();
        text5_description = textGroup.Find("Text5").GetComponent<Text>();
        text6_created = textGroup.Find("Text6").GetComponent<Text>();
        text7_altered = textGroup.Find("Text7").GetComponent<Text>();
        text8_depth = textGroup.Find("Text8").GetComponent<Text>();

        // Link Queue Detail Window
        Queue.QueueDetailWindow = this;
        
    }


    void Start()
    {
        //Button Listeners
        returnButton.onClick.AddListener(ReturnClicked);
        closeButton.onClick.AddListener(CloseClicked);

        toQueueDetail.onClick.AddListener(ToQueueDetailsClicked);
        toMessageList.onClick.AddListener(ToMessageListClicked);
        toConnection.onClick.AddListener(ToConnectionClicked);

    }

    
    /*
        Queue Content Generation
        Parameters: QM name and Queue name in a list
    */
    public void GetQueueBasicInfo(List<string> temp)
    {
        WindowQueueDetails.SetActive(true);
        currentSelected = temp;
        
        
        string qmName = temp[0];
        string queueName = temp[1];

        int starIdx = qmName.Length;
        string removeQMQueueName = queueName.Substring(starIdx+1);

        Debug.Log("AAA"+ qmName + queueName);


        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        MQ.Queue queue = stateComponent.GetQueueDetails(qmName, removeQMQueueName);


        text0_queueName.text = queue.queueName;
        text1_maxNumberMessage.text = queue.maxNumberOfMessages.ToString();
        text2_maxMessageLength.text = queue.maxMessageLength.ToString();
        text3_put.text = queue.inhibitPut.ToString();
        text4_get.text = queue.inhibitGet.ToString();
        text5_description.text = queue.description;
        text6_created.text = queue.timeCreated;
        text7_altered.text = queue.timeAltered;
        text8_depth.text = queue.currentDepth.ToString();


    }


// Button Listeners: 
// closeButton, returnButton, tabButtons(*3)
//
//


    // Return to Queue Lists (QM Window)
    private void ReturnClicked()
    {
        WindowQueueDetails.SetActive(false); // Close Current
        
        WindowMessageLists.SetActive(true);  // To Queue List Window (QMDetailRight)
        WindowMessageLists.SendMessage("GenerateQueueList", currentSelected[0]);
    }

    // Close current window
    private void CloseClicked()
    {
        WindowQueueDetails.SetActive(false);
    }

    // Stay in current window
    private void ToQueueDetailsClicked()
    {
        return;
    }

    // Switch to Message List Window
    private void ToMessageListClicked()
    {
        WindowMessageLists.SetActive(true);
        WindowMessageLists.SendMessage("GenerateMessageList", currentSelected);
        WindowQueueDetails.SetActive(false); // Close current
    }

    // Switch to Connection Window
    private void ToConnectionClicked()
    {
        WindowQueueDetails.SetActive(false);
        WindowConnections.SetActive(true);
    }

}