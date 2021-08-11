using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SidebarController : MonoBehaviour
{

    private Button closeButton;
    private GameObject queueDetails, channelDetails, applicationDetails,
        queueManagerDetails;


    void Awake()
    {
        closeButton = transform.Find("ButtonClose").GetComponent<Button>();
        closeButton.onClick.AddListener(Close);

        queueDetails = transform.Find("QueueDetails").gameObject;
        channelDetails = transform.Find("ChannelDetails").gameObject;
        applicationDetails = transform.Find("ApplicationDetails").gameObject;
        queueManagerDetails = transform.Find("QueueManagerDetails").gameObject;

        MouseListener.sidebarController = this;
    }


    void Start()
    {
        gameObject.SetActive(false);
    }


    void Close()
    {
        gameObject.SetActive(false);
    }


    public void ShowQueueDetails(string qmgrName, string queueName)
    {
        gameObject.SetActive(true);
        queueDetails.SetActive(true);
        channelDetails.SetActive(false);
        applicationDetails.SetActive(false);
        queueManagerDetails.SetActive(false);
        QueueDetailsController controller = queueDetails.GetComponent<QueueDetailsController>();
        controller.GetQueueDetails(qmgrName, queueName);
    }


    public void ShowChannelDetails(string qmgrName, string channelName)
    {
        gameObject.SetActive(true);
        queueDetails.SetActive(false);
        channelDetails.SetActive(true);
        applicationDetails.SetActive(false);
        queueManagerDetails.SetActive(false);
        ChannelDetailsController controller = channelDetails.GetComponent<ChannelDetailsController>();
        controller.GetChannelDetails(qmgrName, channelName);
    }


    public void ShowApplicationDetails(string qmgrName, string applicationName)
    {
        gameObject.SetActive(true);
        queueDetails.SetActive(false);
        channelDetails.SetActive(false);
        applicationDetails.SetActive(true);
        queueManagerDetails.SetActive(false);
        ApplicationDetailsController controller = applicationDetails.GetComponent<ApplicationDetailsController>();
        controller.GetApplicationDetails(qmgrName, applicationName);
    }


    public void ShowQueueManagerDetails(string qmgrName)
    {
        gameObject.SetActive(true);
        queueDetails.SetActive(false);
        channelDetails.SetActive(false);
        applicationDetails.SetActive(false);
        queueManagerDetails.SetActive(true);
        QueueManagerDetailsController controller = queueManagerDetails.GetComponent<QueueManagerDetailsController>();
        controller.GetQueueManagerDetails(qmgrName);
    }
}
