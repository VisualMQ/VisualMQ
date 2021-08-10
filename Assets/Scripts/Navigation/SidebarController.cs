using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SidebarController : MonoBehaviour
{

    private Button closeButton;
    private GameObject queueDetails;


    void Awake()
    {
        closeButton = transform.Find("ButtonClose").GetComponent<Button>();
        closeButton.onClick.AddListener(CloseSidebar);

        queueDetails = transform.Find("QueueDetails").gameObject;

        MouseListener.sidebarController = this;
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void CloseSidebar()
    {
        gameObject.SetActive(false);
    }

    public void ShowQueueDetails(List<string> temp)
    {
        gameObject.SetActive(true);
        queueDetails.SetActive(true);
        QueueDetailsController controller = queueDetails.GetComponent<QueueDetailsController>();
        controller.GetQueueDetails(temp);
    }
}
