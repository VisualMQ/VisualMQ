using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsRightViewController : MonoBehaviour
{
    // Message List Items
    private Transform MessageRowItem;
    private Transform container;

    public Button messageListTest;

    private void Awake() {
        // Locate the Objects
        container = transform.Find("MessageRowContainer");
        MessageRowItem = container.Find("MessageRowItem");
        MessageRowItem.gameObject.SetActive(false);
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        messageListTest.onClick.AddListener(test);
    }

    void test(){
        GenerateMessageList("QM1", "DEV.QUEUE.1");
    }

    public void GenerateMessageList(string qmName, string queueName)
    {
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
            
            //item.Find("TextQueueName").GetComponent<Text>().text = names[i];
            //item.Find("TextQueueName").GetComponent<Text>().text = queues[i].queueName;
        }
    }

}
