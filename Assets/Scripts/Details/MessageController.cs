using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    // Window GameObject
    public GameObject MessageWindow;

    // Button
    public Button closeButton, closeButtonTop, downLoadButton, copyIdButton;
    private Text text0MessageID, text1Format;
    private Text parentPath;

    void Awake()
    {
        // Locate GameObjects
        text0MessageID = transform.Find("TextMessageID").GetComponent<Text>();
        text1Format = transform.Find("TextMessageFormat").GetComponent<Text>();
        //parentPath = transform.Find("ImagePath").Find("TextParentPath").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MessageWindow.SetActive(false);
        // Button Listener
        closeButton.onClick.AddListener(CloseButtonClicked);
        closeButtonTop.onClick.AddListener(CloseButtonClicked);
        downLoadButton.onClick.AddListener(DownLoadButtonClicked);
        copyIdButton.onClick.AddListener(CopyIdButtonClicked);
    }

    void GenerateMessageWindow(List<string> temp)
    {
        ClearAllFields();

        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        string qmName= temp[0];
        string queueName = temp[1];
        string messID = temp[2];

        int starIdx = qmName.Length;
        string removeQueueName = queueName.Substring(starIdx+1);


        MQ.Message message = stateComponent.GetMessage(qmName, removeQueueName, messID);
        text0MessageID.text = message.messageId;
        text1Format.text = message.format;
    }

    /*
        BUTTON ACTIONS
    */
    void CloseButtonClicked()
    {
        MessageWindow.SetActive(false);
    }

    void DownLoadButtonClicked()
    {

    }

    void CopyIdButtonClicked()
    {

    }

    /*
        CONTENT PREPARE
    */
    void ClearAllFields()
    {
        text0MessageID.text = "";
        text1Format.text = "";
        //parentPath.text  = "";
    }

    
}
