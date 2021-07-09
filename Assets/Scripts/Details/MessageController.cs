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
    public Text text0MessageID, text1Format;
    public Text parentPath;

    // Start is called before the first frame update
    void Start()
    {
        // Button Listener
        closeButton.onClick.AddListener(CloseButtonClicked);
        closeButtonTop.onClick.AddListener(CloseButtonClicked);
        downLoadButton.onClick.AddListener(DownLoadButtonClicked);
        copyIdButton.onClick.AddListener(CopyIdButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        parentPath.text  = "";
    }

    void GenerateMessageWindow(string messageID)
    {

    }
}
