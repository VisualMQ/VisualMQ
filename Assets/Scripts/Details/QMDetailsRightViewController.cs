using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QMDetailsRightViewController : MonoBehaviour
{
    // Details Window Game Object
    public GameObject QMDetailsRightWindow;

    // Queue Lists Items
    private Transform QueueRowItem;
    private Transform container;

    // Buttons
    public Button toDetails, toQueueLists;
    public Button closeButton;

    // !!! TEST !!!
    public Button testButton;
    void Clicked(){
        GenerateQueueList("QM1");
    }


    // Start is called before the first frame update
    void Start()
    {   
        // !!! TEST !!!
        testButton.onClick.AddListener(Clicked);

        // Buttons & Listener
        closeButton.onClick.AddListener(CloseButtonClicked);
        toDetails.onClick.AddListener(ToDetailsClicked);
        toQueueLists.onClick.AddListener(ToQueueListsClicked);
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    void GenerateQueueList(string selectedQM)
    {
        // Get queues in the selectedQM
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        List<MQ.Queue> queues = stateComponent.GetAllQueuesInQmgr(selectedQM);

        // Container and Row Item
        container = transform.Find("QueueRowContainer");
        QueueRowItem = container.Find("QueueRowItem");
        
        QueueRowItem.gameObject.SetActive(false);

        // Row Origin Position
        float rowHeight = 45f;
        float startY = -32f;

        int size = queues.Count;

        for (int i = 0; i < size; i++)
        {
            Transform item = Instantiate(QueueRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(5, -rowHeight * i + startY);
            item.gameObject.SetActive(true);

            item.Find("TextQueueName").GetComponent<Text>().text = queues[i].queueName;
        }
    }


    void CloseButtonClicked()
    {
        QMDetailsRightWindow.SetActive(false);
    }


    void ToDetailsClicked()
    {

    }


    void ToQueueListsClicked()
    {

    }

}
