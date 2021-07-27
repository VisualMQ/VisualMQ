using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsRightViewController : MonoBehaviour
{

    // Windows Object
    public GameObject WindowQueueDetails, WindowMessageLists, WindowConnections;
    public GameObject WindowQMQueueList;
    public GameObject MessageWindow;

    // Message List Items
    private Transform MessageRowItem;
    private Transform container;

    // Buttons
    private Button closeButton, returnRight;
    private Transform tabButtonGroup; // parent of tab buttons
    private Button toQueueDetail, toMessageList, toConnection;

    public List<string> currentSelected;

    private void Awake() 
    {
        // Locate the Objects
        container = transform.Find("MessageRowContainer");
        MessageRowItem = container.Find("MessageRowItem");
        MessageRowItem.gameObject.SetActive(false);

        // Locate the Buttons
        closeButton = transform.Find("ButtonClose").GetComponent<Button>();
        returnRight = transform.Find("ButtonReturn").GetComponent<Button>();

        // Locate Tab Buttons
        tabButtonGroup = transform.Find("GameObjectTabButtons");
        toQueueDetail = tabButtonGroup.Find("ButtonQueueDetails").GetComponent<Button>();
        toMessageList = tabButtonGroup.Find("ButtonMessageList").GetComponent<Button>();
        toConnection = tabButtonGroup.Find("ButtonConnections").GetComponent<Button>();
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        // Default Hide
        WindowQueueDetails.SetActive(false);
        WindowMessageLists.SetActive(false);

        // Button Listener
        closeButton.onClick.AddListener(closeWindowClicked);
        returnRight.onClick.AddListener(returnToQueueList);

        toQueueDetail.onClick.AddListener(ToQueueDetailsClicked);
        toMessageList.onClick.AddListener(toQueueMessageList);
        toConnection.onClick.AddListener(toConnectionClicked);
    }

    
    public void GenerateMessageList(List<string> temp)
    {
        DestroyMessageList();

        currentSelected = temp;
        string qmName = temp[0];
        string queueName = temp[1];

        int starIdx = qmName.Length;
        string removeQMQueueName = queueName.Substring(starIdx+1);

        // Get List of MQ.Message
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        List<MQ.Message> messages = stateComponent.GetAllMessages(qmName,removeQMQueueName);

        int size = messages.Count;

        // Row Origin Position
        float rowHeight = 50f;
        float startY = -37f;

        for (int i = 0; i < size; i++)
        {
            Transform item = Instantiate(MessageRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(7, -rowHeight * i + startY);
            item.gameObject.SetActive(true);
            
            item.Find("TextMessageName").GetComponent<Text>().text = messages[i].messageId;
            
            int keyIdx = i;
            Button curButton = item.Find("MessageRowButton").GetComponent<Button>();
            curButton.onClick.AddListener(() => messageRowItemSelected(keyIdx, qmName, queueName, messages[keyIdx].messageId) );
        }
    }

    // destroy previous message lists
    private void DestroyMessageList()
    {
        foreach (Transform child in container) 
        {
            if(child.gameObject.name == "MessageRowItem(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    // if a message selected -> pop out window for messages
    private void messageRowItemSelected(int idx, string qmName, string queueName, string messageid)
    {
        MessageWindow.SetActive(true);
        Debug.Log(messageid);
        List<string> temp = new List<string>() { qmName, queueName , messageid};
        MessageWindow.SendMessage("GenerateMessageWindow", temp);
    }



// Button Listeners
//
//
//

    // Close window
    private void closeWindowClicked(){
        WindowMessageLists.SetActive(false);
        WindowQueueDetails.SetActive(false);
    }

    // To window Queue Detail
    private void ToQueueDetailsClicked()
    {
        WindowMessageLists.SetActive(false);  // close current
        WindowQueueDetails.SetActive(true);   // show left
        //WindowQueueDetails.SendMessage("test");  // Reload Details
    }

    // Stay in current window
    private void toQueueMessageList()
    {
        return;
    }

    // To QM Window (Queue List)
    private void returnToQueueList()
    {
         // Close Current
        WindowMessageLists.SetActive(false);
        // To Queue List Window (QMDetailRight)
        WindowQMQueueList.SetActive(true);
        WindowQMQueueList.SendMessage("GenerateQueueList", currentSelected[0]);
    }

    // To Queue Detail -- Cconnection Window
    private void toConnectionClicked()
    {
        WindowMessageLists.SetActive(false);
        WindowConnections.SetActive(true);
    }

}
