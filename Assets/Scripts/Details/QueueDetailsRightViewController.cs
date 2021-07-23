using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsRightViewController : MonoBehaviour
{

    // Windows Object
    public GameObject QueueDetailLeftWindow;
    public GameObject QueueDetailRightWindow;
    public GameObject QMDetailRightWindow;
    public GameObject MessageWindow;

    // Message List Items
    private Transform MessageRowItem;
    private Transform container;

    // Buttons
    private Button closeRight, returnRight;
    private Button toLeft, toRight;

    public List<string> currentSelected;

    private void Awake() {
        // Locate the Objects
        container = transform.Find("MessageRowContainer");
        MessageRowItem = container.Find("MessageRowItem");
        MessageRowItem.gameObject.SetActive(false);

        // Locate the Buttons
        closeRight = transform.Find("ButtonCloseQueueRight").GetComponent<Button>();
        returnRight = transform.Find("ButtonReturnRight").GetComponent<Button>();
        toLeft = transform.Find("ButtonQueueDetails2").GetComponent<Button>();
        toRight = transform.Find("ButtonMessageList2").GetComponent<Button>();
    }
    
    // Start is called before the first frame update
    void Start()
    {   
        // Default Hide
        QueueDetailLeftWindow.SetActive(false);
        QueueDetailRightWindow.SetActive(false);

        // Button Listener
        closeRight.onClick.AddListener(closeWindowClicked);
        returnRight.onClick.AddListener(returnToQueueList);
        toLeft.onClick.AddListener(toQueueDetail);
        toRight.onClick.AddListener(toQueueMessageList);
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

    void DestroyMessageList()
    {
        foreach (Transform child in container) 
        {
            if(child.gameObject.name == "MessageRowItem(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }


    void messageRowItemSelected(int idx, string qmName, string queueName, string messageid)
    {
        MessageWindow.SetActive(true);
        Debug.Log(messageid);
        List<string> temp = new List<string>() { qmName, queueName , messageid};
        MessageWindow.SendMessage("GenerateMessageWindow", temp);
    }


    /*
    * Button Listener Functions
    */
    void closeWindowClicked(){
        QueueDetailRightWindow.SetActive(false);
        QueueDetailLeftWindow.SetActive(false);
    }

    void toQueueDetail()
    {
        QueueDetailRightWindow.SetActive(false); // close current
        QueueDetailLeftWindow.SetActive(true);   // show left
        //QueueDetailLeftWindow.SendMessage("test");  // Reload Details
    }

    void toQueueMessageList()
    {
        return;
    }


    void returnToQueueList()
    {
         // Close Current
        QueueDetailRightWindow.SetActive(false);
        // To Queue List Window (QMDetailRight)
        QMDetailRightWindow.SetActive(true);
        QMDetailRightWindow.SendMessage("GenerateQueueList", currentSelected[0]);
    }


}
