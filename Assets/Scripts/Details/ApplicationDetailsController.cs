using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationDetailsController : MonoBehaviour
{
    //public GameObject channelDetailsWindow;
    private Button closeButton;

    private Text textName, textType, textConnType, textConnName, textConnOpts,
        textAppType, textAppTag, textAppDesc;

    private void Awake()
    {
        MouseListener.applicationDetailsWindow = this;

        closeButton = transform.Find("ButtonClose").GetComponent<Button>();
        textName = transform.Find("TextGroup/Text1").GetComponent<Text>();
        textType = transform.Find("TextGroup/Text2").GetComponent<Text>();
        textConnType = transform.Find("TextGroup/Text3").GetComponent<Text>();
        textConnName = transform.Find("TextGroup/Text4").GetComponent<Text>();
        textConnOpts = transform.Find("TextGroup/Text5").GetComponent<Text>();
        textAppType = transform.Find("TextGroup/Text6").GetComponent<Text>();
        textAppTag = transform.Find("TextGroup/Text7").GetComponent<Text>();
        textAppDesc = transform.Find("TextGroup/Text8").GetComponent<Text>();

    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        closeButton.onClick.AddListener(Close);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Close()
    {
        Reset();
        gameObject.SetActive(false);
    }

    void Reset()
    {
        textName.text = "";
        textType.text = "";
        textConnType.text = "";
        textConnName.text = "";
        textConnOpts.text = "";
        textAppType.text = "";
        textAppTag.text = "";
        textAppDesc.text = "";
    }

    public void GetApplicationDetails(string qmgrName, string applicationName)
    {

        GameObject stateGameObject = GameObject.Find("State");
        State stateComponent = stateGameObject.GetComponent(typeof(State)) as State;
        MQ.Application application = stateComponent.GetApplicationDetails(qmgrName, applicationName);

        textName.text = application.conn;
        textType.text = application.type;
        textConnType.text = "";
        textConnName.text = application.conname;
        textConnOpts.text = String.Join(", ", application.connopts);
        textAppType.text = application.appltype;
        textAppTag.text = application.appltag;
        textAppDesc.text = application.appldesc;

        gameObject.SetActive(true);
    }
}