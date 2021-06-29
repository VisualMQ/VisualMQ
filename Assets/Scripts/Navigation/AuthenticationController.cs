using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MQ;

public class AuthenticationController : MonoBehaviour
{
    // UI Object
    public InputField userName, apiKey;
    public InputField urlInput, QMInput;
    public GameObject Authentication;
    public GameObject ToggleQM1;
    public GameObject ToggleQM2;
    
    public Button submit, cancel;
    public Text userDownLabel;
    public Text apiDownLabel;

    public GameObject errorNotification;
    public GameObject successNotification;

    // Variables for make a connection
    private string userNameT = "yuexu";
    private string apiKeyT = "uKnxScu6GXxkbEAWLIGMPCPp5hl1ZPco553uDtWOD620";
    private string MQURLT = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
    private string QMNameT = "QM1";
    
    // Show QM
    public int clickTime;
    public List<GameObject> toggleList = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {   
        toggleList.Add(ToggleQM1);
        toggleList.Add(ToggleQM2);

        Debug.Log("Initialising the authentication field...");
        CleanAllInputField();
        
        // Listen to button activity
        submit.onClick.AddListener(ConfirmButtonClicked);
        cancel.onClick.AddListener(CancelButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update Authentication");
    }


    // Confirm Button Clicked
    void ConfirmButtonClicked()
    {   
        
        Authentication.SetActive(false);
        Debug.Log("xxxxxx Authentication Should Disappear xxxxxx");

        // ToggleQM1.SetActive(true);
        //toggleList[clickTime].SetActive(true);
        //clickTime++;


        Debug.Log("xxxxxx Comfirm Button clicked xxxxx");
        userNameT = userName.text;
        apiKeyT = apiKey.text;
        MQURLT = urlInput.text;
        QMNameT = QMInput.text;
        Debug.Log(apiKeyT);

        try
        {
            QueueManager queue_manager = new QueueManager(MQURLT, QMNameT, userNameT, apiKeyT);
            string allqueue = queue_manager.GetAllQueues(); 
            Debug.Log(allqueue);
        }
        catch
        {
            Debug.Log("Error");
            errorNotification.SetActive(true);
            return;
        }

        //string queueInfo = queue_manager.GetQueue("DEV.QUEUE.1");
        successNotification.SetActive(true);
        Authentication.SetActive(false);
    }

    /*
    Check the authentication form before submit
    1. Empty check
    2. Format check
    3. 
    */
    bool submitFormCheck(string username, string apikey, string url, string qm)
    {

        return true;
    }


    /* Cancel Button Clicked
    */
    void CancelButtonClicked()
    {
        Debug.Log("xxxxxx Cancel Button clicked xxxxxx");
        CleanAllInputField();
        Authentication.SetActive(false);
    }
    
    void CleanAllInputField()
    {
        userName.text = "";
        apiKey.text = "";
        urlInput.text = "";
        QMInput.text = "";
    }

}
