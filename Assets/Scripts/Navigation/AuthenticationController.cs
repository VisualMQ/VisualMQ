using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MQ;

public class AuthenticationController : MonoBehaviour
{

    // Transform & Objects
    private Transform containerQMName, containerUserName, containerAPI, containerURL;
    private InputField userNameInput, apiKey, urlInput, QMInput; // Four Input Fields
    private Text warningURL, warningAPI, warningUserName, warningQueueName; // Four Warning Text Fields

    // Buttons
    private Button submit, cancel;

    // Notification
    public GameObject errorNotification, successNotification;
    private Text successMainText, successTimeText, errorMainText, errorTimeText;


    private string successMessage = "New queue manager added.";
    private string errorMessage = "Failed to add this queue manager. Please try again.";

    private string userNameT, apiKeyT, MQURLT, QMNameT; // Authentication Info

    // Reference NotificationController
    private NotificationController notificationScript;

    private void Awake() 
    {
        // Locate the elements
        containerQMName = transform.Find("QueueManager");
        warningQueueName = containerQMName.Find("HelperQueueManager").GetComponent<Text>();
        QMInput = containerQMName.GetComponent<InputField>();

        containerUserName = transform.Find("Username");
        warningUserName = containerUserName.Find("HelperUsername").GetComponent<Text>();
        userNameInput = containerUserName.GetComponent<InputField>();

        containerAPI = transform.Find("ApiKey");
        warningAPI = containerAPI.Find("HelperApiKey").GetComponent<Text>();
        apiKey = containerAPI.GetComponent<InputField>();

        containerURL = transform.Find("Url");
        warningURL = containerURL.Find("HelperUrl").GetComponent<Text>();
        urlInput = containerURL.GetComponent<InputField>();

        // Buttons
        submit = transform.Find("ButtonSubmit").GetComponent<Button>();
        cancel = transform.Find("ButtonClose").GetComponent<Button>();


        // Notifications texts
        successMainText = successNotification.transform.Find("SuccessTextMain").GetComponent<Text>();
        successTimeText = successNotification.transform.Find("SuccessTextTime").GetComponent<Text>();
        errorMainText = errorNotification.transform.Find("ErrorTextMain").GetComponent<Text>();
        errorTimeText = errorNotification.transform.Find("ErrorTextTime").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Reset();

        // Listen to button activity
        submit.onClick.AddListener(ConfirmButtonClicked);
        cancel.onClick.AddListener(CancelButtonClicked);

    }

    // Confirm Button Clicked
    void ConfirmButtonClicked()
    {
        // Get Current Input Text and Form Checking
        userNameT = userNameInput.text;
        apiKeyT = apiKey.text;
        MQURLT = urlInput.text;
        QMNameT = QMInput.text;

        if (SubmitFormCheck(userNameT, apiKeyT, MQURLT, QMNameT) == false)
        {
            Debug.Log("ERROR: Form Check Fails");
            return;
        }

        // Connect to Queue Manager
        try
        {
            MQ.Client qmClient = new MQ.Client(MQURLT, QMNameT, userNameT, apiKeyT);
            
            GameObject stateGameObject = GameObject.Find("State");
            State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
            stateComponent.AddNewMqClient(qmClient);
        }
        catch
        {
            Debug.Log("Error: Fail to connect to the Queue Manager. Please check your credentials, url, and queue manager's name.");
            GenerateErrorWindow(errorMessage);
            gameObject.SetActive(false);
            Reset(); // Reset all input fields & Warning Label
            return;
        }
        
        Debug.Log("Authentication succeeded.");
        GenerateSuccessWindow(successMessage);
        gameObject.SetActive(false);
        Reset();
    }


    /*  
    * Notification Window Generation as a whole
    */
    void GenerateSuccessWindow(string message)
    {
        successTimeText.text = (DateTime.Now).ToString();
        successMainText.text = message;
        successNotification.SetActive(true);
    }


    void GenerateErrorWindow(string message)
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
    bool SubmitFormCheck(string username, string apikey, string url, string qm)
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
        string warningText = "Please fill this out.";

        if (string.IsNullOrEmpty(url))
        {
            warningURL.text = warningText;
            warningURL.color = Color.red;
            existEmpty = true;
        }
        if (string.IsNullOrEmpty(apikey))
        {
            warningAPI.text = warningText;
            warningAPI.color = Color.red;
            existEmpty = true;
        }
        if (string.IsNullOrEmpty(username))
        {
            warningUserName.text = warningText;
            warningUserName.color = Color.red;
            existEmpty = true;
        }
        if (string.IsNullOrEmpty(qm))
        {
            warningQueueName.text = warningText;
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
        Reset();
        gameObject.SetActive(false); // Hide the authentication window
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
        userNameInput.text = "";
        apiKey.text = "";
        urlInput.text = "";
        QMInput.text = "";
    }


    // The Initial state of all warning labels
    void WarningLabelsInitStatus()
    {
        //warningAPI.text = "Your API key";
        //warningAPI.color = Color.gray;

        //warningQueueName.text = "Name of your queue manager";
        //warningQueueName.color = Color.gray;

        //warningURL.text = "URL to your queue manager";
        //warningURL.color = Color.gray;

        //warningUserName.text = "Your IBM Cloud username";
        //warningUserName.color = Color.gray;

        warningAPI.text = "E.g. I69H42WwUy2fQBbsGvKwFdBBj3ZgtuHEr3vs2xyr0oJ";
        warningAPI.color = Color.gray;

        warningQueueName.text = "E.g. QM1";
        warningQueueName.color = Color.gray;

        warningURL.text = "E.g. https://web-qm1-8543.qm.eu-gb.mq.appdomain.cloud";
        warningURL.color = Color.gray;

        warningUserName.text = "E.g. lillyjohnson";
        warningUserName.color = Color.gray;

    }

}
