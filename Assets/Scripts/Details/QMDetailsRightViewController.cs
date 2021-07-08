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
    void clicked(){
        getQueueNames("QM1");
    }

    // Start is called before the first frame update
    void Start()
    {   
        // !!! TEST !!!
        testButton.onClick.AddListener(clicked);

        // Buttons & Listener
        closeButton.onClick.AddListener(CloseButtonClicked);
        toDetails.onClick.AddListener(toDetailsClicked);
        toQueueLists.onClick.AddListener(toQueueListsClicked);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void generateQueueList(int size, List<string> names)
    {
        // Container and Row Item
        container = transform.Find("QueueRowContainer");
        QueueRowItem = container.Find("QueueRowItem");
        
        QueueRowItem.gameObject.SetActive(false);

        // Row Origin Position
        float rowHeight = 45f;
        float startY = -37f;

        for (int i = 0; i < size; i++)
        {
            Transform item = Instantiate(QueueRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(5, -rowHeight * i + startY);
            item.gameObject.SetActive(true);
            
            item.Find("TextQueueName").GetComponent<Text>().text = names[i];
        }
    }

    void getQueueNames(string selectedQM)
    {
        List<string> queueNames = new List<string>();

        // Get Name Lists of the selectedQM
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        queueNames = stateComponent.GetALLQueuesNames(selectedQM);

        Debug.Log("GET ALL QUEUE NAMES");
        generateQueueList(queueNames.Count, queueNames);
    }

    void CloseButtonClicked()
    {
        QMDetailsRightWindow.SetActive(false);
    }

    void toDetailsClicked()
    {

    }

    void toQueueListsClicked()
    {

    }

}
