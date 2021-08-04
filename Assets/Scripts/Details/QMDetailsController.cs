using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* If the user selects the QM, show this info panle for Queue manager
*/
public class QMDetailsController : MonoBehaviour
{

    public GameObject QMDetailsWindow;
    public GameObject QMDetailsRightWindow;

    // Queue Manager Configuration Details
    public Text text0Name, text1State, 
        text2InstallationName, text3PermitStandBy, text4IsDefault, 
        text5PublishState, text6ConnectionCount, text7ChannelState, 
        text8Idap, text9StartedTime;

    // Buttons inside this window
    public Button closeButton;
    public Button qmDetailsButton, queueListButton;

    // The QM name used for trigerring the QM Details Window
    public string selectedQM = "QM1";
    
    
    void Awake()
    {
        QueueManager.QMDetailWindow = this;
    }

    public void Clicked()
    {
        ClearAllInfoFields();
        QMDetailsWindow.SetActive(true);
        QueueManagerInfoInit(selectedQM);
    }


    void Start()
    {

        QMDetailsWindow.SetActive(false);
        QMDetailsRightWindow.SetActive(false);

        // Button Listener
        qmDetailsButton.onClick.AddListener(Clicked);
        queueListButton.onClick.AddListener(QueueListsButtonClicked);
        closeButton.onClick.AddListener(CloseButtonClicked);

    }


    // Initialise the QM Info using the QM name
    public void QueueManagerInfoInit(string selectedQMName)
    {
        QMDetailsWindow.SetActive(true);

        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;

        MQ.QueueManager queueManager = stateComponent.GetSelectedQmgr(selectedQMName);

        // Basic
        text0Name.text = queueManager.qmgrName;
        text1State.text = queueManager.state;
        // Extended
        text2InstallationName.text = queueManager.installationName;
        text3PermitStandBy.text = queueManager.permitStandby.ToString();
        text4IsDefault.text = queueManager.isDefaultQmgr.ToString();
        // Status
        text5PublishState.text = queueManager.publishSubscribeState;
        text6ConnectionCount.text = queueManager.connectionCount.ToString();
        text7ChannelState.text = queueManager.channelInitiatorState;
        text8Idap.text = queueManager.ldapConnectionState;
        text9StartedTime.text = queueManager.started;
    }


    // Clear all fields
    void ClearAllInfoFields()
    {
        text0Name.text = "";
        text1State.text = "";
        text2InstallationName.text = "";
        text3PermitStandBy.text = "";
        text4IsDefault.text = "";
        text5PublishState.text = "";
        text6ConnectionCount.text = "";
        text7ChannelState.text = "";
        text8Idap.text = "";
        text9StartedTime.text = "";
    }


    /*
    * Button Listener
    */
    void DetailsButtonClicked()
    {
        
    }

    void QueueListsButtonClicked()
    {
        Debug.Log("Generate Queue List");
        // Close Current Details Window
        QMDetailsWindow.SetActive(false);
        // Show the Queue List Window
        QMDetailsRightWindow.SetActive(true);
        // Init Queue List
        QMDetailsRightWindow.SendMessage("GenerateQueueList", "QM1");

    }


    void CloseButtonClicked()
    {
        QMDetailsRightWindow.SetActive(false);
        QMDetailsWindow.SetActive(false);
    }

}
