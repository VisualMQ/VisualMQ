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

    // Transform & Objects
    private Transform containerQMName, containerUserName, containerAPI, containerURL;
    private InputField userNameInput, apiKey, urlInput, QMInput; // Four Input Fields
    private Text warningURL, warningAPI, warningUserName, warningQueueName; // Four Warning Text Fields

    // Buttons
    public Button submit, cancel;

    // Notification
    public GameObject errorNotification, successNotification;
    public Text successMainText, successTimeText, errorMainText, errorTimeText;


    private string successMessage = "A New Queue Manager Added.";
    private string errorMessage = "Fail to add this Queue Manager. Please try later.";

    private string userNameT, apiKeyT, MQURLT, QMNameT; // Authentication Info

    // Reference NotificationController
    private NotificationController notificationScript;

    private void Awake() 
    {
        // Locate the elements

        containerQMName = transform.Find("QMNameGameObject");
        warningQueueName = containerQMName.Find("TextWarningQueueName").GetComponent<Text>();
        QMInput = containerQMName.Find("InputFieldQueueName").GetComponent<InputField>();

        containerUserName = transform.Find("UserNameGameObject");
        warningUserName = containerUserName.Find("TextWarningTypeUser").GetComponent<Text>();
        userNameInput = containerUserName.Find("InputFieldUserName").GetComponent<InputField>();

        containerAPI = transform.Find("APIGameObject");
        warningAPI = containerAPI.Find("TextWarningAPI").GetComponent<Text>();
        apiKey = containerAPI.Find("InputFieldAPI").GetComponent<InputField>();

        containerURL = transform.Find("URLGameObject");
        warningURL = containerURL.Find("TextWarningURL").GetComponent<Text>();
        urlInput = containerURL.Find("InputFieldURL").GetComponent<InputField>();

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
            Authentication.SetActive(false);
            Reset(); // Reset all input fields & Warning Label
            return;
        }
        
        Debug.Log("Authentication succeeded.");
        GenerateSuccessWindow(successMessage);
        Authentication.SetActive(false);
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
        userNameInput.text = "";
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
