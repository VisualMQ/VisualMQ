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
    private Button toDetails, toQueueLists;
    private Button closeButton;


    // Start is called before the first frame update
    void Start()
    {   
        // Buttons & Listener
        closeButton = GameObject.Find("ButtonClose").GetComponent<Button>();
        toDetails = GameObject.Find("ButtonQMDetails").GetComponent<Button>();
        toQueueLists = GameObject.Find("ButtonQueueList").GetComponent<Button>();
        closeButton.onClick.AddListener(CloseButtonClicked);
        toDetails.onClick.AddListener(toDetailsClicked);
        toQueueLists.onClick.AddListener(toQueueListsClicked);

        // Container and Row Item
        container = transform.Find("QueueRowContainer");
        QueueRowItem = container.Find("QueueRowItem");
        
        QueueRowItem.gameObject.SetActive(false);

        // Row Origin Position
        float rowHeight = 45f;
        float startY = -32f;

        // Generate Table Items

        for (int i = 0; i < 10; i++)
        {
            Transform item = Instantiate(QueueRowItem, container);
            RectTransform recTransform = item.GetComponent<RectTransform>();
            recTransform.anchoredPosition = new Vector2(5, -rowHeight * i + startY);
            item.gameObject.SetActive(true);
        }

        getQueueNames("QM1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void getQueueNames(string selectedQM)
    {
        List<string> queueNames = new List<string>();

        // Get Name Lists of the selectedQM
        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        queueNames = stateComponent.GetALLQueuesNames(selectedQM);

        Debug.Log("GET ALL QUEUE NAMES" + String.Join(", ", queueNames));
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
