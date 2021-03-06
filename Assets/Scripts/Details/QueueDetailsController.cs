using UnityEngine;
using UnityEngine.UI;


public class QueueDetailsController : MonoBehaviour
{
    // Different sub-windows of Queue details sidebar
    private GameObject subwindowDetails, subwindowMessages, subwindowConnections;

    // Details text fields
    private Text detailsQueueName, detailsMaxNumberMessage, detailsMaxMessageLength, 
            detailsPut, detailsGet, detailsDescription, detailsCreated, detailsAltered, 
            detailsDepth, detailsType;

    // Connections text fields
    private Text connectionsOpenInputCount, connectionsOpenOutputCount, connectionsTargetQueue,
            connectionsTargetQueueManager, connectionsTransmissionQueue;

    // Buttons
    private Transform tabButtonsGroups;
    private Button toQueueDetail, toMessageList, toConnections;

    // State & Queue
    private MQ.Queue currentQueue;
    private State stateComponent;

    private GameObject messageRowTemplate;


    private void Awake()
    {
        // Tab Buttons
        tabButtonsGroups = transform.Find("Tabs");
        toQueueDetail = tabButtonsGroups.Find("TabDetails").GetComponent<Button>();
        toMessageList = tabButtonsGroups.Find("TabMessages").GetComponent<Button>();
        toConnections = tabButtonsGroups.Find("TabConnections").GetComponent<Button>();

        // Sub-windows
        subwindowDetails = transform.Find("Details").gameObject;
        subwindowMessages = transform.Find("Messages").gameObject;
        subwindowConnections = transform.Find("Connections").gameObject;

        // Buttons
        toQueueDetail.onClick.AddListener(ToQueueDetails);
        toMessageList.onClick.AddListener(ToMessages);
        toConnections.onClick.AddListener(ToConnections);

        // State object
        GameObject stateGameObject = GameObject.Find("State");
        stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        // Details sub-window
        detailsQueueName = subwindowDetails.transform.Find("Name/TextName").GetComponent<Text>();
        detailsMaxNumberMessage = subwindowDetails.transform.Find("MaxMessages/TextMaxMessages").GetComponent<Text>();
        detailsMaxMessageLength = subwindowDetails.transform.Find("MaxLength/TextMaxLength").GetComponent<Text>();
        detailsPut = subwindowDetails.transform.Find("InhibitPut/TextInhibitPut").GetComponent<Text>();
        detailsGet = subwindowDetails.transform.Find("InhibitGet/TextInhibitGet").GetComponent<Text>();
        detailsDescription = subwindowDetails.transform.Find("Description/TextDescription").GetComponent<Text>();
        detailsCreated = subwindowDetails.transform.Find("TimeCreated/TextTimeCreated").GetComponent<Text>();
        detailsAltered = subwindowDetails.transform.Find("TimeAltered/TextTimeAltered").GetComponent<Text>();
        detailsDepth = subwindowDetails.transform.Find("Depth/TextDepth").GetComponent<Text>();
        detailsType = subwindowDetails.transform.Find("Type/TextType").GetComponent<Text>();

        // Connections sub-window
        connectionsOpenInputCount = subwindowConnections.transform.Find("OpenInputCount/TextOpenInputCount").GetComponent<Text>();
        connectionsOpenOutputCount = subwindowConnections.transform.Find("OpenOutputCount/TextOpenOutputCount").GetComponent<Text>();
        connectionsTargetQueue = subwindowConnections.transform.Find("TargetQueue/TextTargetQueue").GetComponent<Text>();
        connectionsTargetQueueManager = subwindowConnections.transform.Find("TargetQueueManager/TextTargetQueueManager").GetComponent<Text>();
        connectionsTransmissionQueue = subwindowConnections.transform.Find("TransmissionQueue/TextTransmissionQueue").GetComponent<Text>();

        messageRowTemplate = Resources.Load("Prefabs/MessageRowItem") as GameObject;
    }
    

    public void GetQueueDetails(string qmgrName, string queueFullName)
    {
        string queueName = queueFullName.Substring(qmgrName.Length + 1);

        subwindowDetails.SetActive(true);        
        
        currentQueue = stateComponent.GetQueueDetails(qmgrName, queueName);
        if (currentQueue.holdsMessages)
        {
            currentQueue.messages = stateComponent.GetAllMessages(qmgrName, queueName);
        }
        
        ToQueueDetails();
    }


    // Display queue Details sub-window
    private void ToQueueDetails()
    {
        subwindowDetails.SetActive(true);
        subwindowMessages.SetActive(false);
        subwindowConnections.SetActive(false);

        toQueueDetail.Select();

        detailsQueueName.text = currentQueue.queueName;
        detailsMaxNumberMessage.text = currentQueue.maxNumberOfMessages.ToString();
        detailsMaxMessageLength.text = currentQueue.maxMessageLength.ToString();
        detailsPut.text = currentQueue.inhibitPut.ToString();
        detailsGet.text = currentQueue.inhibitGet.ToString();
        detailsDescription.text = currentQueue.description;
        detailsCreated.text = currentQueue.timeCreated;
        detailsAltered.text = currentQueue.timeAltered;
        detailsDepth.text = currentQueue.currentDepth.ToString();
        detailsType.text = currentQueue.GetTypeName();
    }


    // Display queue Messages sub-window
    private void ToMessages()
    {
        subwindowDetails.SetActive(false);
        subwindowMessages.SetActive(true);
        subwindowConnections.SetActive(false);

        toMessageList.Select();

        // For remote/alias queues do not do anything, since they don't hold messages
        if (!currentQueue.holdsMessages) return;

        // Delete all remaining messages that were already rendered
        foreach (Transform message in transform.Find("Messages/MessagesList"))
        {
            GameObject.Destroy(message.gameObject);
        }

        // Render new messages
        foreach (MQ.Message message in currentQueue.messages)
        {
            GameObject item = Instantiate(messageRowTemplate, transform.Find("Messages/MessagesList"));
            item.transform.Find("Text").GetComponent<Text>().text = message.messageId;

            Button button = item.GetComponent<Button>();
            button.onClick.AddListener(() => GUIUtility.systemCopyBuffer = message.messageId);
        }
    }


    // Display queue Connections sub-window
    private void ToConnections()
    {
        subwindowDetails.SetActive(false);
        subwindowMessages.SetActive(false);
        subwindowConnections.SetActive(true);

        toConnections.Select();

        // Display appropriate information based on what queue it is, because
        // not all types of queues will have all information
        if (currentQueue is MQ.LocalQueue localQueue)
        {
            connectionsOpenInputCount.text = localQueue.openInputCount.ToString();
            connectionsOpenOutputCount.text = localQueue.openOutputCount.ToString();
            connectionsTargetQueue.text = "N/A";
            connectionsTargetQueueManager.text = "N/A";
            connectionsTransmissionQueue.text = "N/A";
        }
        else if (currentQueue is MQ.TransmissionQueue transmissionQueue)
        {
            connectionsOpenInputCount.text = transmissionQueue.openInputCount.ToString();
            connectionsOpenOutputCount.text = transmissionQueue.openOutputCount.ToString();
            connectionsTargetQueue.text = "N/A";
            connectionsTargetQueueManager.text = "N/A";
            connectionsTransmissionQueue.text = "N/A";
        }
        else if (currentQueue is MQ.AliasQueue aliasQueue)
        {
            connectionsOpenInputCount.text = "N/A";
            connectionsOpenOutputCount.text = "N/A";
            connectionsTargetQueue.text = aliasQueue.targetQueueName;
            connectionsTargetQueueManager.text = "N/A";
            connectionsTransmissionQueue.text = "N/A";
        }
        else if (currentQueue is MQ.RemoteQueue remoteQueue)
        {
            connectionsOpenInputCount.text = "N/A";
            connectionsOpenOutputCount.text = "N/A";
            connectionsTargetQueue.text = remoteQueue.targetQueueName;
            connectionsTargetQueueManager.text = remoteQueue.targetQmgrName;
            connectionsTransmissionQueue.text = remoteQueue.transmissionQueueName;
        }
    }

}
