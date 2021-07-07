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
    
    // Buttons
    public Button closeButton;
    public Button detailsButton, messagesButton;

    
    void Start()
    {
        // Initialise with all details
        queueManagerInfoInit("QM1");
        
    }

    void queueManagerInfoInit(string selectedQMName){

        
    }



    /*
    * Button Listener
    */
    void detailsButtonClicked(){

    }

    // Show 
    void messagesButtonClicked(){

    }

    void closeButtonClicked(){

    }
}
