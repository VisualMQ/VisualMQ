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
        Debug.Log("MESSAGE LIST");
        currentSelected = temp;
        string qmName = temp[0];
        string queueName = temp[1];
        Debug.Log(qmName + queueName);

        // Get List of MQ.Message
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        List<MQ.Message> messages = stateComponent.GetAllMessages(qmName,queueName);

        int size = messages.Count;

        // Row Origin Position
        float rowHeight = 45f;
        float startY = -37f;

        for (int i = 0; i < size; i++)
        {
            Transform item = Instantiate(MessageRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(7, -rowHeight * i + startY);
            item.gameObject.SetActive(true);
            
            item.Find("TextMessageName").GetComponent<Text>().text = messages[i].messageId;
            //item.Find("TextQueueName").GetComponent<Text>().text = queues[i].queueName;
        }
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
