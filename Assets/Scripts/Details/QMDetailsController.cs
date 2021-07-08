using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* If the user selects the QM, show this info panle for Queue manager
*/
public class QMDetailsController : MonoBehaviour
{

    // Queue Manager Configuration Details
    public Text text0Name, text1State, 
        text2InstallationName, text3PermitStandBy, text4IsDefault, 
        text5PublishState, text6ConnectionCount, text7ChannelState, 
        text8Idap, text9StartedTime;

    
    public Button closeButton;
    //public Button detailsButton, messagesButton;
    public string selectedQM = "QM1";
    
    public Button testButton;


    void Clicked(){
        ClearAllInfoFields();
        QueueManagerInfoInit(selectedQM);
    }

    
    void Start()
    {
        testButton.onClick.AddListener(Clicked);
    }


    void Update()
    {

    }


    // Initialise the QM Info using the QM name
    void QueueManagerInfoInit(string selectedQMName)
    {
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


    void MessagesButtonClicked()
    {

    }


    void CloseButtonClicked()
    {

    }

}
