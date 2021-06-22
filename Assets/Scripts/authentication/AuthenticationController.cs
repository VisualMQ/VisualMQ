using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MQ;

public class AuthenticationController : MonoBehaviour
{
    public InputField userName, apiKey;
    public InputField urlInput, QMInput;
    public GameObject Authentication;
    public GameObject ToggleQM1;
    public GameObject ToggleQM2;
    

    public Button submit, cancel;
    public Text userUpLabel, userDownLabel;
    public Text apiUpLabel, apiDownLabel;

    private string userNameT = "yuexu";
    private string apiKeyT = "uKnxScu6GXxkbEAWLIGMPCPp5hl1ZPco553uDtWOD620";
    private string MQURLT = "https://web-qm1-3628.qm.eu-gb.mq.appdomain.cloud:443";
    private string QMNameT = "QM1";
    public int clickTime;
    public List<GameObject> toggleList = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {   
        toggleList.Add(ToggleQM1);
        toggleList.Add(ToggleQM2);

        Debug.Log("Initialising the authentication field...");

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
        toggleList[clickTime].SetActive(true);
        clickTime++;


        Debug.Log("xxxxxx Comfirm Button clicked xxxxx");
        userNameT = userName.text;
        apiKeyT = apiKey.text;
        MQURLT = urlInput.text;
        QMNameT = QMInput.text;
        Debug.Log(apiKeyT);

        QueueManager queue_manager = new QueueManager(MQURLT, QMNameT, userNameT, apiKeyT);
        string queueInfo = queue_manager.GetQueue("DEV.QUEUE.1");
        Debug.Log(queueInfo);
        
        
    }

    // Cancel Button Clicked
    void CancelButtonClicked()
    {
        Debug.Log("xxxxxx Cancel Button clicked xxxxxx");
        CleanAllInputField();
    }
    
    void CleanAllInputField()
    {
        userName.text = "";
        apiKey.text = "";
        urlInput.text = "";
        QMInput.text = "";
    }

}
