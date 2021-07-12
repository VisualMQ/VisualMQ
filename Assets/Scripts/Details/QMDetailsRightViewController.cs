using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QMDetailsRightViewController : MonoBehaviour
{
    // Details Window Game Object
    public GameObject QMDetailQueueListWindow;
    public GameObject QMDetailsWindow;
    public GameObject QueueDetailWindow;

    // Queue Lists Items
    private Transform QueueRowItem;
    private Transform container;

    // Buttons
    public Button toDetails, toQueueLists;
    public Button closeButton;

    // List of row objects
    List<GameObject> rowItemList = new List<GameObject>();

    // Locate the Objects
    private void Awake() {
        // Container and Row Item
        container = transform.Find("QueueRowContainer");
        QueueRowItem = container.Find("QueueRowItem");
    }

    // Start is called before the first frame update
    void Start()
    {   
        // Buttons Listener
        closeButton.onClick.AddListener(CloseButtonClicked);
        toDetails.onClick.AddListener(ToDetailsClicked);
        toQueueLists.onClick.AddListener(ToQueueListsClicked);
    }

    public void GenerateQueueList(string selectedQM)
    {
        // Get queues in the selectedQM
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        List<MQ.Queue> queues = stateComponent.GetAllQueuesInQmgr(selectedQM);
        
        QueueRowItem.gameObject.SetActive(false);

        // Row Origin Position
        float rowHeight = 50f;
        float startY = -37f;

        int size = queues.Count;

        for (int i = 0; i < size; i++)
        {
            Transform item = Instantiate(QueueRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(5, -rowHeight * i + startY);

            item.gameObject.SetActive(true);
            rowItemList.Add(item.gameObject);

            item.Find("TextQueueName").GetComponent<Text>().text = queues[i].queueName;

            int keyIdx = i;
            Button curButton = item.Find("QueueRowButton").GetComponent<Button>();
            curButton.onClick.AddListener(() => queueRowItemSelected(keyIdx, selectedQM, queues[keyIdx].queueName) );
        }
    }


    // Clean all created queue row item
    void QueueListDestroy()
    {
        
        int size = rowItemList.Count;
        for (int i = 0; i < size; i++)
        {
            GameObject cur = rowItemList[0];
            rowItemList.RemoveAt(0);
        }
    }


    // Queue is selected -> To Queue Details Winodw
    void queueRowItemSelected(int rowidx, string qmName, string queueName)
    {
        // To Queue Detail Window
        QMDetailQueueListWindow.SetActive(false);
        QueueDetailWindow.SetActive(true);

        List<string> temp = new List<string>() { qmName, queueName };
        QueueDetailWindow.SendMessage("GetQueueBasicInfo", temp);
    }


    void CloseButtonClicked()
    {
        QMDetailQueueListWindow.SetActive(false);
        QMDetailsWindow.SetActive(false);
    }


    void ToDetailsClicked()
    {
        // Show Current Details Window
        QMDetailsWindow.SetActive(true);
        // Close the Queue List Window
        QMDetailQueueListWindow.SetActive(false);
    }


    void ToQueueListsClicked()
    {

    }

}
