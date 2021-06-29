using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MQ;

public class AuthenticationController : MonoBehaviour
{
    // This Authentication Object
    public GameObject Authentication;

    // Four Input Fields
    public InputField userName, apiKey;
    public InputField urlInput, QMInput;

    // Four Warning Text Fields
    public Text warningURL;
    public Text warningAPI;
    public Text warningUserName;
    public Text warningQueueName;

    // Buttons
    public Button submit, cancel;

    // Notification
    public GameObject errorNotification;
    public GameObject successNotification;

    // Variables for make a connection
    private string userNameT = "";
    private string apiKeyT = "";
    private string MQURLT = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
    private string QMNameT = "";
    
    //Show QM
    //public GameObject ToggleQM1;
    //public GameObject ToggleQM2;
    //public int clickTime;
    //public List<GameObject> toggleList = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {   
        Debug.Log("NOTICE: Initialising the authentication field");
        Reset();
        
        // Listen to button activity
        submit.onClick.AddListener(ConfirmButtonClicked);
        cancel.onClick.AddListener(CancelButtonClicked);
    }

    // Confirm Button Clicked
    void ConfirmButtonClicked()
    {   
        Debug.Log("NOTICE: Comfirm Button clicked");

        // Get Current Input Text and Form Checking
        userNameT = userName.text;
        apiKeyT = apiKey.text;
        MQURLT = urlInput.text;
        QMNameT = QMInput.text;
        if (submitFormCheck(userNameT, apiKeyT, MQURLT, QMNameT) == false){
            Debug.Log("ERROR: Form Check Fails");
            return;
        }

        // Connect to Queue Manager
        QueueManager queue_manager;
        try{
            queue_manager = new QueueManager(MQURLT, QMNameT, userNameT, apiKeyT);
            // string allqueue = queue_manager.GetAllQueues(); 
            // Debug.Log(allqueue);
        }
        catch{
            Debug.Log("Error: Fail to connect to the Queue Manager");
            errorNotification.SetActive(true);
            return;
        }

        successNotification.SetActive(true);
        Authentication.SetActive(false);

        QueueInitilisation(queue_manager);
    }

    /* ---- QueueInitilisation ----
    * If authentication succeeded -> Visualise
    */
    void QueueInitilisation(QueueManager queue_manager){
        //Step two: create QMInfo, QueuesInfo, and List of MessagesInfo objects
        string QMInfo = queue_manager.GetQmgr();
        QMInfo QM1 = new QMInfo(QMInfo);

        string allqueue = queue_manager.GetAllQueues();
        QueuesInfo Qs1 = new QueuesInfo(allqueue);

        List<MessagesInfo> allQMessages = new List<MessagesInfo>();

        //Step three: create a state object that consists of the three objects/list of objects in step three
        State internalState = new State(QM1, Qs1, allQMessages);

        // Display this current QM1
        CreateVisual.VisualizeQM(QM1);
        foreach(QueueStorage storage in internalState.QueuesInfo.storage)
        {
            CreateVisual.VisualizeQueue(QM1, storage);
        }
    }

    /* ---- submitFormCheck ----
    * Check the authentication form before submit
    * Empty check
    * Return false: exist empty
    * Could Add more form check conditions in this method
    */
    bool submitFormCheck(string username, string apikey, string url, string qm){
        bool passFormCheck = true;
        // Exist Empty
        if(EmptyCheck(username, apikey, url, qm)){
            passFormCheck = false;
        }
        return passFormCheck;
    }

    /*
    * Perform Empty Check & Update the warning labels
    * Return true: exist empty field
    */
    bool EmptyCheck(string username, string apikey, string url, string qm){
        bool existEmpty = false;

        if(string.IsNullOrEmpty(username))
        {
            warningUserName.text = "Please ";
            warningUserName.color = Color.red;
            existEmpty = true;
        }
        if(string.IsNullOrEmpty(apikey))
        {
            warningAPI.text = "Please";
            warningAPI.color = Color.red;
            existEmpty = true;
        }
        if(string.IsNullOrEmpty(username))
        {
            warningUserName.text = "Please";
            warningUserName.color = Color.red;
            existEmpty = true;
        }
        if(string.IsNullOrEmpty(qm))
        {
            warningQueueName.text = "Please";
            warningQueueName.color = Color.red;
            existEmpty = true;
        }
        return existEmpty;
    }

    /* 
    * Cancel Button Clicked
    * 1. Clear All Input fields
    * 2. Reset the labels to initial status
    * 3. Hide Window
    */
    void CancelButtonClicked(){
        Debug.Log("NOTICE: Cancel Button clicked");
        Reset();
        Authentication.SetActive(false); // Hide the authentication window
    }

    // Reset: Includes below two functions
    void Reset(){
        CleanAllInputField();
        WarningLabelsInitStatus();
    }

    // Clean all input fields
    void CleanAllInputField(){
        userName.text = "";
        apiKey.text = "";
        urlInput.text = "";
        QMInput.text = "";
    }

    // The Initial state of all warning labels
    void WarningLabelsInitStatus(){
        warningAPI.text = "User's API Key";
        warningAPI.color = Color.gray;

        warningQueueName.text = "The name of the Queue Manager";
        warningQueueName.color = Color.gray;

        warningURL.text = "The URL to your Queue Manager";
        warningURL.color = Color.gray;

        warningUserName.text = "Your user name of the IBM Cloud";
        warningUserName.color = Color.gray;
    }

}
