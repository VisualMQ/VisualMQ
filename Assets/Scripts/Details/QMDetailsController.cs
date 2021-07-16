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
    
    // TEST: The Button used to trigger the window
    public Button testButton;
    
    void Awake()
    {
        QueueManager.QMDetailWindow = this;
    }

    public void Clicked(){
        ClearAllInfoFields();
        QMDetailsWindow.SetActive(true);
        QueueManagerInfoInit(selectedQM);
    }


    void Start()
    {

        // TEST
        testButton.onClick.AddListener(Clicked);
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

        text0Name.text = queueManager.qmgrName;
        text1State.text = queueManager.state;
        // The following properties might or might not be used
        //text2InstallationName.text = qmDetails[2];
        //text3PermitStandBy.text = qmDetails[3];
        //text4IsDefault.text = qmDetails[4];
        //text5PublishState.text = qmDetails[5];
        //text6ConnectionCount.text = qmDetails[6];
        //text7ChannelState.text = qmDetails[7];
        //text8Idap.text = qmDetails[8];
        //text9StartedTime.text = qmDetails[9];
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
