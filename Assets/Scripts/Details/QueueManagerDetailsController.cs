using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* If the user selects the QM, show this info panle for Queue manager
*/
public class QueueManagerDetailsController : MonoBehaviour
{
    private GameObject subwindowDetails, subwindowQueues;
    private GameObject queueRowTemplate;

    // Queue manager details
    private Text detailsName, detailsStatus, detailsConnectionCount, detailsTimeStarted;

    // Buttons inside this window
    private Button toDetails, toQueues;

    // The QM name used for trigerring the QM Details Window
    private MQ.QueueManager currentQueueManager;

    private SidebarController sidebarController;
    private State stateComponent;


    void Awake()
    {
        toDetails = transform.Find("Tabs/TabDetails").GetComponent<Button>();
        toQueues = transform.Find("Tabs/TabQueues").GetComponent<Button>();

        subwindowDetails = transform.Find("Details").gameObject;
        subwindowQueues = transform.Find("Queues").gameObject;
        queueRowTemplate = Resources.Load("Prefabs/QueueRowItem") as GameObject;

        detailsName = subwindowDetails.transform.Find("Name/TextName").GetComponent<Text>();
        detailsStatus = subwindowDetails.transform.Find("Status/TextStatus").GetComponent<Text>();
        detailsConnectionCount = subwindowDetails.transform.Find("ConnectionCount/TextConnectionCount").GetComponent<Text>();
        detailsTimeStarted = subwindowDetails.transform.Find("TimeStarted/TextTimeStarted").GetComponent<Text>();


        QueueManager.QMDetailWindow = this;

        toDetails.onClick.AddListener(ToQueueManagerDetails);
        toQueues.onClick.AddListener(ToQueueList);

        GameObject stateGameObject = GameObject.Find("State");
        stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        sidebarController = gameObject.transform.parent.gameObject.GetComponent<SidebarController>();
    }


    // Initialise the QM Info using the QM name
    public void GetQueueManagerDetails(string selectedQMName)
    {
        currentQueueManager = stateComponent.GetSelectedQmgr(selectedQMName);

        ToQueueManagerDetails();
    }


    public void ToQueueManagerDetails()
    {
        subwindowDetails.SetActive(true);
        subwindowQueues.SetActive(false);

        toDetails.Select();

        detailsName.text = currentQueueManager.qmgrName;
        detailsStatus.text = currentQueueManager.state;
        detailsConnectionCount.text = currentQueueManager.connectionCount.ToString();
        detailsTimeStarted.text = currentQueueManager.started;
    }

    public void ToQueueList()
    {
        subwindowDetails.SetActive(false);
        subwindowQueues.SetActive(true);

        toQueues.Select();

        // Clear previous generate list
        foreach (Transform child in transform.Find("Queues/QueuesList"))
        {
            if (child.gameObject.name == "QueueRowItem(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
        }


        // Get queues in the selected queue manager
        List<MQ.Queue> queues = stateComponent.GetAllQueuesInQmgr(currentQueueManager.qmgrName);
        for (int i = 0; i < queues.Count; i++)
        {
            MQ.Queue queue = queues[i];

            GameObject item = Instantiate(queueRowTemplate, transform.Find("Queues/QueuesList"));

            item.transform.Find("Text").GetComponent<Text>().text = queue.queueName;

            Button button = item.GetComponent<Button>();
            // TODO: pretty nasty solution when we have to change the queue name
            string queueFullName = currentQueueManager.qmgrName + "." + queue.queueName;
            button.onClick.AddListener(() => sidebarController.ShowQueueDetails(currentQueueManager.qmgrName, queueFullName));
        }

    }

}