using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MQ;

public class AuthenticationController : MonoBehaviour
{
    // This Authentication Object
    public GameObject Authentication;

    // Four Input Fields
    public InputField userName, apiKey, urlInput, QMInput;

    // Four Warning Text Fields
    public Text warningURL, warningAPI, warningUserName, warningQueueName;

    // Buttons
    public Button submit, cancel;

    // Notification
    public GameObject errorNotification, successNotification;
    public Text successMainText, successTimeText, errorMainText, errorTimeText;


    private string successMessage = "A New Queue Manager Added.";
    private string errorMessage = "Fail to add this Queue Manager. Please try later.";

    // Variables for make a connection
    private string userNameT = "yuexu";
    private string apiKeyT = "72L4nsHoYzLRiSecPgJbkNp1imOp0ZeZcUcOoNxoVSvV";
    private string MQURLT = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
    private string QMNameT = "QM1";

    //Show QM
    //public GameObject ToggleQM1;
    //public GameObject ToggleQM2;
    //public int clickTime;
    //public List<GameObject> toggleList = new List<GameObject>();

    // Reference NotificationController
    private NotificationController notificationScript;

    // Start is called before the first frame update
    void Start()
    {
        //Authentication.SetActive(false);
        //Debug.Log("NOTICE: Initialising the authentication field");
        Reset();

        // Listen to button activity
        submit.onClick.AddListener(ConfirmButtonClicked);
        cancel.onClick.AddListener(CancelButtonClicked);

        // TEST
        ConfirmButtonClicked();
    }

    // Confirm Button Clicked
    void ConfirmButtonClicked()
    {
        //Debug.Log("NOTICE: Comfirm Button clicked");

        // Get Current Input Text and Form Checking
        /*
        userNameT = userName.text;
        apiKeyT = apiKey.text;
        MQURLT = urlInput.text;
        QMNameT = QMInput.text;
        */
        if (submitFormCheck(userNameT, apiKeyT, MQURLT, QMNameT) == false)
        {
            Debug.Log("ERROR: Form Check Fails");
            return;
        }

        // Connect to Queue Manager
        try
        {
            MQ.Client qmClient = new MQ.Client(MQURLT, QMNameT, userNameT, apiKeyT);
            qmClient.GetAllChannels();

            GameObject stateGameObject = GameObject.Find("State");
            State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
            stateComponent.AddNewMqClient(qmClient);
        }
        catch
        {
            Debug.Log("Error: Fail to connect to the Queue Manager. Please check your credentials, url, and queue manager's name.");
            generateErrorWindow(errorMessage);
            Authentication.SetActive(false);
            Reset(); // Reset all input fields & Warning Label
            return;
        }
        
        Debug.Log("Authentication succeeded.");
        generateSuccessWindow(successMessage);
        Authentication.SetActive(false);
        Reset();
    }

    /*  
    * Notification Window Generation as a whole
    */
    void generateSuccessWindow(string message)
    {
        successTimeText.text = (DateTime.Now).ToString();
        successMainText.text = message;
        successNotification.SetActive(true);
    }

    void generateErrorWindow(string message)
    {
        errorTimeText.text = (DateTime.Now).ToString();
        errorMainText.text = message;
        errorNotification.SetActive(true);
    }

    /* ---- submitFormCheck ----
    * Check the authentication form before submit
    * Empty check
    * Return false: exist empty
    * Could Add more form check conditions in this method
    */
    bool submitFormCheck(string username, string apikey, string url, string qm)
    {
        bool passFormCheck = true;
        // Exist Empty
        if (EmptyCheck(username, apikey, url, qm))
        {
            passFormCheck = false;
        }
        return passFormCheck;
    }

    /*
    * Perform Empty Check & Update the warning labels
    * Return true: exist empty field
    */
    bool EmptyCheck(string username, string apikey, string url, string qm)
    {
        bool existEmpty = false;

        if (string.IsNullOrEmpty(username))
        {
            warningUserName.text = "Please ";
            warningUserName.color = Color.red;
            existEmpty = true;
        }
        if (string.IsNullOrEmpty(apikey))
        {
            warningAPI.text = "Please";
            warningAPI.color = Color.red;
            existEmpty = true;
        }
        if (string.IsNullOrEmpty(username))
        {
            warningUserName.text = "Please";
            warningUserName.color = Color.red;
            existEmpty = true;
        }
        if (string.IsNullOrEmpty(qm))
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
    void CancelButtonClicked()
    {
        //Debug.Log("NOTICE: Cancel Button clicked");
        Reset();
        Authentication.SetActive(false); // Hide the authentication window
    }

    // Reset: Includes below two functions
    void Reset()
    {
        CleanAllInputField();
        WarningLabelsInitStatus();
    }

    // Clean all input fields
    void CleanAllInputField()
    {
        userName.text = "";
        apiKey.text = "";
        urlInput.text = "";
        QMInput.text = "";
    }

    // The Initial state of all warning labels
    void WarningLabelsInitStatus()
    {
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
