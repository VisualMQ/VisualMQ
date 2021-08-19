using System;
using UnityEngine;
using UnityEngine.UI;


public class AuthenticationController : MonoBehaviour
{
    private Transform containerQMName, containerUserName, containerAPI, containerURL;
    private InputField userNameInput, apiKey, urlInput, QMInput; // Four input fields
    private Text warningURL, warningAPI, warningUserName, warningQueueName; // Four warning text fields

    // Buttons
    private Button submit, cancel;

    // Notification
    public GameObject errorNotification, successNotification;
    private Text successMainText, successTimeText, errorMainText, errorTimeText;

    private readonly string successMessage = "New queue manager added.";
    private readonly string errorMessage = "Failed to add this queue manager. Please try again.";

    // Variables to hold credentials data
    private string userNameT, apiKeyT, MQURLT, QMNameT;


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
    private void Start()
    {
        Reset();

        // Listen to button activity
        submit.onClick.AddListener(ConfirmButtonClicked);
        cancel.onClick.AddListener(CancelButtonClicked);
    }


    private void ConfirmButtonClicked()
    {
        // Get current input text and form checking
        userNameT = userNameInput.text;
        apiKeyT = apiKey.text;
        MQURLT = urlInput.text;
        QMNameT = QMInput.text;

        if (SubmitFormCheck(userNameT, apiKeyT, MQURLT, QMNameT) == false)
        {
            Debug.Log("ERROR: Form check Failed");
            return;
        }

        // Connect to Queue Manager
        try
        {
            // Throws Exception if username/API key/MQ URL don't match
            MQ.Client qmClient = new MQ.Client(MQURLT, QMNameT, userNameT, apiKeyT);

            // Throws Exception if QM name is wrong
            qmClient.GetQmgr();
            
            GameObject stateGameObject = GameObject.Find("State");
            State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
            stateComponent.AddNewMqClient(qmClient);
        }
        catch
        {
            Debug.Log("Error: Fail to connect to the Queue Manager. Please check your credentials, url, and queue manager's name.");
            GenerateErrorWindow(errorMessage);
            gameObject.SetActive(false);
            Reset(); // Reset all input fields & warning labels
            return;
        }
        
        Debug.Log("Authentication succeeded.");
        GenerateSuccessWindow(successMessage);
        gameObject.SetActive(false);
        Reset();
    }


    private void GenerateSuccessWindow(string message)
    {
        successTimeText.text = (DateTime.Now).ToString();
        successMainText.text = message;
        errorNotification.SetActive(false);
        successNotification.SetActive(true);
    }


    private void GenerateErrorWindow(string message)
    {
        errorTimeText.text = (DateTime.Now).ToString();
        errorMainText.text = message;
        successNotification.SetActive(false);
        errorNotification.SetActive(true);
    }


    // We could add more form checks conditions to this method
    private bool SubmitFormCheck(string username, string apikey, string url, string qm)
    {
        bool passFormCheck = true;
        // Exist Empty
        if (EmptyCheck(username, apikey, url, qm))
        {
            passFormCheck = false;
        }
        return passFormCheck;
    }
    

    // Check whether any field is not empty and updates warning labels accordingly
    private bool EmptyCheck(string username, string apikey, string url, string qmgr)
    {
        bool existEmpty = false;
        string warningText = "Please fill this out.";

        if (string.IsNullOrEmpty(url))
        {
            warningURL.text = warningText;
            warningURL.color = Color.red;
            existEmpty = true;
        } else
        {
            warningURL.text = "";
        }
        if (string.IsNullOrEmpty(apikey))
        {
            warningAPI.text = warningText;
            warningAPI.color = Color.red;
            existEmpty = true;
        } else
        {
            warningAPI.text = "";
        }
        if (string.IsNullOrEmpty(username))
        {
            warningUserName.text = warningText;
            warningUserName.color = Color.red;
            existEmpty = true;
        } else
        {
            warningUserName.text = "";
        }
        if (string.IsNullOrEmpty(qmgr))
        {
            warningQueueName.text = warningText;
            warningQueueName.color = Color.red;
            existEmpty = true;
        } else
        {
            warningQueueName.text = "";
        }
        return existEmpty;
    }


    private void CancelButtonClicked()
    {
        Reset();
        gameObject.SetActive(false);
    }


    private void Reset()
    {
        // Clear all input fields
        userNameInput.text = "";
        apiKey.text = "";
        urlInput.text = "";
        QMInput.text = "";

        // Reset all labels to default
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
