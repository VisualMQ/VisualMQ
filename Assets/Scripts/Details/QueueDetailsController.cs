using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDetailsController : MonoBehaviour
{
    // PUBLIC
    public GameObject QueueDetailLeftWindow;
    public GameObject QueueDetailRightWindow;
    public GameObject QueueDetailLocal;
    public GameObject QueueDetailRemote;
    public GameObject QueueDetialAlias;
    public GameObject QueueDetailTransmission;

    // Text Fields Basic Info
    private Transform textGroup;
    private Text text0_queuename, text1_maxnumbermessage, text2_maxmessagelength, 
            text3_put, text4_get,text5_description, text6_created, text7_altered, 
            text8_hold, text9_depth;
    // Button
    private Button returnButton, closeButton;
    private Button toQueueDetail, toMessageList;

    private void Awake() {

        // Locate the Button
        returnButton = transform.Find("ButtonReturn").GetComponent<Button>();
        closeButton = transform.Find("ButtonCloseQueue").GetComponent<Button>();
        toQueueDetail = transform.Find("ButtonQueueDetails").GetComponent<Button>();
        toMessageList = transform.Find("ButtonMessageList").GetComponent<Button>();

        // Locate the Text
        textGroup = transform.Find("TextFieldsGroup");
        text0_queuename = textGroup.Find("Text").GetComponent<Text>();
        text1_maxnumbermessage = textGroup.Find("Text1").GetComponent<Text>();
        text2_maxmessagelength = textGroup.Find("Text2").GetComponent<Text>();
        text3_put = textGroup.Find("Text3").GetComponent<Text>();
        text4_get = textGroup.Find("Text4").GetComponent<Text>();
        text5_description = textGroup.Find("Text5").GetComponent<Text>();
        text6_created = textGroup.Find("Text6").GetComponent<Text>();
        text7_altered = textGroup.Find("Text7").GetComponent<Text>();
        text8_hold = textGroup.Find("Text8").GetComponent<Text>();
        text9_depth = textGroup.Find("Text9").GetComponent<Text>();

    }

    // Start is called before the first frame update
    void Start()
    {
        /* Button Listeners
        returnButton.onClick.AddListener(ReturnClicked);
        closeButton.onClick.AddListener(CloseClicked);
        toQueueDetail.onClick.AddListener(ToQueueDetailsClicked);
        toMessageList.onClick.AddListener(ToMessageListClicked);*/
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
        Content Generation
    */

    void GetQueueBasicInfo(string queuename)
    {

    }

    void GetQueueLocal(string queuename)
    {

    }

    void GetQueueRemote(string queuename)
    {

    }

    void GetQueueAlias(string queuename)
    {

    }

    void GetQueueTransmission(string queuename)
    {

    }


    /*
    *    Button Listeners
    */
    void ReturnClicked()
    {

    }

    void CloseClicked()
    {
        QueueDetailLeftWindow.SetActive(false);
    }

    void ToQueueDetailsClicked()
    {

    }

    void ToMessageListClicked()
    {

    }
}
